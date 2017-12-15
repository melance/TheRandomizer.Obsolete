using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace TheRandomizer.Utility
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Attempts to serialize the <paramref name="value"/> using the default xml serialization options.
        /// </summary>
        /// <param name="value">The object to serialize</param>
        /// <param name="xml">The value of the serialization</param>
        /// <returns>True if the serialization is successful; otherwise false.</returns>
        public static bool TrySerialize(this object value, out string xml)
        {
            try
            {
                xml = Serialize(value);
                return true;
            }
            catch
            {
                xml = string.Empty;         
            }
            return false;         
        }

        /// <summary>
        /// Serializes the <paramref name="value"/> using the default xml serialization options.
        /// </summary>
        /// <param name="value">The object to serialize</param>
        /// <returns>The serialized object</returns>
        public static string Serialize(this object value)
        {
            var serializer = new XmlSerializer(value.GetType());
            using (var stream = new StringWriter())
            {
                serializer.Serialize(stream, value);
                return stream.ToString();
            }                
        }
    }
}
