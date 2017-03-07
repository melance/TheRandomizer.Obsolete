using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheRandomizer.Generators.Phonotactics;

namespace TheRandomizer.Generators.Parameter.UnitTests
{
    [TestClass]
    public class PhonotacticsGeneratorTests
    {
        [TestMethod]
        public void BasicTest()
        {
            var generator = new PhonotacticsGenerator();
            generator.Definitions.Add(new Definition() { Key = "H", Value = "Hello" });
            generator.Definitions.Add(new Definition() { Key = "W", Value = "World,Earth" });
            generator.Patterns.Add(new Pattern() { Value = "H W" });
            var results = generator.Generate();
            Debug.WriteLine(results);
            if (results != "Hello World" && results != "Hello Earth")
            {
                Assert.Fail();
            }
        }
    }
}
