namespace AdventOfCode2025.Day8;

public class Day8
{
    public static int Day8Part1Answer()
    {
        /** Initial Thoughts:
         *  Well, goodbye 2d, hello 3d! There are 1000 junction boxes
         *  in the input.
         *  
         *  I'm thinking of creating a junction class with it's location,
         *  a nullable circuit ID that's a GUID (if null it's alone)
         *  , a list of connected junctions by their location along with
         *  the distance to those junctions
         *  
         *  Then, I'll have to calculate distance. There are 1000 junctions,
         *  and each junction seems to have a max value of 99999. This means
         *  that there are 999,970,000,299,999 possible nodes in the 3d matrix.
         *  
         *  To calculate the distance between every one, it's 1000(1000-1)/2
         *  or 499,500 distances. My idea is to calculate every distance into
         *  a sortable collection (pointA,pointB, distance), then short by shortest
         *  distance.
         *  
         *  Then I can loop over this collection and start connecting up my
         *  junction boxes, 1000 times. Then i'll group my collection by
         *  circuit ID, and grab the sum of the top 3.
         */

        // Take in the input
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day8", "input.txt");
        List<Junction> junctions = [.. File.ReadAllLines(filePath).Select(x =>
            {
                var splitOut = x
                    .Split(',')
                    .Select(x => int.Parse(x))
                    .ToList();
                return new Junction([splitOut[0], splitOut[1], splitOut[2]], null);
            })];

        // Calculate all the distances
        List<PointDistance> distances = [];
        for (int i = 0; i < junctions.Count; i++)
        {
            for (int j = 0; j < junctions.Count; j++)
            {
                if (i == j) continue;
                distances.Add(new(junctions[i].Coordinates, junctions[j].Coordinates));
            }
        }
        distances = distances.DistinctBy(x => x.Distance).ToList();

        return 0;
    }
    public static UInt128 Day8Part2Answer()
    {
        return 0;
    }

}


public class Junction(int[] coordinates, Guid? circuitId = null)
{
    public int[] Coordinates { get; } = coordinates;
    public Guid? CircuitId { get; set; } = circuitId;
    public List<(int[] Coordinates, double Distance)> ConnectedJunctions { get; set; } = [];

    public void AddConnection(int[] coordinates, double distance)
    {
        ConnectedJunctions.Add((coordinates, distance));
    }
}

public class PointDistance(int[] pointA, int[] pointB)
{
    public int[] PointA { get; } = pointA;
    public int[] PointB { get; } = pointB;
    public double? Distance { get; } = CalculateDistance(pointA, pointB);

    // Rather than calculate the distance myself I'll lean on others:
    // Removed guard clauses and changed to int for my problem
    // https://github.com/TheAlgorithms/C-Sharp/blob/5aee9651da068d50a64cfa159fb695904e4a8b5b/Algorithms/LinearAlgebra/Distances/Euclidean.cs
    public static double CalculateDistance(int[] point1, int[] point2)
    {
        return Math.Sqrt(point1.Zip(point2, (x1, x2) => (long)(x1 - x2) * (x1 - x2)).Sum());
    }
}
