using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Assignment
{
    public class RequestImportEventArgs : EventArgs
    {
        public RequestImportEventArgs(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the library being requested for import
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The serialized library object to import
        /// </summary>
        public string Library { get; set; }
    }
}
