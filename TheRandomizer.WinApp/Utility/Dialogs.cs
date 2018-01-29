using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Forms;
using System.Windows.Input;
using TheRandomizer.WinApp.Commands;
using System.Windows;

namespace TheRandomizer.WinApp.Utility
{
    class Dialogs
    {
        private const string GRAMMAR_EXTENSION = "xml";
        private const string GENERATOR_EXTENSION = "rgen";
        private const string RANDOMIZER_EXTENSION_FILTER = "Randomizer Generators (*.rgen)|*.rgen|All Files (*.*)|*.*";
        private static readonly CommonFileDialogFilter[] Extensions = { new CommonFileDialogFilter("The Randomizer Generators (*.rgen)","*.rgen"),
                                                                        new CommonFileDialogFilter("Extensible Markup Language File (*.xml)", "*.xml"),
                                                                        new CommonFileDialogFilter("All Files (*.*)","*.*") };

        public static ICommand Ok
        {
            get
            {
                return new DelegateCommand<Window>(w =>
                                                   {
                                                       w.DialogResult = true;
                                                       w.Close();
                                                   });
            }
        }

        public static ICommand Cancel
        {
            get
            {
                return new DelegateCommand<Window>(w =>
                {
                    w.DialogResult = false;
                    w.Close();
                });
            }
        }

        public static string OpenFolderDialog(string defaultDirectory)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    EnsurePathExists = true,
                    DefaultDirectory = defaultDirectory
                };
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                var dialog = new FolderBrowserDialog
                {
                    SelectedPath = defaultDirectory
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
            return defaultDirectory;
        }

        public static string OpenGrammarFileDialog(string defaultPath)
        {
            return OpenGrammarFileDialog(defaultPath, false).FirstOrDefault();
        }

        public static List<string> OpenGrammarFileDialog(string defaultPath, bool multiSelect)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonOpenFileDialog
                {
                    DefaultDirectory = defaultPath,
                    EnsureFileExists = true,
                    DefaultExtension = "xml",
                    Multiselect = multiSelect
                };
                dialog.Filters.Add(new CommonFileDialogFilter("Grammar Files", "*.xml"));
                dialog.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return new List<string>(dialog.FileNames);
                }
            }
            else
            {
                var dialog = new OpenFileDialog
                {
                    FileName = defaultPath,
                    Filter = "Grammar Files (*.rnd.xml)|*.rnd.xml|All Files (*.*)|*.*"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return new List<string>(dialog.FileNames);
                }
            }
            return new List<string>(new string[] { defaultPath });
        }

        public static string OpenGeneratorFileDialog(string defaultPath)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonOpenFileDialog
                {
                    DefaultDirectory = defaultPath,
                    EnsureFileExists = true,
                    DefaultExtension = "rgen"
                };
                dialog.Filters.Add(new CommonFileDialogFilter("Generator Files", "*.rgen"));
                dialog.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                var dialog = new OpenFileDialog
                {
                    FileName = defaultPath,
                    Filter = "Generator Files (*.regen)|*.rgen|All Files (*.*)|*.*"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }
            return defaultPath;
        }

        public static string CreateGeneratorFileDialog(string defaultPath)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonSaveFileDialog
                {
                    DefaultDirectory = defaultPath,
                    EnsureFileExists = false,
                    DefaultExtension = "rgen",
                    CreatePrompt = true
                };
                dialog.Filters.Add(new CommonFileDialogFilter("Generator Files", "*.rgen"));
                dialog.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                var dialog = new SaveFileDialog
                {
                    FileName = defaultPath,
                    Filter = "Generator Files (*.rgen)|*.rnd.xml|All Files (*.*)|*.*",
                    AddExtension = true,
                    CreatePrompt = true,
                    DefaultExt = "rgen"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }
            return defaultPath;
        }

        private static string OpenFile(string defaultPath,
                                       bool ensureExists,
                                       string filter)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = ensureExists,
                Filter = filter,
                FilterIndex = 0
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return defaultPath;
        }

       
    }
}
