using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string extended)
        {
            Int32 value;
            if (Int32.TryParse(extended, out value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Remove(this string extended, params string[] toRemove)
        {
            var value = extended;
            foreach (var s in toRemove)
            {
                extended.Replace(s, string.Empty);
            }
            return value;
        }

        public static IEnumerable<string> SplitLines(this string extended, bool keepEmptyLines)
        {
            using (System.IO.StringReader reader = new System.IO.StringReader(extended))
            {
                var line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (keepEmptyLines || !string.IsNullOrWhiteSpace(line))
                    {
                        yield return line;
                    }
                } while (line != null);
            }
        }

        public static string Left(this string extended, Int32 count)
        {
            return extended.Substring(count);
        }

        public static string Right(this string extended, Int32 count)
        {
            return extended.Substring(extended.Length - count);
        }
    }
}
