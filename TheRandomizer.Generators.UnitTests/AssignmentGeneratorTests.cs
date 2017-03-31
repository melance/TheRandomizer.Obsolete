using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheRandomizer.Generators.Assignment;
using TheRandomizer.Generators.Parameter;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheRandomizer.Generators.UnitTests
{
    [TestClass]
    public class AssignmentGeneratorTests
    {
        private const string TEST_CATEGORY = "Assignment";

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void SimpleStringTest()
        {
            var xml = @"<items>
                            <item name='Start'>Hello World!</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Hello World!", result);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void SimpleItemTest()
        {
            var xml = @"<items>
                            <item name='Start'>Hello [World]!</item>
                            <item name='World'>Earth</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Hello Earth!", result);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void SimpleExpressionTest()
        {
            var xml = @"<items>
                            <item name='Start'>[=2+2]</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("4", result);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void SimpleVariableTest()
        {
            var xml = @"<items>
                            <item name='Start' next='next' variable='blah'>Blah!</item>
                            <item name='next'>[*blah]</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Blah!", result);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void SimpleParameterTest()
        {
            var xml = @"<parameters>
                            <parameter name='Gender' default='Male' />
                        </parameters>
                        <items>
                               <item name='Start'>[*Gender]</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Male", result);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void UserParameterTest()
        {
            var xml = @"<parameters>
                            <parameter name='Gender' default='Female' />
                        </parameters>
                        <items>
                               <item name='Start'>[*Gender]</item>
                        </items>";
            var generator = CreateGenerator(xml);
            generator.Parameters["Gender"].Value = "Male";
            var result = generator.Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Male", result);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void NestedItemTest()
        {
            var xml = @"<items>
                            <item name='Start'>Hello [World[S]]</item>
                            <item name='World'>Earth</item>
                            <item name='Worlds'>Earth and Mars</item>
                            <item name='S'>s</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Hello Earth and Mars", result);
        }
        
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void NestedVariableTest()
        {
            var xml = @"<items>
                            <item name='Start' next='Name' variable='Race'>Elf</item>
                            <item name='Name'>[[*Race]Name]</item>
                            <item name='ElfName'>Legolas</item>
                            <item name='HumanName'>Aragorn</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("Legolas", result);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void ExpressionParameterTest()
        {
            var xml = @"<items>
                            <item name='Start'>[=CInt(Two)+2]</item>
                            <item name='Two'>2</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("4", result);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void LibrarySerializationTest()
        {
            //var library = new Library();
            //library.ItemList.Add(new LineItem());
            //library.ItemList[0].Name = "Test";
            //library.ItemList[0].Expression = "Test Expression";
            //var xml = library.Serialize();
            //Debug.WriteLine(xml);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void ImportLibraryTest()
        {
            var xml = @"<items>
                            <item name='Start'>[Test]</item>
                        </items>
                        <imports>
                            <import>Test</import>
                        </imports>";
            var generator = CreateGenerator(xml);
            generator.RequestGenerator += RequestGeneratorHandler;
            var value = generator.Generate();
            generator.RequestGenerator -= RequestGeneratorHandler;
            Debug.WriteLine(value);
            Assert.AreEqual("Test", value);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void GenerateFunction()
        {
            var xml = @"<items>
                            <item name='Start'>Hello [=Generate('Test')]!</item>
                        </items>";
            var generator = CreateGenerator(xml);
            generator.RequestGenerator += RequestGeneratorHandler;
            var value = generator.Generate();
            generator.RequestGenerator -= RequestGeneratorHandler;
            Debug.WriteLine(value);
            Assert.AreEqual("Hello World!", value);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        [ExpectedException(typeof(Exceptions.MaxRecursionDepthExceededException))]
        public void InfiniteRecursionTest()
        {
            var xml = @"<items>
                            <item name='Start'>[Test]</item>
                            <item name='Test'>[Start]</item>
                        </items>";
            CreateGenerator(xml).Generate();
        }

        [TestMethod]
        public void EsecapeCharaterTest()
        {
            var xml = @"<items>
                            <item name='Start'>\[Test\]</item>
                            <item name='Test'>[Start]</item>
                        </items>";
            var result = CreateGenerator(xml).Generate();
            Debug.Print(result);
            Assert.AreEqual("[Test]", result);
        }

        [TestMethod]
        public void NoItemsTest()
        {
            var generator = CreateGenerator("");
            var result = generator.Generate();
            Debug.Print($"Result: {result}");
            Assert.AreEqual(string.Empty, result);
        }

        #region Private Methods
        private void RequestGeneratorHandler(object sender, RequestGeneratorEventArgs e)
        {
            if (e.Name == "Test")
            {
                var xml = @"<items>
                                <item name='Start'>World</item>
                            </items>";
                e.Generator = CreateGeneratorXml(xml);
            }
        }

        private void RequestLibraryHandler(object sender, RequestImportEventArgs e)
        {
            if (e.Name == "Test")
            {
                var xml = @"<?xml version='1.0' encoding='utf-16'?>
                            <Library xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                                <item name='Test'>Test</item>
                            </Library>";
                e.Library = xml;
            }
        }

        private AssignmentGenerator CreateGenerator(string items)
        {
            var xml = String.Format(XmlWrapper, items);
            return (AssignmentGenerator)BaseGenerator.Deserialize(xml);
        }

        private string CreateGeneratorXml(string items)
        {
            return String.Format(XmlWrapper, items);
        }

        private string XmlWrapper
        {
            get
            {
                return "<?xml version=\"1.0\" encoding=\"utf-16\"?><Grammar xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xsi:type=\"AssignmentGrammar\">{0}</Grammar>";
            }
        }
        #endregion
    }
}
