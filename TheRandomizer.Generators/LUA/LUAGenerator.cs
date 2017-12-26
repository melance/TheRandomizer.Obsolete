using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NLua;
using TheRandomizer.Generators.Attributes;
using NLua.Exceptions;

namespace TheRandomizer.Generators.Lua
{
    [XmlType("Lua")]
    [GeneratorDisplay(Generators.GeneratorType.Lua, "A LUA scripting language based generator.")]
    public class LuaGenerator : BaseGenerator
    {
        /// <summary>
        /// The text of the Lua script to run
        /// </summary>
        [XmlElement("script")]
        [LuaHide]
        public string Script { get { return GetProperty<string>(); } set { SetProperty(value); } }

        /// <summary>
        /// The path to a Lua script to run instead of the <see cref="Script"/> 
        /// </summary>
        [XmlElement("scriptPath")]
        [LuaHide]
        public string ScriptPath { get { return GetProperty<string>(); } set { SetProperty(value); } }

        [XmlIgnore]
        [LuaHide]
        public override bool? SupportsMaxLength { get { return null; } set { } }

        private StringBuilder Result { get; } = new StringBuilder();

        private NLua.Lua _lua;

        protected override string GenerateInternal(int? maxLength)
        {
            try
            {
                var scriptText = Script;

                _lua = new NLua.Lua();
                Result.Clear();
                foreach (var parameter in Parameters)
                {
                    _lua[parameter.Name] = parameter.Value;
                }

                // Add all methods tagged with Lua Global
                LuaRegistrationHelper.TaggedInstanceMethods(_lua, this);

                // Prevent imports to sandbox the script
                _lua.DoString("import = function () end");

                // If the script path is set, load that script
                if (!string.IsNullOrWhiteSpace(ScriptPath))
                {
                    scriptText = OnRequestFileText(ScriptPath);
                }

                // Run the script
                _lua.DoString(scriptText, Name);

                // Return the values printed to the Results
                return Result.ToString().Trim();
            }
            catch (LuaScriptException ex)
            {
                throw new Exception($"Error encountered in the Lua script:<br /> {ex.Source}<br /> {ex.Message}");
            }
        }
        
        /// <summary>
        /// Prints a blank line to the Results
        /// </summary>
        [LuaGlobal(Name = "printLine", Description = "Appends a blank line to the output")]
        public void PrintLine()
        {
            Result.AppendLine();
        }

        /// <summary>
        /// Prints a line of text to the Results
        /// </summary>
        /// <param name="value">The text to print</param>
        [LuaGlobal(Name = "printLine", Description = "Appends text to the output followed by a new line")]
        public void PrintLine(object value)
        {
            Result.AppendLine(value.ToString());
        }
        
        /// <summary>
        /// Prints a line of text to the Results if <paramref name="Condition"/> is true;
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <param name="value">The text to print</param>
        [LuaGlobal(Name = "printLineIf", Description = "Appends text to the output followed by a new line if the condition is true")]
        public void PrintLineIf(bool condition, object value)
        {
            if (condition) PrintLine(value);
        }
        
        /// <summary>
        /// Prints a formatted line of text to the Results
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="values">An array of objects to format</param>
        [LuaGlobal(Name = "printLineFormat", Description = "Appends formatted text to the output followed by a new line")]
        public void PrintLineFormat(string format, params object[] values)
        {
            Result.AppendFormat(format, values);
            Result.AppendLine();
        }

        /// <summary>
        /// Prints text to the Results
        /// </summary>
        /// <param name="value">The text to print</param>
        [LuaGlobal(Name = "print", Description = "Appends text to the output")]
        public void Print(object value)
        {
            Result.Append(value.ToString());
        }

        /// <summary>
        /// Prints a text to the Results if <paramref name="Condition"/> is true.
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <param name="value">The text to print</param>
        [LuaGlobal(Name = "printIf", Description = "Appends text to the output if the condition is true")]
        public void PrintIf(bool condition, object value)
        {
            if (condition) Print(value);
        }

        /// <summary>
        /// Prints a formatted line of text to the Results
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="values">An array of objects to format</param>
        [LuaGlobal(Name = "printFormat", Description = "Appends formatted text to the output")]
        public void PrintFormat(string format, params object[] values)
        {
            Result.AppendFormat(format, values);
        }
        /// <summary>
        /// Creates an empty LuaTable
        /// </summary>

        [LuaGlobal(Name = "createTable", Description = "Creates a table")]
        public LuaTable CreateTable()
        {
            return (LuaTable)_lua.DoString("return {}")[0];
        }

        /// <summary>
        /// Calls the NCalc engine to evaluate the provided expression
        /// </summary>
        /// <param name="expression">A string expression to evaluate</param>
        [LuaGlobal(Name = "calc", Description = "Runs the expression through the NCalc engine.")]
        public string NCalc(string expression)
        {
            return Calculate(expression);
        }

        [LuaGlobal(Name = "rnd")]
        public int GetRandomNumber()
        {
            return Random.Next();
        }

        [LuaGlobal(Name = "rnd")]
        public int GetRandomNumber(int maxValue)
        {
            return Random.Next(maxValue);
        }

        [LuaGlobal(Name = "rnd")]
        public int GetRandomNumber(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Selects one item from the provided table
        /// </summary>
        /// <param name="table">The table to select from, must have an integer index</param>
        [LuaGlobal(Name = "selectFromTable", Description = "Selects a single item from the provided table.")]
        public string SelectFromTable(LuaTable table)
        {
            var selected = Random.Next(1, table.Keys.Count);
            foreach (KeyValuePair<object,object> value in table)
            {
                var index = value.Key as double?;
                if (index != null && selected <= index)
                {
                    return value.Value.ToString();
                }                
            }
            return string.Empty;
        }
    }
}
