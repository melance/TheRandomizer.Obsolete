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
        public string SourceFile
        {
            get { return GetProperty<string>(); }
            set
            {
                SetProperty(value);
                if (string.IsNullOrWhiteSpace(TargetFile) && !string.IsNullOrWhiteSpace(value))
                {
                    TargetFile = value;
                    do
                    {
                        TargetFile = Path.GetFileNameWithoutExtension(TargetFile);
                    } while (Path.HasExtension(TargetFile));
                    TargetFile += ".rgen";
                }
            }
        }

        public string TargetFile { get { return GetProperty<string>(); } set { SetProperty(value); } }

        public ICommand GetSourceFile { get { return new DelegateCommand( () => { SourceFile = Utility.Dialogs.OpenGeneratorFileDialog(SourceFile); }); } }
        public ICommand GetTargetFile { get { return new DelegateCommand(() => { TargetFile = Utility.Dialogs.CreateGeneratorFileDialog(TargetFile); }); } }

        public ICommand Ok { get { return new DelegateCommand<Window>(Convert); } }

        public void Convert(Window window)
        {
            var generator = BaseGenerator.Deserialize(File.ReadAllText(SourceFile));
            Directory.CreateDirectory(Path.GetDirectoryName(TargetFile));
            File.WriteAllText(TargetFile, generator.Serialize());
            window.Close();            
        }        
    }
}
