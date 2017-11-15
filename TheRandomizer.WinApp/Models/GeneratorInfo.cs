using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRandomizer.Generators;
using TheRandomizer.Utility.Collections;

namespace TheRandomizer.WinApp.Models
{
    [Serializable]
    class GeneratorInfo
    {
        public GeneratorInfo() { }

        public GeneratorInfo(string filePath)
        {
            GetGenertorFromFile(filePath);
        }
        
        public string Name { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public string FilePath { get; private set; }
        public ObservableCollection<string> Tags { get; set; }
        public DateTime LastModified { get; private set; }

        private void GetGenertorFromFile(string filePath)
        {
            var xml = File.ReadAllText(filePath);
            var generator = BaseGenerator.Deserialize(xml);
            FilePath = filePath;
            Name = generator.Name;
            Author = generator.Author;
            Description = generator.Description;
            Tags = new ObservableCollection<string>(generator.Tags);
            LastModified = File.GetLastWriteTime(FilePath);
        }
    }
}
