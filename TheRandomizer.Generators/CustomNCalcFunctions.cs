using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc;
using System.ComponentModel;
using System.Reflection;
using TheRandomizer.Utility;
using System.Globalization;

namespace TheRandomizer.Generators
{
    /// <summary>
    /// Creates a list of custom functions used by the base grammar when 
    /// evaluating NCalc expressions
    /// </summary>
    [DefaultProperty("CustomFunctionDictionary")]
    internal sealed class CustomNCalcFunctions
    {
        private static Dictionary<string, Action<FunctionArgs>> _dictionary;
        private static Random Random { get; set; }
        private static Dice.DiceRoll _lastRoll;

        public static Dictionary<string, Action<FunctionArgs>> Functions
        {
            get
            {
                if (_dictionary == null)
                {
                    _dictionary = new Dictionary<string, Action<FunctionArgs>>();
                    foreach (var method in typeof(CustomNCalcFunctions).GetMethods())
                    {
                        if (method.GetCustomAttribute(typeof(CustomNCalcFunctionAttribute), false) != null)
                        {
                            var action = (Action<FunctionArgs>)Delegate.CreateDelegate(typeof(Action<FunctionArgs>), method);
                            _dictionary.Add(method.Name, action);
                        }
                    }
                }
                return _dictionary;
            }
        }
                
        /// <summary>
        /// Converts a value to a Boolean
        /// </summary>
        /// <param name="e">One parameter to convert to a boolean required</param>
        [CustomNCalcFunction]
        public static void CBool(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var value = e.EvaluateParameters()[0];
                e.Result = Convert.ChangeType(value, typeof(bool));
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Converts a value to a Double
        /// </summary>
        /// <param name="e">One parameter to convert to a double required</param>
        [CustomNCalcFunction]
        public static void CDbl(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var value = e.EvaluateParameters()[0];
                e.Result = Convert.ChangeType(value, typeof(double));
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Converts a value to an Int32
        /// </summary>
        /// <param name="e">One parameter to convert to an integer required</param>
        [CustomNCalcFunction]
        public static void CInt(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var value = e.EvaluateParameters()[0];
                e.Result = Convert.ChangeType(value, typeof(Int32));
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Converts a value to an Int64
        /// </summary>
        /// <param name="e">One parameter to convert to a long required</param>
        [CustomNCalcFunction]
        public static void CLng(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var value = e.EvaluateParameters()[0];
                e.Result = Convert.ChangeType(value, typeof(Int64));
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Provides a random number
        /// </summary>
        /// <param name="e">Accepts from 0 to 2 parameters
        /// Rnd() : Returns a non-negative random integer
        /// Rnd(max) : Returns a non-negative random integer that is less than the specified maximum
        /// Rnd(min, max) : Returns a random integer that is within a specified range
        /// </param>
        [CustomNCalcFunction]
        public static void Rnd(FunctionArgs e)
        {
            var parameters = e.EvaluateParameters();
            if (Random == null) { Random = new Random(); }
            switch (parameters.Count())
            {
                case 0: e.Result = Random.Next(); break;
                case 1: e.Result = Random.Next((Int32)parameters[0] + 1); break;
                case 2: e.Result = Random.Next((Int32)parameters[0], (Int32)parameters[1] + 1); break;
                default: throw new EvaluationException($"{Common.GetCaller()} requires one, two, or three parameters, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Rolls dice
        /// </summary>
        /// <param name="e">Requires at least one parameter:
        /// Roll(sides) : Rolls one die with the number of sides provided
        /// Roll(dice, sides) : Rolls a number of dice equal to dice having sides
        /// Roll(dice, sides, modifier) : Rolls a number of dice equal to dice having sides adding the modifier to the result(s)
        /// Roll(dice, sides, modifier, options[]) : Rolls a number of dice equal to dice having sides adding the modifier to the result(s) using the provided options
        /// </param>
        [CustomNCalcFunction]
        public static void Roll(FunctionArgs e)
        {
            var parameters = e.EvaluateParameters();
            Dice.DiceRoll roller;
            switch  (parameters.Count())
            {
                case 1: roller = new Dice.DiceRoll(Convert.ToInt32(parameters[0])); break;
                case 2: roller = new Dice.DiceRoll(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1])); break;
                case 3: roller = new Dice.DiceRoll(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2])); break;
                default:
                    var options = Array.ConvertAll(parameters.Skip(3).ToArray(), i => i.ToString());
                    roller = new Dice.DiceRoll(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), options);
                    break;
            }
            if (roller != null)
            {
                roller.Roll();
                e.Result = roller.Result;
                _lastRoll = roller;
            }
            
        }

        /// <summary>
        /// Returns details about the last dice roll performed using <see cref="Roll"/>.
        /// </summary>
        /// <param name="e">Requires zero or one parameters:
        /// LastRoll() : Returns the Result of the last roll
        /// LastRoll(Property) : Returns the value from the given property; Result, ResultList, Successes, Failures, Botches</param>
        [CustomNCalcFunction]
        public static void LastRoll(FunctionArgs e)
        {
            if (_lastRoll != null)
            {
                if (e.Parameters.Count() == 0)
                {
                    e.Result = _lastRoll.Result;
                }
                else if (e.Parameters.Count() == 1)
                {
                    var parameters = e.EvaluateParameters();
                    switch (parameters[0].ToString())
                    {
                        case "Result": e.Result = _lastRoll.Result; break;
                        case "ResultList": e.Result = String.Join(", ", _lastRoll.ResultList.ConvertAll(i => i.ToString())); break;
                        case "Successes": e.Result = _lastRoll.Successes; break;
                        case "Failures": e.Result = _lastRoll.Failures; break;
                        case "Botches": e.Result = _lastRoll.Botches; break;
                        default: throw new EvaluationException($"Unrecognized parameter for {Common.GetCaller()} function: {parameters[0].ToString()}.");
                    }
                }
                else
                {
                    throw new EvaluationException($"{Common.GetCaller()} requires zero or one parameter, {e.Parameters.Count()} provided.");
                }
            }
        }

        /// <summary>
        /// Selects a single item from the list provided
        /// </summary>
        /// <param name="e">Requires at least one parameter:
        /// Pick (ItemsToChooseFrom[])</param>
        [CustomNCalcFunction]
        public static void Pick(FunctionArgs e)
        {
            if (e.Parameters.Count() > 0)
            {
                var parameters = e.EvaluateParameters();
                var index = Random.Next(0, parameters.Count());
                e.Result = parameters[index];
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires at least one parameter.");
            }
        }

        /// <summary>
        /// Converts a string to UPPER CASE
        /// </summary>
        /// <param name="e">Requires exactly one parameter:
        /// UCase(string)</param>
        [CustomNCalcFunction]
        public static void UCase(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var parameters = e.EvaluateParameters();
                e.Result = CultureInfo.CurrentCulture.TextInfo.ToUpper(parameters[0].ToString());
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Converts a string to lower case
        /// </summary>
        /// <param name="e">Requires exactly one parameter:
        /// LCase(string)</param>
        [CustomNCalcFunction]
        public static void LCase(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var parameters = e.EvaluateParameters();
                e.Result = CultureInfo.CurrentCulture.TextInfo.ToLower(parameters[0].ToString());
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Converts a string to Title Case
        /// </summary>
        /// <param name="e">Requires exactly one parameter:
        /// TCase(string)</param>
        [CustomNCalcFunction]
        public static void TCase(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var parameters = e.EvaluateParameters();
                e.Result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parameters[0].ToString());
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Converts an integer to an ordinal number such as 1st, 2nd, or 3rd
        /// </summary>
        /// <param name="e">Requires exactly one parameter:
        /// ToOrdinal(number)</param>
        [CustomNCalcFunction]
        public static void ToOrdinal(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var parameters = e.EvaluateParameters();
                var value = Convert.ChangeType(parameters[0], typeof(Int32));
                e.Result = ((Int32)value).ToOrdinal();
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Converts an integer to a string such as One, Two Hundred, or Thirty
        /// </summary>
        /// <param name="e">Requires exactly one parameter:
        /// ToText(number)</param>
        [CustomNCalcFunction]
        public static void ToText(FunctionArgs e)
        {
            if (e.Parameters.Count() == 1)
            {
                var parameters = e.EvaluateParameters();
                e.Result = ((Int32)parameters[0]).ToText();
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires exactly one parameter, {e.Parameters.Count()} provided.");
            }
        }

        /// <summary>
        /// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array. 
        /// </summary>
        /// <param name="e">Requires at least one parameter:
        /// Format(format, args[])</param>
        [CustomNCalcFunction]
        public static void Format(FunctionArgs e)
        {
            if (e.Parameters.Count() > 1)
            {
                var parameters = e.EvaluateParameters();
                var arguments = parameters.Skip(1).ToArray();
                e.Result = String.Format(parameters[0].ToString(), arguments);
            }
            else
            {
                throw new EvaluationException($"{Common.GetCaller()} requires at least one parameter, zero provided.");
            }
        }

    }
}
