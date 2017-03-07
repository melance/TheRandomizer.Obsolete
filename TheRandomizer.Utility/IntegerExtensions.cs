using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Utility
{
    public static class IntegerExtensions
    {        
        /// <summary>
        /// Adds the ordinal suffix to an integer
        /// </summary>
        /// <param name="extended">The integer to add the suffix to</param>
        /// <returns>The orginal with it's appropriate suffix</returns>
        /// <remarks>Currently only supports English</remarks>
        public static string ToOrdinal(this Int32 extended)
        {
            Int32 lastDigit = Convert.ToInt32(extended.ToString().Right(1));

            if (extended <= 0)
                return extended.ToString();

            switch (extended % 100)
            {
                case 11:
                case 12:
                case 13:
                    return extended + "th";
            }

            switch (lastDigit)
            {
                case 1:
                    return extended + "st";
                case 2:
                    return extended + "nd";
                case 3:
                    return extended + "rd";
                default:
                    return extended + "th";
            }
        }


        /// <summary>
        /// Converts an integer to its numeric word
        /// </summary>
        /// <param name="extended">The integer to convert</param>
        /// <returns>The numeric word for the integer</returns>
        /// <remarks>Currently only supports English</remarks>
        public static string ToText(this Int32 extended)
        {
            if (extended < 0)
            {
                return Convert.ToString("negative ") + ToText(-extended);
            }
            else if (extended == 0)
            {
                return "zero ";
            }
            else if (extended <= 19)
            {
                return new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" }[extended - 1] + " ";
            }
            else if (extended <= 99)
            {
                return Convert.ToString(new string[] { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" }[extended / 10 - 2] + " ") + ToText(extended % 10);
            }
            else if (extended <= 999)
            {
                return Convert.ToString(ToText(extended / 100) + Convert.ToString("hundred ")) + ToText(extended % 100);
            }
            else if (extended <= 999999)
            {
                return Convert.ToString(ToText(extended / 1000) + Convert.ToString("thousand ")) + ToText(extended % 1000);
            }
            else if (extended <= 999999999)
            {
                return Convert.ToString(ToText(extended / 1000000) + Convert.ToString("million ")) + ToText(extended % 1000000);
            }
            else
            {
                return Convert.ToString(ToText(extended / 1000000000) + Convert.ToString("billion ")) + ToText(extended % 1000000000);
            }
        }
    }
}
