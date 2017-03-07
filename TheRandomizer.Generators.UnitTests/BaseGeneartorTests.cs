using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheRandomizer.Generators.Assignment;
using TheRandomizer.Generators.Dice;
using System.Collections.Generic;

namespace TheRandomizer.Generators.UnitTests
{
    [TestClass]
    public class BaseGeneratorTests
    {
        [TestMethod]
        public void SerializationTest()
        {
            var generator = new DiceGenerator();
            generator.Name = "Test";
            generator.Description = "Test Assignment Generator";
            generator.Author = "John Doe";
            generator.Url = "http://www.google.gom";
            generator.SupportsMaxLength = false;
            generator.Tags.Add("Test");
            var xml = generator.Serialize();
            Debug.WriteLine(xml);
        }

        [TestMethod]
        public void CustomCalculationTest()
        {
            var expressions = new Dictionary<string, string> () {{"[=CInt('1')+1]", "2"},
                                                                 {"[=CLng('2')+2]" , "4"},
                                                                 {"[=if(CBool('True'),'Yes','No')]", "Yes"},
                                                                 {"[=CDbl('2.2')-1]", "1.2"}};
            var generator = new AssignmentGenerator();
            var item = new LineItem();
            var result = string.Empty;
            generator.Name = "CalculatorTest";
            item.Name = "Start";
            generator.Items.Add(item);
            foreach (var expression in expressions)
            {
                item.Expression = expression.Key;
                result = generator.Generate();
                Debug.WriteLine(expression.Key + " = " + expression.Value);
                Assert.AreEqual(expression.Value, result);
            }
            
            item.Expression = "[=Rnd(1,10)]";
            result = generator.Generate();
            Debug.WriteLine("[=Rnd(10,10)] = " + result);
            if (Int32.Parse(result) < 1 || Int32.Parse(result) > 10)
            {
                Assert.Fail();
            }

            item.Expression = "[=Roll(6)]";
            result = generator.Generate();
            Debug.WriteLine("[=Roll(6)] = " + result);
            if (Int32.Parse(result) < 1 || Int32.Parse(result) > 6)
            {
                Assert.Fail();
            }

            item.Expression = "[=Roll(2,6)]";
            result = generator.Generate();
            Debug.WriteLine("[=Roll(2,6)] = " + result);
            if (Int32.Parse(result) < 2 || Int32.Parse(result) > 12)
            {
                Assert.Fail();
            }

            item.Expression = "[=Roll(2,6,-1)]";
            result = generator.Generate();
            Debug.WriteLine($"[=Roll(2,6,-1)] = {result}");
            if (Int32.Parse(result) < 1 || Int32.Parse(result) > 11)
            {
                Assert.Fail();
            }

            item.Expression = "[=Roll(5,6,0,'DL',2)]";
            result = generator.Generate();
            Debug.WriteLine($"[=Roll(5,6,0,'DL',2)] = {result}");
            if (Int32.Parse(result) < 3 || Int32.Parse(result) > 18)
            {
                Assert.Fail();
            }

            item.Expression = "[=LastRoll('ResultList')]";
            result = generator.Generate();
            Debug.WriteLine($"[=LastRoll('ResultList')] = {result}");

            item.Expression = "[=Roll(5,10,0,'GT',6,'R1',0)]";
            result = generator.Generate();
            Debug.WriteLine($"[=Roll(5,10,0,'GT',6)] = {result}");

            item.Expression = "[=LastRoll('ResultList')]";
            result = generator.Generate();
            Debug.WriteLine($"{item.Expression} = {result}");

            item.Expression = "Successes : [=LastRoll('Successes')], Failures : [=LastRoll('Failures')], Botches : [=LastRoll('Botches')]";
            result = generator.Generate();
            Debug.WriteLine($"{item.Expression} = {result}");

            item.Expression = "[=Pick('Hello World','Hello Earth')]";
            result = generator.Generate();
            Debug.WriteLine($"{item.Expression} = {result}");
            if (result != "Hello World" && result != "Hello Earth")
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OldAssignmentTest()
        {
            var xml = Properties.Resources.ADnD_Dwarf_Names_rnd;
            Execute(xml);
        }

        [TestMethod]
        public void OldTableTest()
        {
            var xml = Properties.Resources.TownGeneration_rnd;
            Execute(xml);
        }

        [TestMethod]
        public void OldPhonotacticsTest()
        {
            var xml = Properties.Resources.phonotactics_example_rnd;
            Execute(xml);
        }

        [TestMethod]
        public void OldDiceTest()
        {
            var xml = Properties.Resources.WorldOfDarknessCheck_rnd;
            var generator = BaseGenerator.Deserialize(xml);
            generator.Parameters["Version"].Value = "OWOD";
            generator.Parameters["Pool"].Value = "5";
            generator.Parameters["Diff"].Value = "6";
            var result = generator.Generate();
            Debug.WriteLine(result);
        }

        [TestMethod]
        public void GeneratorInfoTest()
        {
            var xml = Properties.Resources.TownGeneration_rnd;
            var generator = GeneratorInfo.Deserialize(xml);
        }

        private void Execute(string xml)
        {
            var generator = BaseGenerator.Deserialize(xml);
            var result = generator.Generate();
            Debug.WriteLine(result);
        }
    }
}
