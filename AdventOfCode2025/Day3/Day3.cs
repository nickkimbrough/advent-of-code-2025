using System.Text;

namespace AdventOfCode2025.Day3;

public class Day3
{
    public static UInt16 Day3Part1Answer()
    {
        /** Initial thoughts:
         * Input is 100 characters long. If we brute force and try every possible joltage....
         * that would be Binomial(100, 2) combinations per line (4950).
         * 200 lines means 990,000 permutations. Brute force is very feasible.
         * 
         * Since 99 is the maximum value, the maximum sum is 19,800. Uint16 is the ideal return type here.
         * 
         * My initial thought is to do an algorithm that selects the first number, then every other one after it.
         * Then it will select the second number, and every one after it, until every joltage has been selected.
         * I'll put them in an efficient distinct collection and just grab the max value from it.
         * 
         * I suspect 990,000 loops won't take long with these simple operations, but I fear part 2.
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day3", "input.txt");
        string[] banks = File.ReadAllLines(filePath);

        UInt16 totalOutputJoltage = 0;
        foreach (string bank in banks)
        {
            HashSet<byte> possibleJoltages = [];
            for (byte i = 0; i < bank.Length - 1; i++)
            {
                for (byte j = (byte)(i + 1); j < bank.Length; j++)
                {
                    byte joltage = byte.Parse($"{bank[i]}{bank[j]}");
                    possibleJoltages.Add(joltage);
                }
            }
            totalOutputJoltage += possibleJoltages.Max();
        }
        return totalOutputJoltage;

        /** Final Thoughts
         * This worked pretty well, I didn't run into any issues really, just cleaning up an off
         * by one error in my inner for loop. It operates very quickly even on the real input so
         * I'm not terrified of part 2 in terms of complexity yet.
         */
    }

    public static ulong Day3Part2Answer()
    {
        /** Initial Thoughts
         * Oh great we went from 2 to 12, that's going to completely blow up the complexity.
         * 
         * Now it's Binomial(100,12) or 1,050,421,051,106,700 combinations. Times 200 that becomes
         * 2.1x10^17.
         * My previous solution is completely blown up, and now I'm having to investigate
         * the possibility of something more elegant or some shortcuts. I imagine there's
         * a math trick.
         * 
         * This field is apparently called combinatorics: https://en.wikipedia.org/wiki/Combinatorics
         * 
         * Reading about integer partitions https://en.wikipedia.org/wiki/Integer_partition gave me
         * a thought: could I start with the maximum answer and work backwards from that? I could
         * plug in the maximum answer, calculate it's partitions that have twelve parts, and see
         * if that set is present. Then I realized we're not doing summation, its' more like a product
         * because every digit is gonna be 10x more than the one to the right of it. So this won't work.
         * 
         * Thinking about the real value of the numbers like this has me thinking maybe I could run a transform
         * on the numbers. So if there are 100 numbers, number 1-88 would have a value of 11 zeroes to the rigt
         * added, and then tapering down until the last digit is only worth it's actual value. Then, you'd select
         * the largest leftmost number, transform the numbers by 0.1, and repeat until you've grabbed the largest
         * 12. This feels ALMOST like a good algorithm but I think it needs more smarts...... I'm
         * missing something here and it's because I don't have a strong math background.
         * 
         * Looking at a simple example: 811111111111119
         * Got me thinking of this: let i = 11 and j = 9..1. Is there a j with i places to the right of it?
         * That's the most valuable number in the first digit, proceed to the next digit. This is a simple
         * algorithm that I think would work, I'm gonna try it!
         * 
         * Maximum output joltage: 199,999,999,999,800 - Uint64
         * Maximum bank joltage: 999,999,999,999 - Uint64
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day3", "input.txt");
        string[] banks = File.ReadAllLines(filePath);

        ulong totalOutputJoltage = 0;
        foreach (string bank in banks)
        {
            StringBuilder outputJoltage = new();
            string remainingBank = bank;
            for (byte i = 12; i >= 1; i--)
            {
                char bestBattery = remainingBank[..^(i-1)].Max();
                outputJoltage.Append(bestBattery);
                remainingBank = remainingBank[(remainingBank.IndexOf(bestBattery)+1)..];
            }
            totalOutputJoltage += ulong.Parse(outputJoltage.ToString());
        }
        return totalOutputJoltage;

        /** Final Thoughts
         * 
         * Turns out simplifying it with that algorithm was the answer! I could make this a function to handle part 1 easily,
         * or any other binomial combination. The key thought that lead to this discovery was that
         * 9 followed by 11 1's is still greater than 8 followed by 11 9's. Now that I've seen this,
         * it's obvious!
         */
    }
}
