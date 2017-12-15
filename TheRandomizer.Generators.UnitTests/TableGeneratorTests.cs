using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheRandomizer.Generators.Table;

namespace TheRandomizer.Generators.Parameter.UnitTests
{
    [TestClass]
    public class TableGeneratorTests
    {
        [TestMethod]
        public void SimpleRandomTable()
        {
            var generator = new TableGenerator();
            var table = new RandomTable();
            table.Name = "Simple";
            table.Column = "Roll";
            table.Value = @"Roll | Result
                            1    | Hello
                            2    | Goodbye";
            generator.Output = "[Simple.Result] World!";
            generator.Tables.Add(table);
            var result = generator.Generate();
            Debug.WriteLine(result);
            if (result != "Hello World!" && result != "Goodbye World!")
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SimpleSelectTable()
        {
            var generator = new TableGenerator();
            var table = new SelectTable();
            table.Name = "Simple";
            table.Column = "Roll";
            table.Value = @"Roll | Result
                            1    | Hello
                            2    | Goodbye";
            table.SelectValue = "2";
            generator.Output = "[Simple.Result] World!";
            generator.Tables.Add(table);
            var result = generator.Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Goodbye World!", result);
        }

        [TestMethod]
        public void SimpleLoopTable()
        {
            var generator = new TableGenerator();
            var table = new LoopTable();
            table.Name = "Simple";
            table.Column = "Roll";
            table.Value = @"Roll | Result
                            1    | Hello
                            2    | Goodbye";
            generator.Output = "[Simple.2.Result] World!";
            generator.Tables.Add(table);
            var result = generator.Generate();
            Debug.WriteLine(result);
            if (result != "Hello World!" && result != "Goodbye World!")
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SerializeTest()
        {
            var generator = new TableGenerator();
            var table = new LoopTable();
            table.Name = "Simple";
            table.Column = "Roll";
            table.Value = @"Roll | Result
                            1    | Hello
                            2    | Goodbye";
            generator.Output = "[Simple.2.Result] World!";
            generator.Tables.Add(table);
            var result = generator.Serialize();
            Debug.WriteLine(result);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            var xml = @"<tables><table action=""Random"" column=""Roll"" name=""Test"" delimiter=""|"">
                        Roll|Value
                        100 | Hello World!</table></tables>
                        <output>[Test.Value]</output>";
            var generator = CreateGenerator(xml);
            var result = generator.Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Hello World!", result);
        }

        private TableGenerator CreateGenerator(string items)
        {
            var xml = String.Format(XmlWrapper, items);
            return (TableGenerator)BaseGenerator.Deserialize(xml);
        }

        private string CreateGeneratorXml(string items)
        {
            return String.Format(XmlWrapper, items);
        }

        private string XmlWrapper
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"utf-16\"?><Grammar xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:type=\"TableGrammar\">{0}</Grammar>";
            }
        }
    }
}
