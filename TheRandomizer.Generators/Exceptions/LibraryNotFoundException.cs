using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Exceptions
{
    /// <summary>
    /// Thrown when a request for library returns null
    /// </summary>
    public class LibraryNotFoundException : Exception
    {
        public LibraryNotFoundException(string name) : base($"Unable to locate the library named '{name}'")
        {
        }
    }
}
