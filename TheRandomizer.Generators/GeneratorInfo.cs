using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using TheRandomizer.Generators.Attributes;

namespace TheRandomizer.Generators
{
    /// <summary>
    /// Contains only the definition of the generator
    /// </summary>
    public class GeneratorInfo : BaseGenerator, IXmlSerializable
    {
        public new static GeneratorInfo Deserialize(string xml)
        {
            var info = new GeneratorInfo();
            BaseGenerator generator;
            xml = TransformXml.TransformToLatestVersion(xml);
            using (var writer = XmlReader.Create(new StringReader(xml)))
            {
                info.ReadXml(writer);
                generator = BaseGenerator.Deserialize(writer);
            }
            return info;
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
            Name = generator.Name;
            Author = generator.Author;
            Description = generator.Description;
            OutputFormat = generator.OutputFormat;
            SupportsMaxLength = generator.SupportsMaxLength;
            foreach (var tag in generator.Tags)
            {
                Tags.Add(tag);
            }
            Url = generator.Url;
            Version = generator.Version;
            Published = generator.Published;
            if (generator.GetType() == typeof(Assignment.AssignmentGenerator)) IsLibrary = ((Assignment.AssignmentGenerator)generator).IsLibrary;
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
