using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TheRandomizer.Generators.Attributes;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace TheRandomizer.Generators.Assignment
{
    [XmlType(AnonymousType=true)]
    [XmlRoot("library", Namespace=null)]
    [GeneratorDisplay("Assignment Library", "A reusable item list for use in Assignment Generators.")]
    public class Library : BaseGenerator
    {
        private const Int32 LATEST_VERSION = 2;

        /// <summary>
        /// Deserializes the provided xml into a library object
        /// </summary>
        /// <param name="xml">The xml to deserialize</param>
        /// <returns>A library object</returns>
        public static Library Deserialize(string xml)
        {
            var serializer = new XmlSerializer(typeof(Library));
            xml = TransformToLatestVersion(xml);
            using (var stream = new System.IO.StringReader(xml))
            {
                return (Library)serializer.Deserialize(stream);
            }
        }

        private static string TransformToLatestVersion(string xml)
        {
            XDocument doc;
            XElement root;
            XAttribute version;
            Int32 versionNumber = 0;

            using (var reader = new StringReader(xml))
            {
                doc = XDocument.Load(reader);
            }

            root = doc.Root;
            
            version = root.Attribute("version");
            if (version == null || Int32.TryParse(version.Value, out versionNumber))
            {
                switch (versionNumber)
                {
                    case 1: return TransformVersion1toVersion2(xml);
                    default: return TransformVersion1toVersion2(xml);
                }
            }

            return xml;
        }

        private static string TransformVersion1toVersion2(string xml)
        {
            XDocument doc;
            XElement root;
            List<XElement> items;
            XAttribute version;
            
            // Open the xml as an XDocument
            using (var reader = new StringReader(xml))
            {
                doc = XDocument.Load(reader);
            }
            var itemList = new XElement("items");

            // Get the root and set the version number
            root = doc.Root;
            version = root.Attribute("version");
            if (version == null) { version = new XAttribute("version", LATEST_VERSION); }
            version.Value = LATEST_VERSION.ToString();

            // Move the item list into the items element
            items = new List<XElement>(root.XPathSelectElements("/library/item"));

            foreach (var i in items)
            {
                i.Remove();
                itemList.Add(i);
            }

            root.Add(itemList);

            // Save the new document to a string and return it
            using (var writer = new StringWriter())
            {
                doc.Save(writer);
                return writer.ToString();
            }

        }
        
        /// <summary>
        /// Libraries do not implement Generate Internal
        /// </summary>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        protected override string GenerateInternal(int? maxLength)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The list of line items added to the importing Assignment Generator
        /// </summary>
        [XmlArray("items")]
        [XmlArrayItem("item")]
        [RequireOneElement(ErrorMessage = "You must include at least one Line Item.")]
        public List<LineItem> ItemList { get; set; } = new List<LineItem>();
    }
}
