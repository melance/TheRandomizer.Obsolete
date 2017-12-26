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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

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
                TargetFile = value;
                do
                {
                    TargetFile = Path.GetFileNameWithoutExtension(TargetFile);
                } while (Path.HasExtension(TargetFile));
                TargetFile += ".rgen";
            }
        }

        public string TargetFile { get { return GetProperty<string>(); } set { SetProperty(value); } }

        public List<string> BatchFiles
        {
            get { return GetProperty<List<string>>(); }
            set
            {
                SetProperty(value);
            }
        }

        public ICommand GetSourceFile { get { return new DelegateCommand(() => { SourceFile = Utility.Dialogs.OpenGrammarFileDialog(SourceFile); }); } }
        public ICommand GetTargetFile { get { return new DelegateCommand(() => { TargetFile = Utility.Dialogs.CreateGeneratorFileDialog(TargetFile); }); } }
        public ICommand GetBatchFiles { get { return new DelegateCommand(() => { BatchFiles = Utility.Dialogs.OpenGrammarFileDialog(string.Empty, true); }); } }
        public ICommand ConvertSingle { get { return new DelegateCommand<MetroWindow>(Convert); } }
        public ICommand ConvertBatch { get { return new DelegateCommand<MetroWindow>(MultiConvert); } }

        public async void MultiConvert(MetroWindow window)
        {
            window.Cursor = Cursors.Wait;
            var options = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                FirstAuxiliaryButtonText = "Cancel",
                DefaultButtonFocus = MessageDialogResult.Affirmative
            };
            var result = MessageDialogResult.Negative;
            for (var i = 0; i < BatchFiles.Count() && result != MessageDialogResult.FirstAuxiliary; i++)
            {
                var sourceFile = BatchFiles[i];
                var targetFile = sourceFile;
                result = MessageDialogResult.Affirmative;
                do
                {
                    targetFile = Path.GetFileNameWithoutExtension(targetFile);
                } while (Path.HasExtension(targetFile));
                targetFile += ".rgen";
                var targetPath = Path.Combine(Utility.Settings.GeneratorPath, Path.GetFileName(targetFile));
                if (File.Exists(targetPath) && result != MessageDialogResult.FirstAuxiliary)
                {
                    result = await window.ShowMessageAsync("Overwrite Target File?", targetFile, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, options);
                }
                if (result == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        Convert(sourceFile, targetPath);
                    }
                    catch (Exception ex)
                    {
                        await window.ShowMessageAsync("Error", $"Error converting {sourceFile}. {ex.Message}");
                    }
                }
            }
            window.Cursor = null;
            await window.ShowMessageAsync("Done", "");
        }

        public async void Convert(MetroWindow window)
        {
            try
            {
                var convert = true;
                var targetPath = TargetFile;
                if (!Path.IsPathRooted(targetPath)) targetPath = Path.Combine(Utility.Settings.GeneratorPath, targetPath);
                if (File.Exists(targetPath))
                {
                    var options = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Yes",
                        NegativeButtonText = "No"
                    };
                    var result = await window.ShowMessageAsync("Overwrite Target File?", "", MessageDialogStyle.AffirmativeAndNegative, options);
                    if (result != MessageDialogResult.Affirmative)
                    {
                        convert = false;
                    }
                }
                if (convert)
                {
                    window.Cursor = Cursors.Wait;
                    Convert(SourceFile, targetPath);
                    window.Cursor = null;
                    await window.ShowMessageAsync("Done", "");
                }
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("Error", $"An error occured converting the grammar {ex.Message}.");
            }
            
        }    
        
        private void Convert(string source, string target)
        {
            var generator = BaseGenerator.Deserialize(File.ReadAllText(source));

            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.WriteAllText(target, generator.Serialize());
        }    
    }
}
