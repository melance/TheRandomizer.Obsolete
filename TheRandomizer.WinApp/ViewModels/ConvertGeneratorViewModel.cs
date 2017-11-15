using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TheRandomizer.Utility;
using TheRandomizer.WinApp.Commands;
using TheRandomizer.Generators;
using System.IO;
using System.Windows;

namespace TheRandomizer.WinApp.ViewModels
{
    class ConvertGeneratorViewModel : ObservableBase
    {
        public string SourceFile { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string TargetFile { get { return GetProperty<string>(); } set { SetProperty(value); } }

        public ICommand GetSourceFile { get { return new DelegateCommand( () => { SourceFile = Utility.Dialogs.OpenGeneratorFileDialog(SourceFile); }); } }
        public ICommand GetTargetFile { get { return new DelegateCommand(() => { TargetFile = Utility.Dialogs.CreatGeneratorFileDialog(TargetFile); }); } }

        public ICommand Ok { get { return new DelegateCommand<Window>(Convert); } }

        public void Convert(Window window)
        {
            var generator = BaseGenerator.Deserialize(File.ReadAllText(SourceFile));
            Directory.CreateDirectory(Path.GetDirectoryName(TargetFile));
            var option = new Generators.Parameter.Option();
            option.DisplayName = "Any";
            option.Value = "Any";

            generator.Parameters[0].Options.Add(option);
            File.WriteAllText(TargetFile, generator.Serialize());
            window.Close();            
        }        
    }
}
