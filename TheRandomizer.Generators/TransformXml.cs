using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml.XPath;
using System.Xml;

namespace TheRandomizer.Generators
{
    internal class TransformXml
    {
        /// <summary>
        /// Returns the version of the xml provided
        /// </summary>
        /// <param name="xml">The xml to find the version of</param>
        /// <returns>A Version object</returns>
        private static Int32 GetXmlVersion(string xml)
        {
            var document = new XmlDocument();
            XmlAttribute versionValue;
            Int32 version;

            document.LoadXml(xml);

            versionValue = document.DocumentElement.Attributes["version"];

            if (versionValue == null ||
                !string.IsNullOrWhiteSpace(versionValue.Value) ||
                !Int32.TryParse(versionValue.Value, out version))
            {
                // If no version attribute is available assume version 1
                version = 1;
            }

            return version;
        }

        /// <summary>
        /// Transforms the provided xml to the latest version allowing backwards compatibility
        /// </summary>
        /// <param name="xml">The xml to transform</param>
        public static string TransformToLatestVersion(string xml)
        {
            // Determine what version of the xml is being used
            var version = GetXmlVersion(xml);

            // Use the appropriate transform the update the xml
            switch (version)
            {
                case 1: return TransformVersion1ToVersion2(xml);
            }
            return xml;
        }

        /// <summary>
        /// Transforms the incoming XML to V2
        /// </summary>
        private static string TransformVersion1ToVersion2(string xml)
        {
            if (!string.IsNullOrWhiteSpace(xml))
            {
                XDocument doc;
                XElement root;
                XAttribute type;

                using (var reader = new StringReader(xml))
                {
                    doc = XDocument.Load(reader);
                }

                // Change the name of the root from Grammar to generator
                root = doc.Root;
                root.Name = "generator";

                // Update the value of the @type attribute
                type = root.Attribute(XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance"));

                switch (type.Value)
                {
                    case "AssignmentGrammar": type.Value = "Assignment"; break;
                    case "DiceRoll": type.Value = "Dice"; break;
                    case "LuaGrammar": type.Value = "Lua"; break;
                    case "PhonotacticsGrammar": type.Value = "Phonotactics"; break;
                    case "TableGrammar": type.Value = "Table"; break;
                }

                //Update table elements to the new names
                if (type.Value == "Table")
                {
                    var tables = doc.XPathSelectElements("//generator/tables/table"); // (XName.Get("table", "http://www.w3.org/2001/XMLSchema"));
                    foreach (var table in tables)
                    {
                        var action = table.Attribute(XName.Get("action"));
                        switch (action.Value)
                        {
                            case "Random": table.Name = "randomTable"; break;
                            case "Select": table.Name = "selectTable"; break;
                            case "Loop":
                                table.Name = "loopTable";
                                var loopId = table.Attribute(XName.Get("loopId"));
                                var column = new XAttribute(XName.Get("column"), loopId.Value);
                                loopId.Remove();
                                table.Add(column);
                                break;
                        }
                        action.Remove();
                    }
                }

                // Save the new document to a string and return it
                using (var writer = new StringWriter())
                {
                    doc.Save(writer);
                    return writer.ToString();
                }
            }
            return xml;
        }
    }
}
