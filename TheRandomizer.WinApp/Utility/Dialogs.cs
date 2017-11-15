using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Forms;

namespace TheRandomizer.WinApp.Utility
{
    class Dialogs
    {
        private const string GRAMMAR_EXTENSION = ".xml";
        private const string GENERATOR_EXTENSION = ".rgen";
        private const string RANDOMIZER_EXTENSION_FILTER = "Randomizer Generators (*.rgen)|*.rgen|All Files (*.*)|*.*";
        private static readonly CommonFileDialogFilter[] Extensions = { new CommonFileDialogFilter("The Randomizer Generators (*.rgen)","*.rgen"),
                                                                        new CommonFileDialogFilter("Extensible Markup Language File (*.xml)", "*.xml"),
                                                                        new CommonFileDialogFilter("All Files (*.*)","*.*") };

        public static string OpenFolderDialog(string defaultDirectory)
        {
            return OpenFolder(defaultDirectory, true, true, string.Empty);
        }

        public static string OpenGeneratorFileDialog(string defaultPath)
        {
            //return OpenFile(defaultPath, true, RANDOMIZER_EXTENSION_FILTER);
            return OpenFolder(defaultPath, false, true, GRAMMAR_EXTENSION, Extensions);
        }

        public static string CreatGeneratorFileDialog(string defaultPath)
        {
            //return OpenFile(defaultPath, false, RANDOMIZER_EXTENSION_FILTER);
            return OpenFolder(defaultPath, false, true, GENERATOR_EXTENSION , Extensions);
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

        private static string OpenFolder(string defaultPath,
                                                     bool isFolderPicker,
                                                     bool ensureExists,
                                                     string defaultExtension,
                                                     params CommonFileDialogFilter[] filters)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = isFolderPicker;
                foreach (var filter in filters)
                {
                    dialog.Filters.Add(filter);
                }
                dialog.DefaultExtension = defaultExtension;
                dialog.EnsurePathExists = ensureExists;
                dialog.DefaultDirectory = defaultPath;
                dialog.InitialDirectory = defaultPath;
                CommonFileDialogResult result = dialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            else if (isFolderPicker)
            {
                var dialog = new FolderBrowserDialog();
                dialog.SelectedPath = defaultPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
            return defaultPath;
        }
    }
}
