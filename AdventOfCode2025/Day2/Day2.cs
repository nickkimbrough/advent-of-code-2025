namespace AdventOfCode2025.Day2;

public class Day2
{
    public UInt128 Day2Part1Answer()
    {
        /** Initial thoughts:
         *  I'll probably grab the input and split it by commas and then hyphens to build my list of ranges.
         *  Then I'll do a for loop over the range. I'll write a method to check for invalid ID's and apply
         *  it to every ID.
         * 
         *  Method to check for invalid ID's:
         *  If it starts with 0 - ignore (maybe some trickery in the input)
         *  Since we're checkign for digits repeated twice, the number is going to have to be an even number
         *  length. So if it's odd, it's valid automatically.
         *  If it's even, split it in two and check it. If they match, it's invalid.
         * 
         *  This seems very easy for part 1, but I'm afraid of what part 2 will bring. I'm hoping that some
         *  of my logic shortcuts will allow this method to work if the dataset gets substantially larger.
         *  If not, I'll have to think of some more shortcuts or change my approach.
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day2", "input.txt");
        string instructions = File.ReadAllLines(filePath)[0];
        var parsedInstructions = instructions.Split(',');

        UInt128 invalidIdSum = 0;
        foreach (string instruction in parsedInstructions)
        {
            string[] range = instruction.Split("-");
            for (UInt128 id = UInt128.Parse(range[0]); id < UInt128.Parse(range[1]) + 1; id++)
            {
                string idString = id.ToString();
                if (idString.Length % 2 == 0)
                {
                    string firstHalf = idString[..(idString.Length / 2)];
                    string secondHalf = idString.Substring(idString.Length / 2);

                    if (firstHalf == secondHalf)
                    {
                        invalidIdSum += id;
                    }
                }
            }
        }

        return invalidIdSum;

        /** What I Learned
         *  Shoulda guessed the numbers would be huge. Luckily UInt128 was able to handle it. I fear part 2 though.
         *  Otherwise this was mostly standard looping and parsing.
         */
    }
        

    public UInt128 Day2Part2Answer()
    {
        /** Initial Thoughts
         *  They changed the pattern! I suspected something like this. Thankfully it shouldn't override my UInt128.
         *  
         *  To handle this, I'll stop ignoring odd lengthed ID's, and I'll have to change my splitting logic.
         *  
         *  I'll do a loop starting at 2 up to the total length of the ID. I'll first check with modulo for divisibility,
         *  if it's not divisible i'll move on. If it is, I'll build up the array and check for sameness.
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day2", "input.txt");
        string instructions = File.ReadAllLines(filePath)[0];
        var parsedInstructions = instructions.Split(',');

        UInt128 invalidIdSum = 0;
        foreach (string instruction in parsedInstructions)
        {
            string[] range = instruction.Split("-");
            for (UInt128 id = UInt128.Parse(range[0]); id < UInt128.Parse(range[1]) + 1; id++)
            {
                string idString = id.ToString();

                for (int i = 1; i < idString.Length; i++)
                {
                    if (idString.Chunk(i).Select(x => new string(x)).Distinct().Count() == 1)
                    {
                        invalidIdSum += id;
                        break;
                    }
                }
            }
        }

        return invalidIdSum;

        /** What I learned
         *  LINQ has a chunk method! 
         */
    }
}
