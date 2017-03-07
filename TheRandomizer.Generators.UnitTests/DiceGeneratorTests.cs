using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TheRandomizer.Generators.UnitTests
{
    [TestClass]
    public class DiceGeneratorTests
    {
        [TestMethod]
        public void SimpleCalculation()
        {
            var generator = new Dice.DiceGenerator();
            generator.Functions.Add(new Dice.RollFunction());
            generator.Functions[0].Function = @"2+2";
            var result = generator.Generate();
            Debug.WriteLine(result);
            Assert.AreEqual("4", result);
        }

        [TestMethod]
        public void SimpleRoll()
        {
            var generator = new Dice.DiceGenerator();
            generator.Functions.Add(new Dice.RollFunction());
            generator.Functions[0].Function = @"Roll(2,6)";
            var result = generator.Generate();
            Debug.WriteLine(result);
            if (Int32.Parse(result) < 2 || Int32.Parse(result) > 12)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void MultiLineRoll()
        {
            var generator = new Dice.DiceGenerator();
            generator.Functions.Add(new Dice.RollFunction());
            generator.Functions[0].Function = @"'2+2 = ' + (2+2)
                                                'Roll(2,6) = ' + Roll(2,6)
                                                CBool(1)";
            var result = generator.Generate();
            Debug.WriteLine(result);
        }

        [TestMethod]
        public void VariableAssignment()
        {
            var generator = new Dice.DiceGenerator();
            generator.Functions.Add(new Dice.RollFunction());
            generator.Functions[0].Function = @"Four := 2+2
                                                Four";
            var result = generator.Generate();
            Debug.WriteLine(result);
        }

        [TestMethod]
        public void DeserializeTestV1()
        {
            var xml = @"<?xml version =""1.0"" encoding=""utf-8""?>
                        <Grammar xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xsi:type=""DiceRoll"">
                            <name>Test</name>
                            <function name=""test"">Roll(3,6)</function>
                        </Grammar>";
            var generator = BaseGenerator.Deserialize(xml);
            var result = generator.Generate();
            Debug.WriteLine($"Result: {result}");
        }

        [TestMethod]
        public void DeserializeTestV2()
        {
            var xml = @"<?xml version =""1.0"" encoding=""utf-16""?>
                        <generator xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xsi:type=""Dice"" version=""2"">
                            <name>Test</name>
                            <function name=""test"">Roll(3,6)</function>
                        </generator>";
            var generator = BaseGenerator.Deserialize(xml);
            var result = generator.Generate();
            Debug.WriteLine($"Result: {result}");
        }

        [TestMethod]
        public void SerializeTest()
        {
            var generator = new Dice.DiceGenerator();
            generator.Functions.Add(new Dice.RollFunction());
            generator.Functions[0].Function = @"Four := 2+2
                                                Four";
            var result = generator.Serialize();
            Debug.WriteLine(result);
        }

        [TestMethod]
        public void MultipleFunctionTest()
        {
            var generator = new Dice.DiceGenerator();
            var classic = new Dice.RollFunction() { Name = "3d6", DisplayName = "Classic", Function = "Roll(3,6,0,'EX')" };
            var modern = new Dice.RollFunction() { Name = "4d6", DisplayName = "Modern", Function = "Roll(4,6,0,'DL',1)" };
            generator.Functions.Add(classic);
            generator.Functions.Add(modern);
            generator.Parameters["RollFunction"].Value = "3d6";
            var result = generator.Generate();
            Debug.WriteLine(result);
        }
    }
}
