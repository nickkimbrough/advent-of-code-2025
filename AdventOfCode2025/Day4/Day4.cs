using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2025.Day4;

public class Day4
{
    public static ushort Day4Part1Answer()
    {
        /** Initial Thoughts:
         *  A classic 2d array problem based on surrounding values
         *  In the past I've handled these by writing a function
         *  to calculate adjacent values and return if something
         *  is true.
         *  
         *  What scared me with this one is that part 1 seems
         *  quite simple to me, but I'm worried part 2 is going to
         *  add some crazy requirement and blow up my solution.
         *  
         *  I'll try and write this to be able to handle different
         *  parameters and work in different scenarios. 
         *  
         *  The input is 140 characters square, so y'll have to 
         *  calculate 19600 nodes. Is this another brute force trap?
         *  
         *  Every node can have essentially 3 values, no roll, invalid, and valid.
         *  Maybe a bool? would work here.
         */

        // Intake the info, shape it
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day4", "input.txt");
        string[] paperLayout = File.ReadAllLines(filePath);
        byte floorDimension = (byte)paperLayout.Length;
        ushort accessiblePaperRolls = 0;
        for (byte y = 0; y < floorDimension; y++)
        {
            for (byte x = 0; x < floorDimension; x++)
            {
                // Only check the cell if it's a roll
                if (paperLayout[y][x] == '@')
                {
                    // Check adjacent squares
                    bool upSafe = y > 0;
                    bool leftSafe = x > 0;
                    bool downSafe = y < floorDimension - 1;
                    bool rightSafe = x < floorDimension - 1;

                    byte adjacentRolls = 0;
                    adjacentRolls += (upSafe && paperLayout[y - 1][x] == '@') ? (byte)1 : (byte)0;
                    adjacentRolls += (upSafe && rightSafe && paperLayout[y - 1][x + 1] == '@') ? (byte)1 : (byte)0;
                    adjacentRolls += (rightSafe && paperLayout[y][x + 1] == '@') ? (byte)1 : (byte)0;
                    adjacentRolls += (rightSafe && downSafe && paperLayout[y + 1][x + 1] == '@') ? (byte)1 : (byte)0;
                    adjacentRolls += (downSafe && paperLayout[y + 1][x] == '@') ? (byte)1 : (byte)0;
                    adjacentRolls += (downSafe && leftSafe && paperLayout[y + 1][x - 1] == '@') ? (byte)1 : (byte)0;
                    adjacentRolls += (leftSafe && paperLayout[y][x - 1] == '@') ? (byte)1 : (byte)0;
                    adjacentRolls += (leftSafe && upSafe && paperLayout[y - 1][x - 1] == '@') ? (byte)1 : (byte)0;

                    accessiblePaperRolls += adjacentRolls < 4 ? (ushort)1 : (ushort)0;
                }
            }
        }

        return accessiblePaperRolls;

        /** Final Thoughts:
         * 
         *  Well, I ended up just directly reading the array and doing 8 checks. I've done
         *  this in past coding excercises. It's simple but it got the job done!
         */
    }
    public static ushort Day4Part2Answer()
    {
        /** Initial Thoughts:
         *  My initial day 1 was extrapolating the input into a 2d array for manipulation
         *  but then I ended up just reading values directly. Now I need to store
         *  state.
         * 
         *  I'll need to modify my part 1 solution to save state and then loop until no
         *  more are grabbed. I might try modifying in place and set them to X like in the
         *  puzzle.
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day4", "input.txt");
        char[][] paperLayout = [.. File.ReadAllLines(filePath).Select(line => line.ToCharArray())];
        byte floorDimension = (byte)paperLayout.Length;
        ushort accessiblePaperRolls = 0;
        byte rollsFound = 0;

        do
        {
            rollsFound = 0;
            for (byte y = 0; y < floorDimension; y++)
            {
                for (byte x = 0; x < floorDimension; x++)
                {
                    // Only check the cell if it's a roll
                    if (paperLayout[y][x] == '@')
                    {
                        // Check adjacent squares
                        bool upSafe = y > 0;
                        bool leftSafe = x > 0;
                        bool downSafe = y < floorDimension - 1;
                        bool rightSafe = x < floorDimension - 1;

                        byte adjacentRolls = 0;
                        adjacentRolls += (upSafe && paperLayout[y - 1][x] == '@') ? (byte)1 : (byte)0;
                        adjacentRolls += (upSafe && rightSafe && paperLayout[y - 1][x + 1] == '@') ? (byte)1 : (byte)0;
                        adjacentRolls += (rightSafe && paperLayout[y][x + 1] == '@') ? (byte)1 : (byte)0;
                        adjacentRolls += (rightSafe && downSafe && paperLayout[y + 1][x + 1] == '@') ? (byte)1 : (byte)0;
                        adjacentRolls += (downSafe && paperLayout[y + 1][x] == '@') ? (byte)1 : (byte)0;
                        adjacentRolls += (downSafe && leftSafe && paperLayout[y + 1][x - 1] == '@') ? (byte)1 : (byte)0;
                        adjacentRolls += (leftSafe && paperLayout[y][x - 1] == '@') ? (byte)1 : (byte)0;
                        adjacentRolls += (leftSafe && upSafe && paperLayout[y - 1][x - 1] == '@') ? (byte)1 : (byte)0;

                        if (adjacentRolls < 4)
                        {
                            accessiblePaperRolls ++;
                            rollsFound++;
                            paperLayout[y][x] = 'X';
                        }
                    }
                }
            }
        }
        while (rollsFound > 0);

        return accessiblePaperRolls;

        /** Final Thoughts:
         *  Really all I had to do was change from a string[] because it was immutable to something that was.
         *  I used LINQ to pull itinto a 2d char array and most of my base code still worked.
         *  Then all I had todo was implement a do while loop to iterate until nothing was removed!
         */
    }
}
