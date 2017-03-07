using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheRandomizer.Generators.List;

namespace TheRandomizer.Generators.Parameter.UnitTests
{
    [TestClass]
    public class ListGeneratorTests
    {
        [TestMethod]
        public void GenerateTest()
        {
            var xml = @"<items>
                            FooBar
                            Hello
                            World
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            if (result != "FooBar" && result != "Hello" && result != "World")
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void WhiteSpaceTest()
        {
            var xml = @"<keepWhitespace>true</keepWhitespace><items> hello world </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual(" hello world ", result);
        }

        [TestMethod]
        public void SerializeTest()
        {
            var generator = new ListGenerator();
            generator.Name = "List Test";
            generator.Description = "Testing the List Generator Serialization";
            generator.Author = "Lance Boudreaux";
            generator.Items = @"This
                                That";
            var result = generator.Serialize();
            Debug.WriteLine(result);
        }

        [TestMethod]
        public void EmptyListTest()
        {
            var generator = new ListGenerator();
            var result = generator.Generate();
            Debug.WriteLine($"Result: {result}");
            Assert.AreEqual(string.Empty, result);
        }

        private ListGenerator CreateGenerator(string items)
        {
            var xml = String.Format(XmlWrapper, items);
            return (ListGenerator)BaseGenerator.Deserialize(xml);
        }        

        private string XmlWrapper
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"utf-16\"?><Grammar xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:type=\"List\">{0}</Grammar>";
            }
        }
    }
}
