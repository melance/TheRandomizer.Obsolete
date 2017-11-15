using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.WinApp.Models
{
    class GeneratorError
    {
        public GeneratorError(string filePath, string message)
        {
            FilePath = filePath;
            Message = message;
        }

        public string FilePath { get; private set; }
        public string Message { get; private set; }
    }
}
