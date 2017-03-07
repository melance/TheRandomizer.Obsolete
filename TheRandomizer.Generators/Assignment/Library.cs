using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheRandomizer.Generators.Assignment
{
    [XmlType(AnonymousType=true)]
    [XmlRoot("Library", Namespace=null)]
    public class Library
    {
        /// <summary>
        /// Deserializes the provided xml into a library object
        /// </summary>
        /// <param name="xml">The xml to deserialize</param>
        /// <returns>A library object</returns>
        public static Library Deserialize(string xml)
        {
            var serializer = new XmlSerializer(typeof(Library));
            using (var stream = new System.IO.StringReader(xml))
            {
                return (Library)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// The list of line items used to generate the content
        /// </summary>
        [XmlElement("item")]
        public List<LineItem> Items { get; set; } = new List<LineItem>();

        /// <summary>
        /// Serializes this library as an XML string
        /// </summary>
        /// <returns>An xml string</returns>
        public string Serialize()
        {
            var serializer = new XmlSerializer(typeof(Library));
            string value;
            using (var stream = new System.IO.StringWriter())
            {
                serializer.Serialize(stream, this);
                value = stream.ToString();
            }
            return value;
        }
    }
}
