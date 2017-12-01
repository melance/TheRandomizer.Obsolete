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

namespace TheRandomizer.WinApp.ViewModels
{
    class GeneratorEditorViewModel : ObservableBase
    {
        public GeneratorEditorViewModel()
        {
        }

        public GeneratorEditorViewModel(BaseGenerator generator)
        {
            Generator = generator;
        }

        public BaseGenerator Generator { get { return GetProperty<BaseGenerator>(); } set { SetProperty(value); } }

        public string FileName { get { return GetProperty<string>(); } set { SetProperty(value); } }

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

        private void SaveGenerator(bool saveAs)
        {
            var name = FileName;
            if (saveAs || string.IsNullOrWhiteSpace(name))
            {
                name = Utility.Dialogs.CreateGeneratorFileDialog(name);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                File.WriteAllText(name, Generator.Serialize());
                FileName = name;
            }
        }

    }
}
