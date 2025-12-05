

using System.Runtime.CompilerServices;

namespace AdventOfCode2025.Day5;

public class Day5
{
    public static ulong Day5Part1Answer()
    {
        /** Initial Thoughts:
         *  This seems suspiciously easy. Has Advent of Code gone soft?
         *  
         *  I'll pull the file into a string array for each line, splitting
         *  it into two arrays on the blank line.
         *  
         *  I'll loop over the ingredient ID's, creating a range by splitting
         *  on the dash. I'll add all of these to a HashSet<int>
         *  (unless ID's are massive outside of the sample).
         *  
         *  I'll pull all of the ingredient id's in the second array into a
         *  HashSet<int> as well, and then do a union to get the overlap,
         *  finding the matching ID's, then using .Count.
         *  
         *  Looking at inputs, seems like the ID's are 15 digits, i'll need
         *  at least a 64 bit int. I might be able to add the ranges to the HashSet
         *  directly.
         */

        /** Roadblocks
         * 
         *  Well, 15 digits is a problem. The first ID range holds 191 billion elements.
         *  There are 174 ranges. If 191 billion is even an average it'll be 33 trillion
         *  total elements. A ulong takes up 8 bytes so that would be something ridiculous
         *  like 264 terabytes of data in memory! I need a new method.
         *  
         *  New Method:
         *  
         *  There are 1000 available ingredients. I'm thinking of building a comparison
         *  algorithm that will loop over every range and check to see if it's >= to the
         *  first element or <= the last element. If it is, it'll return true and short
         *  circuit. Otherwise it'll return false. With 174 ranges, this is a maximum
         *  of 1000 X 174 X 2 (2 comparisons per range) = 348k simple comparison operations.
         *  
         *  I think this is doable.
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day5", "input.txt");
        string[] input = File.ReadAllLines(filePath);
        IReadOnlyList<ulong[]> freshIngredients = [.. input
            .Take(input.IndexOf(""))
            .Select(x => new ulong[] { ulong.Parse(x.Split('-')[0]), ulong.Parse(x.Split('-')[1]) })];
        var availableIngredients = input.Skip(input.IndexOf("") + 1).Select(ulong.Parse).ToList();

        ulong freshAvailableIngredientCount = 0;

        foreach (ulong ingredientId in availableIngredients)
        {
            bool ingredientIsFresh = false;
            for (int j = 0; j < freshIngredients.Count; j++)
            {
                if (ingredientId >= freshIngredients[j][0] && ingredientId <= freshIngredients[j][1])
                {
                    ingredientIsFresh = true;
                }
            }
            freshAvailableIngredientCount += ingredientIsFresh ? 1UL : 0UL;
        }


        return freshAvailableIngredientCount;

        /** Final Thoughts:
         * 
         *  Doing 2 comparisons was snappy, but this feels brittle to part 2..... let's see.
         */
    }
    public static ulong Day5Part2Answer()
    {
        /** Initial Thoughts:
         * 
         *  AoC seems to have gone soft unless I'm missing something. This seems trivial.
         *  
         *  Let the bottom of the range = i, top of the range = j
         *  
         *  I'll just take what I already had and subtract i from j and
         *  add 1 to get the range..... oooooh I see, overlapping ranges.
         *  
         *  I'll need to combine all overlapping ranges first and then do the trivial thing.
         *  I'll use the example ranges 10-14 and 12-18. I guess you'd do something like....
         *  
         *  until no merges happen, loop over every range, comparing to every other range. If
         *  i in the target range is in the range of the source range, either:
         *   - delete it if target j isn't larger than source j
         *   - delete and modify source range to have target j
         *   
         *  I'll need to makes sure I'm using something mutable.         *  
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day5", "input.txt");
        string[] input = File.ReadAllLines(filePath);
        List<ulong[]> freshIngredients = [.. input
            .Take(input.IndexOf(""))
            .Select(x => new ulong[] { ulong.Parse(x.Split('-')[0]), ulong.Parse(x.Split('-')[1]) })];

        // Merge overlapping ranges
        bool rangeMerged = false;
        do
        {
            rangeMerged = false;
            for (int i = 0; i < freshIngredients.Count; i++)
            {
                for (int j = 0; j < freshIngredients.Count; j++)
                {
                    if (i == j) { continue; } // Don't compare to self

                    if (freshIngredients[j][0] >= freshIngredients[i][0] && freshIngredients[j][0] <= freshIngredients[i][1])
                    {
                        // we have an overlap
                        if (freshIngredients[j][1] <= freshIngredients[i][1])
                        {
                            // Range is entirely within the source range
                            freshIngredients.RemoveAt(j);
                        }
                        else
                        {
                            // target range expands source range
                            freshIngredients.Add([freshIngredients[i][0], freshIngredients[j][1]]);

                            int secondIndex = i < j ? j - 1 : j;

                            freshIngredients.RemoveAt(i);
                            freshIngredients.RemoveAt(secondIndex);
                        }
                        rangeMerged = true;
                        goto ContinueDo;
                    }
                }
            }
        ContinueDo:;
        }
        while (rangeMerged);

        ulong totalFreshIngredients = 0;

        foreach (ulong[] ingredientRange in freshIngredients)
        {
            totalFreshIngredients += ingredientRange[1] - ingredientRange[0] + 1;
        }

        return totalFreshIngredients;

        /** Final thoughts 
         * 
         * Ok, AoC didn't lose it's touch. That was an interesting conundrum.
         * I think I've had to deal with something similar before.
         * 
         * I had Gemini grade my answer to get some insights into improvements.
         * In part 1 I should have broke to short circuit once I found a match.
         * 
         * In part 2, I should have used a sort and merge to avoid having to
         * use my goto statement. That did feel nasty when I was writing it and
         * I tend to agree.
         */
    }
}
