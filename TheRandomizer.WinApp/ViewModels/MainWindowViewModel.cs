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
        private Utility.InterTabClient _interTabClient;
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            Tags.ItemPropertyChanged += Tags_ItemPropertyChanged;
            LoadGenerators();
        }

        #endregion

        #region Properties
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
        #endregion  

        #region Public Methods
        public ICommand RefreshGenerators
        {
            get { return new DelegateCommand(RefreshGeneratorList); }
        }

        public ICommand SelectAllTags
        {
            get { return new DelegateCommand<ToggleType>(ToggleTagSelction, ToggleType.All); }
        }

        public ICommand UnselectAllTags
        {
            get { return new DelegateCommand<ToggleType>(ToggleTagSelction, ToggleType.None); }
        }

        public ICommand ToggleTags
        {
            get { return new DelegateCommand<ToggleType>(ToggleTagSelction, ToggleType.Flip); }
        }

        public ICommand Help
        {
            get { return new DelegateCommand(ShowHelp); }
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
        private void ToggleTagSelction(ToggleType toggle)
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

        private void RefreshGeneratorList()
        {
            LoadGenerators();
        }

        private void ShowHelp()
        {
            MessageBox.Show("Coming Soon!");
        }

        private void ShowAbout()
        {
            var about = new Views.About();
            about.ShowDialog();            
        }
        
        private void LaunchGenerator(string filePath)
        {
            var xml = File.ReadAllText(filePath);
            var generator = BaseGenerator.Deserialize(xml);
            var model = new GeneratorWrapper(generator);
            LoadedGenerators.Add(model);
            SelectedGenerator = model;
        }

        public void LoadGenerators()
        {
            if (Directory.Exists(GeneratorInfoCollection.GeneratorPath))
            {
                Generators = GeneratorInfoCollection.LoadGeneratorList();
                OnPropertyChanged("FilteredGenerators");
                Tags.Clear();
                Tags.AddRange(Generators.GetTags());
                if (Properties.Settings.Default.ShowGeneratorLoadErrors && GeneratorInfoCollection.GeneratorLoadErrors.Count > 0)
                {
                    if (!DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow))
                    {
                        var loadErrors = new Views.LoadErrorDialog();
                        loadErrors.DataContext = GeneratorInfoCollection.GeneratorLoadErrors;
                        if (loadErrors.ShowDialog() == false)
                        {
                            Application.Current.Shutdown(0);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show($"Unable to locate the Generator Directory: {GeneratorInfoCollection.GeneratorPath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
