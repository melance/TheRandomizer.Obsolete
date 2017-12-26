using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheRandomizer.Generators.Lua;

namespace TheRandomizer.Generators.Parameter.UnitTests
{
    [TestClass]
    public class LUAGeneratorTest
    {
        [TestMethod]
        public void SimpleFunctionTest()
        {
            var result = Generate("print('Hello World')");
            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void PrintFormatTest()
        {
            var result = Generate("printFormat('{0} World{1}', 'Hello', '!')");
            Assert.AreEqual("Hello World!", result);
        }

        [TestMethod]
        public void CalculationTest()
        {
            var result = Generate("print(calc(\"'Hello World'\"))");
            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void RollTest()
        {
            var result = Generate("print(calc(\"Roll(1,6)\"))");
            if (Int32.Parse(result) < 1 || Int32.Parse(result) > 6)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void VariableTest()
        {
            var result = Generate("a = calc(\"Roll(1,6)\")" + Environment.NewLine +
                                  "print(a)");
            if (Int32.Parse(result) < 1 || Int32.Parse(result) > 6)
            {
                Assert.Fail();
            }
        }

        private string Generate(string script)
        {
            var generator = new LuaGenerator();
            generator.Name = "Test";
            generator.Script = script;
            var result = generator.Generate();
            Debug.Print(result);
            return result;
        }
    }
}
