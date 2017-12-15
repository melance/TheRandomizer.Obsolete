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
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                dialog.DefaultDirectory = defaultDirectory;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                var dialog = new FolderBrowserDialog();
                dialog.SelectedPath = defaultDirectory;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
            return defaultDirectory;
        }

        public static string OpenGrammarFileDialog(string defaultPath)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonOpenFileDialog();
                dialog.DefaultDirectory = defaultPath;
                dialog.EnsureFileExists = true;
                dialog.DefaultExtension = "xml";
                dialog.Filters.Add(new CommonFileDialogFilter("Grammar Files", "*.xml"));
                dialog.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                var dialog = new OpenFileDialog();
                dialog.FileName = defaultPath;
                dialog.Filter = "Grammar Files (*.rnd.xml)|*.rnd.xml|All Files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }
            return defaultPath;
        }

        public static string OpenGeneratorFileDialog(string defaultPath)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonOpenFileDialog();
                dialog.DefaultDirectory = defaultPath;
                dialog.EnsureFileExists = true;
                dialog.DefaultExtension = "rgen";
                dialog.Filters.Add(new CommonFileDialogFilter("Generator Files", "*.rgen"));
                dialog.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                var dialog = new OpenFileDialog();
                dialog.FileName = defaultPath;
                dialog.Filter = "Generator Files (*.regen)|*.rgen|All Files (*.*)|*.*";
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
                var dialog = new CommonSaveFileDialog();
                dialog.DefaultDirectory = defaultPath;
                dialog.EnsureFileExists = false;
                dialog.DefaultExtension = "rgen";
                dialog.CreatePrompt = true;
                dialog.Filters.Add(new CommonFileDialogFilter("Generator Files", "*.rgen"));
                dialog.Filters.Add(new CommonFileDialogFilter("All Files", "*.*"));
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else
            {
                var dialog = new SaveFileDialog();
                dialog.FileName = defaultPath;
                dialog.Filter = "Generator Files (*.rgen)|*.rnd.xml|All Files (*.*)|*.*";
                dialog.AddExtension = true;
                dialog.CreatePrompt = true;
                dialog.DefaultExt = "rgen";
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
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = ensureExists;
            dialog.Filter = filter;
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return defaultPath;
        }

       
    }
}
