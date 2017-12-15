using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TheRandomizer.WinApp.Models
{
    public class MRUItem 
    {
        public MRUItem() { }
        public MRUItem(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; set; }
        public string FileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FilePath)) return null;
                return Path.GetFileName(FilePath);
            }
        }
    }
}
