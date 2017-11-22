using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Dice
{
    internal class DiceRoll
    {

        #region Constants
        private static readonly Dictionary<string, Func<DiceRollOption>> OPTION_LIST =
            new Dictionary<string, Func<DiceRollOption>>()
            {
                { "DL", () => new DiceRollOption() { Option = DiceRollOptions.DropLowest, HasVariable = true, DefaultValue = 1 } },
                { "DH", () => new DiceRollOption() { Option = DiceRollOptions.DropHighest, DefaultValue = 1 } },
                { "EX", () => new DiceRollOption() { Option = DiceRollOptions.Explode, HasVariable = false } },
                { "CX", () => new DiceRollOption() { Option = DiceRollOptions.CompoundExplode, HasVariable = false } },
                { "RB", () => new DiceRollOption() { Option = DiceRollOptions.RerollBelow, VariableIsRequired = true } },
                { "RA", () => new DiceRollOption() { Option = DiceRollOptions.RerollAbove, VariableIsRequired = true } },
                { "GT", () => new DiceRollOption() { Option = DiceRollOptions.GreaterThan, VariableIsRequired = true } },
                { "LT", () => new DiceRollOption() { Option = DiceRollOptions.LessThan, VariableIsRequired = true } },
                { "R1", () => new DiceRollOption() { Option = DiceRollOptions.RuleOfOne, HasVariable = false } }
            };
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DiceRoll class 
        /// </summary>
        public DiceRoll() { }
        /// <summary>
        /// Initializes a new instance of the DiceRoll class for a single die
        /// </summary>
        /// <param name="sides">The number of sides the die contains</param>

        public DiceRoll(Int32 sides) : this(1, sides, 0, new string[] { } ) { }
        /// <summary>
        /// Initializes a new instance of the DiceRoll class  for the number of dice specified
        /// </summary>
        /// <param name="dice">The number of dice to roll</param>
        /// <param name="sides">The number of sides the die contains</param>

        public DiceRoll(Int32 dice, Int32 sides) : this(dice, sides, 0, new string[] { }) { }
        /// <summary>
        /// Initializes a new instance of the DiceRoll class  for the number of dice specified
        /// </summary>
        /// <param name="dice">The number of dice to roll</param>
        /// <param name="sides">The number of sides the die contains</param>
        /// <param name="modifier">A modifier to add to the roll</param>

        public DiceRoll(Int32 dice, Int32 sides, Int32 modifier) : this(dice, sides, modifier, new string[] { }) { }
        /// <summary>
        /// Initializes a new instance of the DiceRoll class  for the number of dice specified
        /// </summary>
        /// <param name="dice">The number of dice to roll</param>
        /// <param name="sides">The number of sides the die contains</param>
        /// <param name="modifier">A modifier to add to the roll</param>
        /// <param name="options">A list of options for the roll.  Should be in the format:
        /// "OptionCode","Variable (if applicable)" ex: {"DL","1","EX","RB","2"}</param>

        public DiceRoll(Int32 dice, Int32 sides, Int32 modifier, params string[] options)
        {
            var optionList = new List<DiceRollOption>();
            var index = 0;
            while (index < options.Count())
            {
                if (OPTION_LIST.ContainsKey(options[index]))
                {
                    var newOption = OPTION_LIST[options[index]].Invoke();
                    index++;
                    if (newOption.HasVariable)
                    {
                        int value;
                        if (index >= options.Count() || !int.TryParse(options[index], out value))
                        {
                            if (newOption.VariableIsRequired)
                            {
                                throw new ArgumentException($"DiceRoll Option {newOption.Option} requires a variable.");
                            }
                            else
                            {
                                value = (int)newOption.DefaultValue;
                            }
                        }
                        
                        newOption.Variable = value;
                        index++;
                    }
                    optionList.Add(newOption);
                }
            }
            Initialize(dice, sides, modifier, optionList.ToArray());

        }
        /// <summary>
        /// Initializes a new instance of the DiceRoll class  for the number of dice specified
        /// </summary>
        /// <param name="dice">The number of dice to roll</param>
        /// <param name="sides">The number of sides the die contains</param>
        /// <param name="modifier">A modifier to add to the roll</param>
        /// <param name="options">A list of options for the roll.</param>
        public DiceRoll(Int32 dice, Int32 sides, Int32 modifier, params DiceRollOption[] options)
        {
            Initialize(dice, sides, modifier, options);   
        }
        #endregion

        #region Members
        private static Random _random;
        #endregion

        #region Public Properties
        /// <summary>Number of dice to roll</summary>
        public Int32 Dice { get; set; } = 1;
        /// <summary>How many sides on the dice</summary>
        public Int32 Sides { get; set; }
        /// <summary>Modifier to the roll</summary>
        public Int32 Modifier { get; set; } = 0;
        /// <summary>Target number for target rolls</summary>
        public Int32 Target { get; set; } = 0;
        /// <summary>Method to use for exploding dice</summary>
        public ExplodeMethod Explode { get; set; } = ExplodeMethod.None;
        /// <summary>For target rolls, use the rule of one</summary>
        public bool RuleOfOne { get; set; } = false;
        /// <summary>Drop 'n' lowest rolls</summary>
        public Int32 DropLowest { get; set; } = 0;
        /// <summary>Drop 'n' highest rolls</summary>
        public Int32 DropHighest { get; set; } = 0;
        /// <summary>Target type for targeted rolls</summary>
        public TargetType TargetType { get; set; } = TargetType.None;
        /// <summary>Reroll any result over this value</summary>
        public Int32 RerollAbove { get; set; } = 0;
        /// <summary>Reroll any result below this value</summary>
        public Int32 RerollBelow { get; set; } = 0;
        /// <summary>The result of the roll</summary>
        public dynamic Result { get; private set; }
        /// <summary>A list of the individual die rolls</summary>
        public List<Int32> ResultList { get; private set; } = new List<Int32>();
        /// <summary>For target rolls, the number of successes</summary>
        public Int32 Successes { get; set; }
        /// <summary>For target rolls, the number of failures</summary>
        public Int32 Failures { get; set; }
        /// <summary>For target rolls with rule of one, how many botches</summary>
        public Int32 Botches { get; set; }
        #endregion

        #region Protected Properties
        /// <summary>
        /// A random number generator.  This is initialized only once to keep it as random as possible.
        /// </summary>
        protected static Random Random
        {
            get
            {
                if (_random == null) { _random = new Random(); }
                return _random;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Roll the dice
        /// </summary>
        public void Roll()
        {
            for (var i = 1; i <= Dice; i++)
            {
                var roll = InternalRoll();

                // Handle rerolls
                if (RerollBelow != 0 && i < RerollBelow)
                {
                    do
                    {
                        roll = InternalRoll();
                    } while (roll < RerollBelow);
                }
                else if (RerollAbove != 0 && i > RerollAbove)
                {
                    do
                    {
                        roll = InternalRoll();
                    } while (roll > RerollAbove);
                }

                // Handle exploding dice
                if (roll == Sides)
                {
                    switch (Explode)
                    {
                        case ExplodeMethod.Simple: roll += InternalRoll(); break;
                        case ExplodeMethod.Compound:
                            var lastRoll = roll;
                            do
                            {
                                lastRoll = InternalRoll();
                                roll += lastRoll;
                            } while (lastRoll == Sides);
                            break;
                    }
                }

                ResultList.Add(roll);

                //  Handle target rolls
                switch (TargetType)
                {
                    case TargetType.GreaterThan:
                        if (RuleOfOne && roll == 1)
                        {
                            Botches++;
                        }
                        else if (roll + Modifier >= Target)
                        {
                            Successes++;
                        }
                        else
                        {
                            Failures++;
                        }
                        break;
                    case TargetType.LessThan:
                        if (roll + Modifier <= Target)
                        {
                            Successes++;
                        }
                        else
                        {
                            Failures++;
                        }
                        break;
                }
            }
           
            if (TargetType == TargetType.None)
            {
                Result = ResultList.Sum() + Modifier;
                if (DropHighest > 0)
                {
                    if (DropHighest >= ResultList.Count())
                    {
                        Result = 0;
                    }
                    else
                    {
                        var tempList = ResultList.OrderByDescending(i => i).ToList();
                        for (var i = 0; i < DropHighest; i++)
                        {
                            Result -= tempList[i];                     
                        }
                    }
                }
                if (DropLowest > 0)
                {
                    if (DropLowest >= ResultList.Count())
                    {
                        Result = 0;
                    }
                    else
                    {
                        var tempList = ResultList.OrderBy(i => i).ToList();
                        for (var i = 0; i < DropLowest; i++)
                        {
                            Result -= tempList[i];
                        }
                    }
                }
            }
            else
            {
                if ((Successes - Botches) > 0)
                {
                    Result = true;
                }
                else
                {
                    Result = false;
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Genertes the random numnber using the dice properties
        /// </summary>
        private Int32 InternalRoll()
        {
            return Random.Next(1, Sides + 1);
        }

        /// <summary>
        /// Initializes the properties and options of the Dice Roll
        /// </summary>
        /// <param name="dice">The number of dice to roll</param>
        /// <param name="sides">The number of sides the dice have</param>
        /// <param name="modifier">The modifier to the roll</param>
        /// <param name="options">Optional parameters for the dice roll</param>
        public void Initialize(Int32 dice, Int32 sides, Int32 modifier, params DiceRollOption[] options)
        {
            Dice = dice;
            Sides = sides;
            Modifier = modifier;
            if (options != null)
            {
                foreach (var option in options)
                {
                    switch (option.Option)
                    {
                        case DiceRollOptions.CompoundExplode: Explode = ExplodeMethod.Compound; break;
                        case DiceRollOptions.Explode: Explode = ExplodeMethod.Simple; break;
                        case DiceRollOptions.DropHighest: DropHighest = option.Variable; break;
                        case DiceRollOptions.DropLowest: DropLowest = option.Variable; break;
                        case DiceRollOptions.RerollAbove: RerollAbove = option.Variable; break;
                        case DiceRollOptions.RerollBelow: RerollBelow = option.Variable; break;
                        case DiceRollOptions.RuleOfOne: RuleOfOne = true; break;
                        case DiceRollOptions.GreaterThan:
                            TargetType = TargetType.GreaterThan;
                            Target = option.Variable;
                            break;
                        case DiceRollOptions.LessThan:
                            TargetType = TargetType.LessThan;
                            Target = option.Variable;
                            break;
                    }
                }
            }
        }
        #endregion
    }
}

