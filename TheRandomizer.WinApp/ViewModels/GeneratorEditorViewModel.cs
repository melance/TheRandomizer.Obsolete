using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TheRandomizer.Generators;
using TheRandomizer.Utility;
using TheRandomizer.WinApp.Commands;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Specialized;

namespace TheRandomizer.WinApp.ViewModels
{
    class GeneratorEditorViewModel : ObservableBase
    {

        #region Constructors
        public GeneratorEditorViewModel()
        {
        }

        public GeneratorEditorViewModel(IDialogCoordinator dialogCoordinator) : this(null, dialogCoordinator) { }

        public GeneratorEditorViewModel(BaseGenerator generator) : this(generator, null) { }

        public GeneratorEditorViewModel(BaseGenerator generator, IDialogCoordinator dialogCoordinator)
        {
            Generator = generator;
            DialogCoordinator = dialogCoordinator;
        }
        #endregion
        
        #region Members
        private List<Models.DropDownButtonItem> _newGeneratorList;

        private List<Models.DropDownButtonItem> _saveList;
        #endregion
        
        #region Public Properties
        public bool IsDirty { get { return GetProperty(false); } set { SetProperty(value); } }
        public string FilePath { get { return GetProperty<string>(); } set { SetProperty(value); OnPropertyChanged("FileName"); } }
        public string FileName { get
            {
                if (string.IsNullOrWhiteSpace(FilePath)) return null;
                return  Path.GetFileName(FilePath) + (IsDirty ? "•" : "");
            }
        }

        public bool HasGenerator
        {
            get
            {
                return Generator != null;
            }
        }

        public string GeneratorXML
        {
            get
            {
                return Generator?.Serialize();
            }
        }
                
        public BaseGenerator Generator
        {
            get
            {
                return GetProperty<BaseGenerator>();
            }
            set
            {
                RemoveEventHandlers();
                SetProperty(value);
                if (Generator != null)
                    SetEventHandlers();
                OnPropertyChanged("HasGenerator");
            }
        }
        
        public List<Models.DropDownButtonItem> NewGeneratorList
        {
            get
            {
                if (_newGeneratorList == null)
                {
                    _newGeneratorList = new List<Models.DropDownButtonItem>()
                    {
                        new Models.DropDownButtonItem("Assignment Generator", "Create a new Assignment Generator", New(typeof(Generators.Assignment.AssignmentGenerator))),
                        new Models.DropDownButtonItem("Dice Generator", "Create a new Dice Generator", New(typeof(Generators.Dice.DiceGenerator))),
                        new Models.DropDownButtonItem("List Generator", "Create a new List Generator", New(typeof(Generators.List.ListGenerator))),
                        new Models.DropDownButtonItem("Lua Generator", "Create a new Lua Generator", New (typeof(Generators.LUA.LuaGenerator))),
                        new Models.DropDownButtonItem("Phonotactics Generator", "Create a new Phonotactics Generator", New (typeof(Generators.Phonotactics.PhonotacticsGenerator))),
                        new Models.DropDownButtonItem("Table Generator", "Create a new Table Generator", New (typeof(Generators.Table.TableGenerator)))
                    };
                    _newGeneratorList = _newGeneratorList.OrderBy(ddbi => ddbi.Text).ToList();
                }
                return _newGeneratorList;
            }
        }

        public List<Models.DropDownButtonItem> SaveList
        {
            get
            {
                if (_saveList == null)
                {
                    _saveList = new List<Models.DropDownButtonItem>() { new Models.DropDownButtonItem("Save As...", "Save As...", SaveAs) };
                }
                return _saveList;
            }
        }

        public Visibility ParameterVisibility
        {
            get
            {
                if (Generator == null) return Visibility.Visible;
                return Generator.GetType().GetProperty("Parameters").HasAttribute(typeof(XmlIgnoreAttribute)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility MaxLengthVisibility
        {
            get
            {
                if (Generator == null) return Visibility.Visible;
                return Generator.GetType().GetProperty("SupportsMaxLength").HasAttribute(typeof(XmlIgnoreAttribute)) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

        #region Private Properties
        private IDialogCoordinator DialogCoordinator { get; set; }
        #endregion 
        
        #region Command Properties
        public ICommand Save
        {
            get
            {
                return new DelegateCommand<bool>(SaveGenerator, false);
            }
        }

        public ICommand SaveAs
        {
            get
            {
                return new DelegateCommand<bool>(SaveGenerator, true);
            }
        }

        public ICommand Open
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    var result = await CancelGeneratorClosing();
                    if (!result.HasValue || result == false)
                    {
                        FilePath = Utility.Dialogs.OpenGeneratorFileDialog(FilePath);
                        if (!string.IsNullOrWhiteSpace(FilePath) && File.Exists(FilePath))
                        {
                            Generator = BaseGenerator.Deserialize(File.ReadAllText(FilePath));
                            IsDirty = false;
                        }
                    }
                });
            }
        }

        private ICommand New(Type generatorType)
        {
            return new DelegateCommand<Type>(async gt =>
            {
                var result = await CancelGeneratorClosing();
                if (!result.HasValue || result == false)
                {
                    Generator = (BaseGenerator)Activator.CreateInstance(generatorType);
                    IsDirty = false;
                }
            }, generatorType);
        }

        private ICommand NewTable(Type tableType)
        {
            return new DelegateCommand<Type>(t =>
                    {
                        if (Generator?.GeneratorType == GeneratorType.Table)
                        {
                            var model = (Generators.Table.BaseTable)Activator.CreateInstance(t);
                            ((Generators.Table.TableGenerator)Generator).Tables.Add(model);
                        }
                    }
                );
        }
        #endregion

        #region Public Methods
        public async Task<bool?> CancelClosing()
        {
            return await CancelGeneratorClosing();
        }
        #endregion 

        #region Private Methods
        private void SetEventHandlers()
        {
            if (Generator != null)
            {
                Generator.PropertyChanged += GeneratorPropertyChanged;
                Generator.Parameters.CollectionChanged += GeneratorCollectionChanged;
            }
        }

        private void RemoveEventHandlers()
        {
            if (Generator != null)
            {
                Generator.PropertyChanged -= GeneratorPropertyChanged;
                Generator.Parameters.CollectionChanged -= GeneratorCollectionChanged;
            }
        }

        private async Task<bool?> CancelGeneratorClosing()
        {
            if (Generator != null && IsDirty && DialogCoordinator != null)
            {
                var settings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Yes",
                    NegativeButtonText = "No",
                    FirstAuxiliaryButtonText = "Cancel",
                    AnimateShow = true,
                    AnimateHide = true,
                    ColorScheme = MetroDialogColorScheme.Accented
                };
                var result = await DialogCoordinator.ShowMessageAsync(this, "Save Changes", "You have unsaved changes, would you like to save them now?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
                switch (result)
                {
                    case MessageDialogResult.Affirmative:
                        Save?.Execute(false);
                        return false;
                    case MessageDialogResult.Negative:
                        return false;
                    case MessageDialogResult.FirstAuxiliary:
                        return true;
                }
            }
            return null;
        }

        private void SaveGenerator(bool saveAs)
        {
            var name = FilePath;
            if (saveAs || string.IsNullOrWhiteSpace(name))
            {
                name = Utility.Dialogs.CreateGeneratorFileDialog(name);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                File.WriteAllText(name, Generator.Serialize());
                FilePath = name;
                IsDirty = false;
            }
        }

        private void GeneratorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsDirty = true;
            OnPropertyChanged("FileName");
            OnPropertyChanged("GeneratorXML");
        }

        private void GeneratorCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }        
        #endregion

    }
}

