namespace AdventOfCode2025.Day1;

public class Day1
{
    private LinkedList<int> Tumbler { get; set; }

    public Day1()
    {
        Tumbler = new LinkedList<int>([.. Enumerable.Range(0, 100)]);
    }

    public int Day1Part1Answer()
    {
        // Initial ideas:
        // The tumbler lock from 0-99 seems to me like I could use a linked list maybe or a stack.
        // I plan to basically create a stack and have two functions that can turn it. Then i'll iterate on
        // it over my instructions and if it's on 0 I'll record that.


        // Research:
        // Very quickly I thought "hmm a circular linked list could be good. I found this: https://stackoverflow.com/a/2670199
        // I really like this idea so I'm gonna try it.

        // Discoveries:
        // I had to use ! in the circular linked list as a null forgiving operator to clear up warnings with my modern C# project.

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day1", "part1input.txt");
        string[] instructions =  File.ReadAllLines(filePath);

        var current = Tumbler.Find(50)!;
        int zeroes = 0;
        foreach (string instruction in instructions)
        {
            if (instruction.StartsWith('L'))
            {
                for (int i = 0; i < int.Parse(instruction.Substring(1)); i++)
                {
                    current = current.PreviousOrLast();
                }
            } else
            {
                for (int i = 0; i < int.Parse(instruction.Substring(1)); i++)
                {
                    current = current.NextOrFirst();
                }
            }
            if (current.Value == 0)
            {
                zeroes++;
            }
        }

        return zeroes;
    }

    public int Day1Part2Answer()
    {
        // Initial thoughts: this is a simple change in my for loops I think, thankfully I used the circular linked list!
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day1", "part1input.txt");
        string[] instructions = File.ReadAllLines(filePath);

        var current = Tumbler.Find(50)!;
        int zeroes = 0;
        foreach (string instruction in instructions)
        {
            if (instruction.StartsWith('L'))
            {
                for (int i = 0; i < int.Parse(instruction.Substring(1)); i++)
                {
                    current = current.PreviousOrLast();
                    if (current.Value == 0)
                    {
                        zeroes++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < int.Parse(instruction.Substring(1)); i++)
                {
                    current = current.NextOrFirst();
                    if (current.Value == 0)
                    {
                        zeroes++;
                    }
                }
            }
        }

        return zeroes;
    }
}

// Source - https://stackoverflow.com/a
// Posted by Clueless, modified by community. See post 'Timeline' for change history
// Retrieved 2025-12-01, License - CC BY-SA 3.0
static class CircularLinkedList
{
    public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
    {
        return current.Next ?? current.List!.First!;
    }

    public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
    {
        return current.Previous ?? current.List!.Last!;
    }
}
