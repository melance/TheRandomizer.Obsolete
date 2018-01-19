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
            GetReleases();
            Tags = new ObservableList<Models.Tag>();
            Tags.ItemPropertyChanged += Tags_ItemPropertyChanged;
        }
        public MainWindowViewModel(IDialogCoordinator dialogCoordinator) : this()
        {
            DialogCoordinator = dialogCoordinator;
        }
        #endregion

        #region Properties
        public bool LoadingGenerators { get { return GetProperty(false); } set { SetProperty(value); } }

        private MainWindow MainWindowInstance { get { return Application.Current.MainWindow as MainWindow; } }
        private IDialogCoordinator DialogCoordinator { get; set; }

        public Cursor Cursor { get { return GetProperty<Cursor>(); } set { SetProperty(value); } }
        public ObservableList<Models.Tag> Tags { get { return GetProperty<ObservableList<Models.Tag>>(); } private set { SetProperty(value); } } 
        public GeneratorInfoCollection Generators {
            get
            {
                return GetProperty<GeneratorInfoCollection>();
            }
            set
            {
                SetProperty(value);
                Tags.Clear();
                Tags.AddRange(value.GetTags());
                OnPropertyChanged("FilteredGenerators");
                OnPropertyChanged("Tags");
            }
        }
        public GeneratorInfoCollection FilteredGenerators
        {
            get
            {
                if (Tags.Count > 0)
                {
                    return new GeneratorInfoCollection(Generators.Where(bg => bg.Tags == null || bg.Tags.Count() == 0 || bg.Tags.Intersect(SelectedTags, TagComparer.Instance).Count() > 0));
                }
                else
                {
                    return Generators;
                }
            }
        }
        protected List<Generators.Tag> SelectedTags
        {
            get
            {
                return Tags.Where(t => t.Selected).Select(t => new Generators.Tag(t.Name)).ToList();
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

        public Octokit.Release NewRelease { get { return GetProperty<Octokit.Release>(); } set { SetProperty(value); OnPropertyChanged("NewReleaseAvailable"); } }
        public bool NewReleaseAvailable { get { return NewRelease != null; } }
        #endregion  

        #region Commands
        public ICommand CloseAllGenerators
        {
            get
            {
                return new DelegateCommand(() => { LoadedGenerators.Clear(); });
            }
        }

        public ICommand CloseAllOtherGenerators
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    LoadedGenerators.RemoveAll(gw => gw != SelectedGenerator);
                });
            }
        }

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

        public ICommand CreateGenerator
        {
            get
            {
                return new DelegateCommand(
                () =>
                {
                    MainWindowInstance.flyTools.IsOpen = false;
                    var editor = new GeneratorEditor();                    
                    editor.ShowActivated = true;
                    editor.Show();
                });
            }
        }
               
        public ICommand SelectGenerator
        {
            get { return new DelegateCommand<string>(LaunchGenerator); }
        }

        public ICommand ConvertGenerator
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var convert = new Views.ConvertGenerator();
                    convert.Owner = MainWindowInstance;
                    convert.ShowDialog();
                    MainWindowInstance.flyTools.IsOpen = false;
                });
            }
        }
        #endregion

        #region Private Methods
        private async void GetReleases()
        {
            if (Properties.Settings.Default.CheckUpdates)
            {
                var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("TheRandomizer"));
                var releases = await client.Repository.Release.GetAll("melance", "TheRandomizer");
                Octokit.Release newRelease = null;
                Version newReleaseVersion = null;

                if (releases?.Count > 0)
                {
                    foreach (var release in releases)
                    {
                        if (!release.Prerelease || Properties.Settings.Default.IncludeBeta)
                        {
                            var version = new Version(release.TagName);
                            if (version > CurrentVersion && (newReleaseVersion == null || version > newReleaseVersion))
                            {
                                newRelease = release;
                                newReleaseVersion = version;
                            }
                        }
                    }
                }

                if (newRelease != null) NewRelease = newRelease;
            }
        }

        private Version CurrentVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private void ToggleTagSelection(ToggleType toggle)
        {
            foreach (Models.Tag tag in Tags)
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
            LoadingGenerators = true;
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

        private void LoadGenerators_Progress(string fileName, int count, int max)
        {
        }
                
        private void LoadGenerators_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Generators = e.Result as GeneratorInfoCollection;
            Tags.Clear();
            if (Generators != null) Tags.AddRange(Generators.GetTags());
            OnPropertyChanged("FilteredGenerators");
            OnPropertyChanged("LoadErrorCount");
            LoadingGenerators = false;
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
