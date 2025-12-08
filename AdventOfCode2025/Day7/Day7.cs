using System.Linq;

namespace AdventOfCode2025.Day7;

public class Day7
{
    public static int Day7Part1Answer()
    {
        /** Initial Thoughts:
         *  We're now at the part of AoC where I'm starting to not be able
         *  to intuitively come up with solutions.
         *  
         *  I think I'll start out just going line by line, and keeping a
         *  counter of splits along with an array of current running lines
         *  (I'll turn this into a complex object later if I have to
         *  calculate strength of the edge in part 2)
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day7", "input.txt");
        var allLines = File.ReadAllLines(filePath);
        HashSet<int> beams = [allLines[0].IndexOf('S')];
        int splitCount = 0;

        foreach (var line in allLines.Where(s => s.Contains('^')))
        {
            List<int> splitBeams = [];
            List<int> terminatedBeams = [];
            foreach (var beam in beams)
            {
                if (line[beam] == '^')
                {
                    splitCount++;
                    terminatedBeams.Add(beam);
                    splitBeams.Add(beam + 1);
                    splitBeams.Add(beam - 1);
                }
            }

            foreach (var beam in splitBeams)
            {
                beams.Add(beam);
            }
            foreach (var beam in terminatedBeams)
            {
                beams.Remove(beam);
            }
        }

        return splitCount;
    }
    public static UInt128 Day7Part2Answer()
    {
        /** Initial Thoughts:
         *  This immediately brought to mind cyclomatic complexity in
         *  a control flow graph in unit testing: https://en.wikipedia.org/wiki/Cyclomatic_complexity
         * 
         *  There's a mathematical way to calculate this complexity, and I think I might be
         *  able to get that based on the splits and where they happen.
         *  Reading about it I can calculate the number of branches based on the depth,
         *  but that's not static here.
         *  
         *  https://www.sonarsource.com/resources/library/cyclomatic-complexity/ sheds
         *  some light on it with the formula C = E - N + 2P where E = edges, N = nodes
         *  and P = connected components, 1 for a single program. This is McCabe's
         *  cyclomatic complexity algorithm: https://ieeexplore.ieee.org/document/1702388
         *  
         *  Unfortunately, this only gets me the minimum number of paths, not every path.
         *  
         *  I think I need to create a control graph and traverse it. A bit of study
         *  shows that a directed acyclic graph is what I want. https://en.wikipedia.org/wiki/Directed_acyclic_graph
         *  
         *  https://leetcode.com/problems/all-paths-from-source-to-target/description/ this
         *  is basically the problem we have to solve but with a much more complex input.
         *  
         *  It looks like every ^ that is connected will become a vertex(node) along with the starting
         *  vertex and the characters on the last line where the beams exit. The beams themselves
         *  will become the edges.
         */



        // Old solution before stumbling block
        //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day7", "input.txt");
        //var allLines = File.ReadAllLines(filePath);
        //List<Vertice> vertices = [];
        //vertices.Add(new([0, allLines[0].IndexOf('S')]));
        //List<Edge> edges = [];
        //edges.Add(new([0, allLines[0].IndexOf('S')], null));

        //var y = 0;
        //foreach (var line in allLines.Where(s => s.Contains('^')))
        //{
        //    List<int[]> edgeParentVertices = [];
        //    List<int[]> edgeChildVertices = [];
        //    foreach (var edge in edges.Where(x => x.ChildVertex == null))
        //    {
        //        if (line[edge.ParentVertex[1]] == '^')
        //        {
        //            edgeChildVertices.Add([y,edge.ParentVertex[1]]);
        //            edgeParentVertices.Add([y, edge.ParentVertex[1] + 1]);
        //            edgeParentVertices.Add([y, edge.ParentVertex[1] - 1]);
        //        }
        //    }

        //    foreach (var parentVertex in edgeParentVertices)
        //    {
        //        vertices.Add(new([parentVertex[0],parentVertex[1]]));


        //        beams.Add(parentVertex);
        //    }
        //    foreach (var childVertex in edgeChildVertices)
        //    {
        //        beams.Remove(childVertex);
        //    }
        //    y++;
        //}

        //return 0;

        /** Stumbling Block
         *  While I could probably get this to work, I was running into issues
         *  with the vertice addresses changing when the split happened and starting thinking
         *  to myself that there has got to be another way.
         *  
         *  I'll admit I ended up looking at visualizations of others' solutions here.
         *  User EverybodyCodes (hey i'm gonna do that next November!) post this:
         *  https://old.reddit.com/r/adventofcode/comments/1pgbg8a/2025_day_7_part_2_visualization_for_the_sample/
         *  
         *  This was my original thought, I was thinking in terms of 'beam strength'. Going to try this.
         */
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day7", "input.txt");
        var allLines = File.ReadAllLines(filePath);
        HashSet<Beam> beams = [new Beam(allLines[0].IndexOf('S'),1)];

        foreach (var line in allLines.Where(s => s.Contains('^')))
        {
            HashSet<Beam> splitBeams = [];
            HashSet<Beam> terminatedBeams = [];
            foreach (var beam in beams)
            {
                if (line[beam.X] == '^')
                {
                    terminatedBeams.Add(new Beam(beam.X, beam.Strength));
                    splitBeams.Add(new Beam(beam.X + 1, beam.Strength));
                    splitBeams.Add(new Beam(beam.X - 1, beam.Strength));
                }
            }

            beams.RemoveWhere(x => terminatedBeams.Select(x => x.X).Contains(x.X));
            beams.UnionWith(splitBeams
                .GroupBy(b => b.X)
                .Select(g => new Beam(g.Key, g.Select(b => b.Strength).Aggregate(UInt128.Zero, (acc, val) => acc + val)))
                .ToHashSet()
            );

        }

        return beams.Select(x => x.Strength).Aggregate(UInt128.Zero, (acc, val) => acc + val);

        /** Final Thoughts
         *  Just as I expected at the beginning, this basically boiled down to
         *  "beam strength", and I should have just gone with that plan!
         */
    }
}

// Old classes from stumbling block solution
//sealed class Vertice(int[] Address)
//{
//    public int[] Address { get; set; } = Address;
//}

//sealed class Edge(int[] ParentVertex, int[]? ChildVertex)
//{
//    public int[] ParentVertex { get; set; } = ParentVertex;
//    public int[]? ChildVertex { get; set; } = ChildVertex;
//}

sealed class Beam(int x, UInt128 strength)
{
    public int X { get; } = x;
    public UInt128 Strength { get; } = strength;
}