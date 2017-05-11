using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators
{
    public class RequestGeneratorEventArgs : EventArgs
    {
        public RequestGeneratorEventArgs(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the library being requested for import
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The serialized generator object to import
        /// </summary>
        public BaseGenerator Generator { get; set; }
    }
}
