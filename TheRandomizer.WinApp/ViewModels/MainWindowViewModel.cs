using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using System.Configuration;
using TheRandomizer.Generators;
using TheRandomizer.Generators.List;
using TheRandomizer.Utility.Collections;
using System.ComponentModel;
using TheRandomizer.WinApp.Commands;
using TheRandomizer.WinApp.Utility;
using TheRandomizer.Utility;
using System.Windows;
using TheRandomizer.WinApp.Models;
using System.Threading;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using TheRandomizer.WinApp.Views;

namespace TheRandomizer.WinApp.ViewModels
{
    class MainWindowViewModel : ObservableBase
    {
        #region Enumerators
        public enum ToggleType
        {
            None,
            All,
            Flip
        }
        #endregion

        #region Constants
        
        #endregion

        #region Members
        private InterTabClient _interTabClient;
        private BackgroundWorker _loadGeneratorsWorker;
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            Tags.ItemPropertyChanged += Tags_ItemPropertyChanged;
        }
        #endregion

        #region Properties
        private MainWindow MainWindowInstance { get { return Application.Current.MainWindow as MainWindow; } }

        public Cursor Cursor { get { return GetProperty<Cursor>(); } set { SetProperty(value); } }
        public ObservableList<Tag> Tags { get; } = new ObservableList<Tag>();
        public GeneratorInfoCollection Generators { get; private set; }
        public GeneratorInfoCollection FilteredGenerators
        {
            get
            {
                if (Tags.Count > 0)
                {
                    return new GeneratorInfoCollection(Generators.Where(bg => bg.Tags == null || bg.Tags.Count() == 0 || bg.Tags.Intersect(SelectedTags).Count() > 0));
                }
                else
                {
                    return Generators;
                }
            }
        }
        protected List<string> SelectedTags
        {
            get
            {
                return Tags.Where(t => t.Selected).Select(t => t.Name).ToList();
            }
        }

        public InterTabClient InterTabClientInstance
        {
            get
            {
                if (_interTabClient == null) _interTabClient = new InterTabClient();
                return _interTabClient;
            }
            set
            {
                _interTabClient = value;
            }
        }

        public ObservableCollection<GeneratorWrapper> LoadedGenerators { get; } = new ObservableCollection<GeneratorWrapper>();
        public GeneratorWrapper SelectedGenerator { get { return GetProperty<GeneratorWrapper>(); } set { SetProperty(value); } }

        public int LoadErrorCount { get { return GeneratorInfoCollection.GeneratorLoadErrors.Count(); } }
        #endregion  

        #region Commands
        public ICommand ShowLoadErrors
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var loadErrors = new LoadErrorDialog();
                    loadErrors.DataContext = GeneratorInfoCollection.GeneratorLoadErrors;
                    loadErrors.ShowDialog();
                });
            }
        }

        public ICommand RefreshGenerators
        {
            get { return new DelegateCommand(LoadGenerators); }
        }

        public ICommand SelectAllTags
        {
            get { return new DelegateCommand<ToggleType>(ToggleTagSelection, ToggleType.All); }
        }

        public ICommand UnselectAllTags
        {
            get { return new DelegateCommand<ToggleType>(ToggleTagSelection, ToggleType.None); }
        }

        public ICommand ToggleTags
        {
            get { return new DelegateCommand<ToggleType>(ToggleTagSelection, ToggleType.Flip); }
        }

        public ICommand GeneratorsHelp
        {
            get { return new DelegateCommand(ShowGeneratorsHelp); }
        }

        public ICommand GetMoreGenerators
        {
            get {
                return new DelegateCommand(
                () =>
                {
                    Cursor = Cursors.Wait;
                    var view = new GetMoreGenerators();
                    Cursor = null;
                    view.ShowDialog();
                }
            ); }
        }

        public ICommand GeneratorEditor
        {
            get
            {
                return new DelegateCommand<Type>(
                t =>
                {
                    if (!t.IsSubclassOf(typeof(BaseGenerator))) throw new ArgumentException();
                    var model = new GeneratorEditorViewModel(Activator.CreateInstance(t) as BaseGenerator);
                    var editor = new GeneratorEditor();
                    editor.DataContext = model;
                    editor.Show();
                });
            }
        }

        public ICommand About
        {
            get { return new DelegateCommand(ShowAbout); }
        }
        
        public ICommand SelectGenerator
        {
            get { return new DelegateCommand<string>(LaunchGenerator); }
        }
        #endregion

        #region Private Methods
        private void ToggleTagSelection(ToggleType toggle)
        {
            foreach (Tag tag in Tags)
            {
                switch (toggle)
                {
                    case ToggleType.All: tag.Selected = true;
                        break;
                    case ToggleType.None: tag.Selected = false;
                        break;
                    case ToggleType.Flip: tag.Selected = !tag.Selected;
                        break;
                }
            }
        }

        public void LoadGenerators()
        {
            Cursor = Cursors.Wait;
            _loadGeneratorsWorker = new BackgroundWorker();
            _loadGeneratorsWorker.WorkerReportsProgress = true;
            _loadGeneratorsWorker.DoWork += LoadGenerators_DoWork;
            _loadGeneratorsWorker.RunWorkerCompleted += LoadGenerators_Completed;
            _loadGeneratorsWorker.RunWorkerAsync();
        }

        private void ShowGeneratorsHelp()
        {
            System.Windows.Forms.Help.ShowHelp(null, @"help\TheRandomizer.Generators.chm");
        }

        private void ShowAbout()
        {
            var about = new Views.About();
            about.ShowDialog();            
        }
        
        private void LaunchGenerator(string filePath)
        {
            Cursor = Cursors.Wait;
            var worker = new BackgroundWorker();
            worker.DoWork += LaunchGenerator_DoWork;
            worker.RunWorkerCompleted += LaunchGenerator_RunWorkerCompleted;
            worker.RunWorkerAsync(filePath);
        }

        private void LaunchGenerator_DoWork(object sender, DoWorkEventArgs e)
        {
            var xml = File.ReadAllText(e.Argument as string);
            var generator = BaseGenerator.Deserialize(xml);
            var model = new GeneratorWrapper(generator, e.Argument as string);
            e.Result = model;
        }        

        private void LaunchGenerator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var model = e.Result as GeneratorWrapper;
            LoadedGenerators.Add(model);
            SelectedGenerator = model;
            Cursor = null;
        }

        public void LoadGenerators_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Directory.Exists(GeneratorInfoCollection.GeneratorPath))
            {
                e.Result = GeneratorInfoCollection.LoadGeneratorList(LoadGenerators_Progress);                
            }
            else
            {
                MessageBox.Show($"Unable to locate the Generator Directory: {GeneratorInfoCollection.GeneratorPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGenerators_Progress(string fileName)
        {

        }
                
        private void LoadGenerators_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Generators = e.Result as GeneratorInfoCollection;
            Tags.Clear();
            Tags.AddRange(Generators.GetTags());
            OnPropertyChanged("FilteredGenerators");
            OnPropertyChanged("LoadErrorCount");
            Cursor = null;
        }
        #endregion

        #region Event Handlers
        private void Tags_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("FilteredGenerators");
        }

        #endregion
    }
}
