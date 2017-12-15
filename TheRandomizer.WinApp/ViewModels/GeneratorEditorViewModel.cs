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
using TheRandomizer.Generators.Table;
using System.Data;

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

        private Models.MRU _mru;
        #endregion
        
        #region Public Properties
        public Models.MRU MRU
        {
            get
            {
                if (_mru == null)
                {
                    _mru = Models.MRU.LoadMRU();
                    _mru.CollectionChanged += MRUChanged;
                }

                return _mru;
            }
        }

        public bool HasMRUItems
        {
            get
            {
                return MRU.Count > 0;
            }
        }

        public List<string> Names
        {
            get
            {
                var items = (Generator as Generators.Assignment.AssignmentGenerator)?.LineItems;
                if (items != null)
                {
                    return items.Select(i => i.Name).Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
                }
                return new List<string>();
            }
        }

        public bool IsDirty { get { return GetProperty(false); } set { SetProperty(value); } }
        public string FilePath { get { return GetProperty<string>(); } set { SetProperty(value); OnPropertyChanged("FileName"); } }

        public Cursor Cursor { get { return GetProperty<Cursor>(); } set { SetProperty(value); } }

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
                OnPropertyChanged("");
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
                        var filePath = Utility.Dialogs.OpenGeneratorFileDialog(FilePath);
                        Cursor = Cursors.Wait;
                        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                        {
                            FilePath = filePath;
                            Generator = BaseGenerator.Deserialize(File.ReadAllText(FilePath));
                            IsDirty = false;
                            MRU.Add(FilePath);
                        }
                        Cursor = null;
                    }
                });
            }
        }

        public ICommand OpenRecent
        {
            get
            {
                return new DelegateCommand<string>(async filePath =>
                {
                    var result = await CancelGeneratorClosing();
                    if (!result.HasValue || result == false)
                    {
                        if (File.Exists(filePath))
                        {
                            Cursor = Cursors.Wait;
                            FilePath = filePath;
                            Generator = BaseGenerator.Deserialize(File.ReadAllText(filePath));
                            IsDirty = false;
                            MRU.Add(FilePath);
                        }
                        Cursor = null;
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
                    FilePath = string.Empty;
                }
            }, generatorType);
        }

        public BaseTable SelectedTable { get { return GetProperty<BaseTable>(); } set { SetProperty(value); } }

        public ICommand AddTable
        {
            get
            {
                return new DelegateCommand<Type>(t =>
                        {
                            if (Generator?.GeneratorType == GeneratorType.Table)
                            {
                                var model = (BaseTable)Activator.CreateInstance(t);
                                ((TableGenerator)Generator).Tables.Add(model);
                                model.Name = t.DisplayName();
                                SelectedTable = model;
                            }
                        }
                    );
            }
        }

        public ICommand AddColumn
        {
            get
            {
                return new DelegateCommand<BaseTable>(async table =>
                {
                    if (table != null)
                    {
                        var name = await DialogCoordinator.ShowInputAsync(this, "Add Column", "Enter the column name");
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            var converter = new Converters.TableConverter();
                            var data = (DataTable)converter.Convert(table.Value, typeof(DataTable), null, System.Globalization.CultureInfo.CurrentCulture);
                            data.Columns.Add(name);
                            table.Value = (string)converter.ConvertBack(data, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture);
                        }
                    }
                });
            }
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
                if (Generator.GeneratorType == GeneratorType.Assignment)
                {
                    ((Generators.Assignment.AssignmentGenerator)Generator).LineItems.CollectionChanged += LineItemsChanged;
                }
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
                Cursor = Cursors.Wait;
                File.WriteAllText(name, Generator.Serialize());
                FilePath = name;
                IsDirty = false;
                Cursor = null;
                MRU.Add(FilePath);
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

        private void LineItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Names");
        }

        private void MRUChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("HasMRUItems");
        }
        #endregion

    }
}

