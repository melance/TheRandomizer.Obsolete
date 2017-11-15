using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.WinApp.Models
{
    [Serializable]
    class GeneratorInfoCollection : ObservableCollection<GeneratorInfo>
    {
        #region Constants
        private const string GENERATOR_LIST_FILE_NAME = "Generators.lst";
        private const string GENERATOR_FILE_FILTER = "*.rgen";
        private const string GRAMMAR_FILE_FILTER = "*.rgen";
        #endregion

        #region Constructors
        public GeneratorInfoCollection() : base() { }
        public GeneratorInfoCollection(IEnumerable<GeneratorInfo> collection) : base(collection) { }
        public GeneratorInfoCollection(List<GeneratorInfo> list) : base(list) { }
        #endregion

        #region Static Properties
        public static ObservableCollection<GeneratorError> GeneratorLoadErrors { get; private set; } = new ObservableCollection<GeneratorError>();
        
        public static string GeneratorPath
        {
            get
            {
                var value = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.GeneratorDirectory);
                Directory.CreateDirectory(value);
                return value;
            }
        }

        private static string GeneratorListPath
        {
            get
            {
                return Path.Combine(GeneratorPath, GENERATOR_LIST_FILE_NAME);
            }
        }
        #endregion
        
        #region Public Static Methods
        public static GeneratorInfoCollection LoadGeneratorList()
        {
            GeneratorLoadErrors.Clear();
            GeneratorInfoCollection values;
            if (File.Exists(GeneratorListPath))
            {
                try
                {
                    // If the generator list file exists deserialize it
                    var serializer = new BinaryFormatter();
                    using (var stream = new FileStream(GeneratorListPath, FileMode.Open, FileAccess.Read))
                    {
                        values = (GeneratorInfoCollection)serializer.Deserialize(stream);
                    }
                }
                catch
                {
                    values = new GeneratorInfoCollection();
                }
            }
            else
            {
                // If the file doesn't exist, create a new collection
                values = new GeneratorInfoCollection();
            }

            // Remove generators from the list that do not exist in the file system
            for (var i = values.Count - 1; i >=0; i--)
            {
                if (!File.Exists(values[i].FilePath))
                {
                    GeneratorLoadErrors.Add(new GeneratorError(Path.GetFileName(values[i].FilePath), "Could not find this generator file"));
                    values.RemoveAt(i);
                }
            }

            // Loop through all generator files in the directory and update those that have been changed
            var files = new List<string>(Directory.GetFiles(GeneratorPath, GENERATOR_FILE_FILTER));
            files.AddRange(Directory.GetFiles(GeneratorPath, GRAMMAR_FILE_FILTER));
            foreach (string filePath in files)
            {
                try
                {
                    var found = values.Where(gi => gi.FilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase));
                    if (found.Any())
                    {
                        if (found.First().LastModified < File.GetLastWriteTime(filePath))
                        {
                            values.Remove(found.First());
                            values.Add(new GeneratorInfo(filePath));
                        }
                    }
                    else
                    {
                        values.Add(new GeneratorInfo(filePath));
                    }
                }
                catch (Exception ex)
                {
                    GeneratorLoadErrors.Add(new GeneratorError(Path.GetFileName(filePath), ex.Message));
                }
            }
            values.SaveGeneratorList();
            return values;
        }
        #endregion
          
        #region Public Methods
        public List<Tag> GetTags()
        {
            var tags = new List<string>();
            foreach (var item in Items)
            {
                tags.AddRange(item.Tags);
            }
            tags = tags.Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
            return tags.Select(s => new Tag(s)).ToList();
        }

        public void SaveGeneratorList()
        {
            var serializer = new BinaryFormatter();
            using (var file = new FileStream(GeneratorListPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(file, this);
            }
        }
        #endregion 

    }
}
