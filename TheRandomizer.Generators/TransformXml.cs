using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using TheRandomizer.Utility;

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
                XElement category;
                XElement system;
                XElement genre;
                XElement tags;

                using (var reader = new StringReader(xml))
                {
                    doc = XDocument.Load(reader);
                }

                // Change the name of the root from Grammar to generator
                root = doc.Root;
                if (root.Name.LocalName.Equals("library"))
                {
                    var generator = new Assignment.AssignmentGenerator();
                    foreach (var item in root.Elements())
                    {
                        var newItem = new Assignment.LineItem()
                        {
                            Name = item.Attribute("name").Value,
                            Next = item.Attribute("next")?.Value,
                            Variable = item.Attribute("variable")?.Value,
                            Expression = item.Value
                        };
                        var weight = item.Attribute("weight")?.Value;

                        if (weight.IsNumeric())
                        {
                            newItem.Weight = int.Parse(weight);
                        }
                        else
                        {
                            newItem.Weight = 1;
                        }

                        generator.LineItems.Add(newItem);
                    }
                    generator.IsLibrary = true;
                    xml = generator.Serialize();
                }
                else
                {
                    root.Name = "generator";

                    // Add the version number
                    var version = root.Attribute(XName.Get("version"));
                    if (version == null) version = new XAttribute("version", 2);
                    version.Value = "2";

                    // Update the value of the @type attribute
                    type = root.Attribute(XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance"));

                    switch (type.Value.ToLower())
                    {
                        case "assignmentgrammar": type.Value = "Assignment"; break;
                        case "diceroll": type.Value = "Dice"; break;
                        case "luagrammar": type.Value = "Lua"; break;
                        case "phonotacticsgrammar": type.Value = "Phonotactics"; break;
                        case "tablegrammar": type.Value = "Table"; break;
                    }

                    // Move Cateogry, System, and genre to tags
                    category = root.XPathSelectElement("/generator/category");
                    system = root.XPathSelectElement("/generator/system");
                    genre = root.XPathSelectElement("/generator/genre");
                    tags = root.XPathSelectElement("/generator/tags");

                    if (tags == null)
                    {
                        tags = new XElement(XName.Get("tags"));
                        root.Add(tags);
                    }
                
                    if (category != null) { tags.Add(new XElement(XName.Get("tag"), category.Value)); }
                    if (system != null) { tags.Add(new XElement(XName.Get("tag"), system.Value)); }
                    if (genre != null) { tags.Add(new XElement(XName.Get("tag"), genre.Value)); }

                    //Change "Checkbox" parameters to "Boolean"
                    var parameters = root.XPathSelectElements("/generator/parameters/parameter");
                    
                    foreach (var parameter in parameters)
                    {
                        var parameterType = parameter.Attribute("type");
                        if (parameterType.Value.Equals("checkbox", StringComparison.InvariantCultureIgnoreCase))
                        {
                            parameterType.Value = "Boolean";
                        }
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
            }
            return xml;
        }
    }
}
