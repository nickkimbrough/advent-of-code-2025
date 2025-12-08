

using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode2025.Day6;

public class Day6
{
    public static UInt128 Day6Part1Answer()
    {
        /** Initial Thoughts:
         * Part 1 seems trivial. Probably read the last line first to
         * get the operand for each column, then loop over the rows
         * and build a running total. All of the operands are either
         * addition or multiplication, which are both commutative so
         * we don't have to worry about order of operations.
         * 
         * Looking at the input, there are 1000 problems, each with 4
         * operands, so we're looking at 3 operations per problem,
         * 3000 operations on the sheet, and then 2999 to sum them
         * for a total of 5999 operations. Very doable.         * 
         */

        /** Solution Found:
         *  Ran into my sum being too large for int even within a single
         *  problem. Thankfully UInt128 was big enough to handle it.
         *  Otherwise this went exactly as I planned.
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day6", "input.txt");
        var input = File.ReadAllLines(filePath).ToList();
        var operands = input.Last().Replace(" ", "").ToCharArray().ToList();
        var problems = input.Take(input.Count - 1).Select(x =>
        {
            return x
                .Split(' ')
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(y => UInt128.Parse(y))
                .ToList();
        }).ToList();

        UInt128 sumOfProblems = 0;
        do
        {
            var operand = operands.First();
            operands.RemoveAt(0);

            UInt128 runningTotal = operand == '+' ? UInt128.Zero : UInt128.One;
            for (byte i = 0; i < problems.Count; i++)
            {
                if (operand == '+')
                {
                    runningTotal += problems[i].First();
                } else
                {
                    runningTotal = runningTotal * problems[i].First();
                }
                problems[i].RemoveAt(0);
            }
            sumOfProblems += runningTotal;
        }
        while (problems[0].Count > 0);

        return sumOfProblems;
    }
    public static UInt128 Day6Part2Answer()
    {
        /** Initial Thoughts:
         *  Ok so we're changing how to read the numbers but everything else
         *  stays the same. Gonna have to figure out how to distinguish
         *  between a column separation and a null number placeholder.
         *  
         *  Initial thought is to find a space right before a number with regex,
         *  replace it with a comma, then split on commas. That or maybe
         *  the chunk sizes are constant. Then I can read them as strings and
         *  convert characters to numbers to build my operands.
         *  
         *  A quick glance at input shows me chunk sizes differ, so regex it is!
         */

        /** Stumbling Block:
         *  The regex I had thought of won't work becase some numbers are right
         *  alignted and some are left aligned. Looking at the input again,
         *  I noticed that the operands are at the right location to split
         *  the problems. I'll probably have to use this.
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day6", "input.txt");
        var allLines = File.ReadAllLines(filePath);

        var operands = allLines
            .Last()
            .Replace(" ", "")
            .ToCharArray()
            .Reverse() // cephalopods read right to left
            .ToList();
        var rawOperands = allLines
            .Last();
        var rawOperators = allLines
            .Take(allLines.Length - 1)
            .ToList();

        UInt128 sumOfProblems = 0;
        foreach (char operand in operands)
        {
            var columnIndex = rawOperands.LastIndexOf(operand);
            var chunkSize = rawOperands.Length - columnIndex;
            if (columnIndex > 0)
            {
                rawOperands = rawOperands[..(columnIndex - 1)];
            }

            List<string> parsedOperators = [];
            for (byte i = 0; i < rawOperators.Count; i++)
            {
                parsedOperators.Add(rawOperators[i][columnIndex..]);
                if (columnIndex > 0)
                {
                    rawOperators[i] = rawOperators[i][0..(columnIndex - 1)];
                }
            }

            List<ushort> cephalopodOperators = [];

            for (byte i = 0; i < chunkSize; i++)
            {
                string cephalopodOperatorString = string.Empty;
                for (byte j = 0; j < parsedOperators.Count; j++)
                {
                    var currentOperator = parsedOperators[j].Last();
                    parsedOperators[j] = parsedOperators[j][..(parsedOperators[j].Length - 1)];

                    cephalopodOperatorString += currentOperator == ' ' ? "": currentOperator;    
                }
                if (!string.IsNullOrWhiteSpace(cephalopodOperatorString))
                {
                    cephalopodOperators.Add(ushort.Parse(cephalopodOperatorString));
                }
            }

            UInt128 runningTotal = operand == '+' ? UInt128.Zero : UInt128.One;
            if (operand == '+')
            {
                foreach (ushort cephalopodOperator in cephalopodOperators)
                {
                    runningTotal += cephalopodOperator;
                }
            }
            else
            {
                foreach (ushort cephalopodOperator in cephalopodOperators)
                {
                    runningTotal = runningTotal * cephalopodOperator;
                }
            }
            sumOfProblems += runningTotal;
        }

        return sumOfProblems;
    }
}
