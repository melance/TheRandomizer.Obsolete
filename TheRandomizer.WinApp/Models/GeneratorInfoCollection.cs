using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.WinApp.Models
{
    public class GeneratorInfoCollection : ObservableCollection<GeneratorInfo>
    {        
        #region Constants
        private const string GENERATOR_LIST_FILE_NAME = "Generators.lst";
        private const string GENERATOR_FILE_FILTER = "*.rgen";
        private const string GRAMMAR_FILE_FILTER = "*.rnd.xml";
        #endregion

        #region Constructors
        public GeneratorInfoCollection() : base() { }
        public GeneratorInfoCollection(IEnumerable<GeneratorInfo> collection) : base(collection) { }
        public GeneratorInfoCollection(List<GeneratorInfo> list) : base(list) { }
        #endregion

        #region Static Members
        private static GeneratorInfoCollection _generatorList;
        #endregion  

        #region Static Properties
        public static ObservableCollection<GeneratorError> GeneratorLoadErrors { get; private set; } = new ObservableCollection<GeneratorError>();
        
        public static string GeneratorPath
        {
            get
            {
                var value = Environment.ExpandEnvironmentVariables(Utility.Settings.GeneratorDirectory);
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

        public static GeneratorInfoCollection GeneratorList
        {
            get
            {
                if (_generatorList == null) LoadGeneratorList(null);
                return _generatorList;
            }
        }

        public static List<string> Tags
        {
            get
            {
                return _generatorList.GetTags().Select(t => t.Name).ToList();
            }
        }        
        #endregion
        
        #region Public Static Methods
        public static GeneratorInfoCollection LoadGeneratorList(Action<string, int, int> progressCallback)
        {
            GeneratorLoadErrors.Clear();
            var values = new GeneratorInfoCollection(); ;
            if (File.Exists(GeneratorListPath))
            {
                try
                {
                    // If the generator list file exists deserialize it
                    var serializer = new XmlSerializer(typeof(GeneratorInfoCollection));
                    using (var reader = new StringReader(File.ReadAllText(GeneratorListPath)))
                    {
                        values = (GeneratorInfoCollection)serializer.Deserialize(reader);
                    }
                }
                catch
                {
                }
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
            var searchOption = Utility.Settings.IncludeGeneratorSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = new List<string>(Directory.GetFiles(GeneratorPath, GENERATOR_FILE_FILTER, searchOption));
            var count = 0;
            var max = files.Count();

            files.AddRange(Directory.GetFiles(GeneratorPath, GRAMMAR_FILE_FILTER, searchOption));
            foreach (string filePath in files)
            {
                count++;
                progressCallback?.Invoke(Path.GetFileNameWithoutExtension(filePath), count, max);
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
                    GeneratorLoadErrors.Add(new GeneratorError(Path.GetFileName(filePath), ex.ToString()));
                }
            }            
            values.SaveGeneratorList();
            _generatorList = new GeneratorInfoCollection(values.Where(gi => !gi.IsLibrary).OrderBy(gi => gi.Name ));
            return _generatorList;
        }
        #endregion
          
        #region Public Methods
        public List<Tag> GetTags()
        {
            var tags = new List<string>();
            foreach (var item in Items)
            {
                tags.AddRange(item.Tags.Select(t => t.Value));
            }
            tags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();
            return tags.Select(s => new Tag(s)).OrderBy(t => t.Name).ToList();
        }

        public void SaveGeneratorList()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(GeneratorInfoCollection));
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, this);
                    File.WriteAllText(GeneratorListPath, writer.ToString());
                }
            }
            catch
            {
            }
        }
        #endregion 

    }
}
