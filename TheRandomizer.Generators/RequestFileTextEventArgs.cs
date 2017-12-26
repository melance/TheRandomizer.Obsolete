using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators
{
    public class RequestFileTextEventArgs : EventArgs
    {
        public RequestFileTextEventArgs(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
        public string FileText { get; set; }
    }
}
