using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Table
{
    internal class EvaluateArgs : EventArgs
    {
        /// <summary>The expression to evaluate</summary>
        public string Expression { get; set; }
        /// <summary>The result of the evaluation</summary>
        public object Result { get; set; }
    }
}
