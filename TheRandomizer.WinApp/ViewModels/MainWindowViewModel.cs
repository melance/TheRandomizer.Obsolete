using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TheRandomizer.WinApp.ViewModels
{
    public class MainWindowViewModel : ObservableBase
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
        private const string MORE_GENERATORS_URL_KEY = "MoreGeneratorsURL";
        #endregion

        #region Members
        private Utility.InterTabClient _interTabClient;
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            Tags.ItemPropertyChanged += Tags_ItemPropertyChanged;
            //TODO: Remove debug code
            Tags.Add(new Tag("Hello"));
            Tags.Add(new Tag("World"));
            Generators.Add(new ListGenerator() { Name = "Test Generator 1", Id = Guid.NewGuid(), Description="Testing the description", Author="Lance the Boudreaux" });
            Generators[0].Tags.Add("World");
            Generators.Add(new ListGenerator() { Name = "Test Generator 2", Id = Guid.NewGuid(), Description="This is the best generator EVAR!", Author="Some Gal" });
            Generators[1].Tags.Add("Hello");
            Generators[0].Parameters.Add(new Generators.Parameter.Configuration() { Name = "Test", DisplayName = "Display", Type = TheRandomizer.Generators.Parameter.Configuration.ParameterType.Text });
        }

        #endregion

        #region Properties
        public ObservableList<Tag> Tags { get; } = new ObservableList<Tag>();
        public ObservableCollection<BaseGenerator> Generators { get; } = new ObservableCollection<BaseGenerator>();
        public ObservableCollection<BaseGenerator> FilteredGenerators
        {
            get
            {
                if (Tags.Count > 0)
                {
                    return new ObservableCollection<BaseGenerator>(Generators.Where(bg => bg.Tags == null || bg.Tags.Count() == 0 || bg.Tags.Intersect(SelectedTags).Count() > 0));
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

        public Utility.InterTabClient InterTabClientInstance
        {
            get
            {
                if (_interTabClient == null) _interTabClient = new Utility.InterTabClient();
                return _interTabClient;
            }
            set
            {
                _interTabClient = value;
            }
        }

        public ObservableCollection<GeneratorTabItemViewModel> LoadedGenerators { get; } = new ObservableCollection<GeneratorTabItemViewModel>();
        public GeneratorTabItemViewModel SelectedGenerator { get { return GetProperty<GeneratorTabItemViewModel>(); } set { SetProperty(value); } }
        #endregion  

        #region Public Methods
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

        public ICommand GetMoreGenerators
        {
            get { return new DelegateCommand(GeneratorsLink); }
        }

        public ICommand Help
        {
            get { return new DelegateCommand(ShowHelp); }
        }

        public ICommand About
        {
            get { return new DelegateCommand(ShowAbout); }
        }

        public ICommand Preferences
        {
            get { return new DelegateCommand(ShowPreferences); }
        }

        public ICommand Donate
        {
            get { return new DelegateCommand(ShowDonate); }
        }

        public ICommand SelectGenerator
        {
            get { return new DelegateCommand<Guid>(LaunchGenerator); }
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

        private void GeneratorsLink()
        {
            var url = ConfigurationManager.AppSettings.Get(MORE_GENERATORS_URL_KEY);
            var info = new ProcessStartInfo(url);
            Process.Start(info);
        }

        private void ShowHelp()
        {
            System.Windows.MessageBox.Show("You have been helped!");
        }

        private void ShowAbout()
        {
            System.Windows.MessageBox.Show("This is what it is all about!");
        }

        private void ShowPreferences()
        {
            System.Windows.MessageBox.Show("What would you prefer?");
        }

        private void ShowDonate()
        {
            System.Windows.MessageBox.Show("Thanks for the help yo!");
        }

        private void LaunchGenerator(Guid generatorId)
        {
            var generator = Generators.Where(g => g.Id == generatorId).FirstOrDefault();
            if (generator != null)
            {
                var model = new GeneratorTabItemViewModel(generator);
                LoadedGenerators.Add(model);
                SelectedGenerator = model;
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
