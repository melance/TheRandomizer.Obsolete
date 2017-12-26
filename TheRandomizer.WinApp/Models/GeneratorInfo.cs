using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRandomizer.Generators;
using TheRandomizer.Utility.Collections;
using TheRandomizer.WinApp.Commands;

namespace TheRandomizer.WinApp.Models
{
    public class GeneratorInfo
    {
        public GeneratorInfo() { }

        public GeneratorInfo(string filePath)
        {
            GetGenertorFromFile(filePath);
        }
        
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public ObservableCollection<Generators.Tag> Tags { get; set; }
        public DateTime LastModified { get; set; }
        public GeneratorType GeneratorType { get; set; }
        public bool IsLibrary { get; set; } = false;

        private void GetGenertorFromFile(string filePath)
        {
            var xml = File.ReadAllText(filePath);
            var generator = BaseGenerator.Deserialize(xml);
            FilePath = filePath;
            Name = generator.Name;
            Author = generator.Author;
            Description = generator.Description;
            GeneratorType = generator.GeneratorType;
            if (GeneratorType == GeneratorType.Assignment)
            {
                IsLibrary = ((Generators.Assignment.AssignmentGenerator)generator).IsLibrary;
            }
            Tags = new ObservableCollection<Generators.Tag>(generator.Tags);
            LastModified = File.GetLastWriteTime(FilePath);
        }
    }
}
