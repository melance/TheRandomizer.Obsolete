using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;

namespace TheRandomizer.Generators
{
    /// <summary>
    /// Contains only the definition of the generator
    /// </summary>
    public class GeneratorInfo : BaseGenerator, IXmlSerializable
    {
        public new static GeneratorInfo Deserialize(string xml)
        {
            var generator = new GeneratorInfo();
            xml = TransformXml.TransformToLatestVersion(xml);
            using (var writer = XmlReader.Create(new StringReader(xml)))
            {
                generator.ReadXml(writer);
            }
            return generator;
        }

        public bool IsLibrary { get; set; } = false;
        
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // Deserialize
            var generator = BaseGenerator.Deserialize(reader);
            this.Name = generator.Name;
            this.Author = generator.Author;
            this.Description = generator.Description;
            this.OutputFormat = generator.OutputFormat;
            this.SupportsMaxLength = generator.SupportsMaxLength;
            this.Tags.AddRange(generator.Tags);
            this.Url = generator.Url;
            this.Version = generator.Version;
            if (generator.GetType() == typeof(Assignment.AssignmentGenerator)) this.IsLibrary = ((Assignment.AssignmentGenerator)generator).IsLibrary;
        }

        public void WriteXml(XmlWriter writer)
        {
            // Serialize
            this.Serialize(writer);
        }

        protected override string GenerateInternal(int? maxLength)
        {
            throw new NotImplementedException();
        }
    }
}
