namespace AdventOfCode2025.Day8;

public class Day8
{
    public static int Day8Part1Answer(int connectionCount = 1000)
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
        var junctionArrays = File.ReadAllLines(filePath).Select(x =>
            {
                return x
                    .Split(',')
                    .Select(x => int.Parse(x))
                    .ToArray();
            }).ToList();
        List<Junction> junctions = [];
        for (int i = 0; i < junctionArrays.Count; i++)
        {
            junctions.Add(new Junction(i, junctionArrays[i]));
        }

        // Calculate all the distances
        List<PointDistance> distances = [];
        for (int i = 0; i < junctions.Count; i++)
        {
            for (int j = 0; j < junctions.Count; j++)
            {
                if (i == j) continue;
                distances.Add(new(junctions[i].Coordinates, junctions[i].Id, junctions[j].Coordinates, junctions[j].Id));
            }
        }
        distances = [.. distances.DistinctBy(x => x.Distance).OrderBy(x => x.Distance)]; // I think it's easier to just filter later, the calculation is cheap

        for (int i = 0; i < connectionCount; i++)
        {
            PointDistance currentClosest = distances[i];
            Junction junctionA = junctions[currentClosest.PointAId];
            Junction junctionB = junctions[currentClosest.PointBId];

            junctionA.AddConnection(junctionB.Id, currentClosest.Distance);
            junctionB.AddConnection(junctionA.Id, currentClosest.Distance);

            Guid circuitID = Guid.NewGuid();
            HashSet<int> circuitJunctions= [];
            circuitJunctions.UnionWith(junctionA.ConnectedJunctions.Select(x => x.Id));
            circuitJunctions.UnionWith(junctionB.ConnectedJunctions.Select(x => x.Id));
            int addedJunctionCount = 0;
            do
            {
                int startingJunctions = circuitJunctions.Count;
                var loopJunctions = circuitJunctions.ToList();
                foreach (var circuitJunction in loopJunctions)
                {
                    circuitJunctions.UnionWith(junctions[circuitJunction].ConnectedJunctions.Select(x => x.Id));
                }
                addedJunctionCount = circuitJunctions.Count - startingJunctions;
            }
            while (addedJunctionCount > 0);

            foreach (var id in circuitJunctions)
            {
                junctions[id].CircuitId = circuitID;
            }
        }

        var circuitProduct = junctions
            .GroupBy(x => x.CircuitId)
            .Select(g => new
            {
                Count = g.Count()
            })
            .Select(x => x.Count)
            .OrderDescending()
            .Take(3)
            .Aggregate(1, (acc, n) => acc * n);

        return circuitProduct;
    }
    public static ulong Day8Part2Answer()
    {
        /** Initial Thoughts:
         * I'm failing to see the massive complication here but I'm probably missing something.
         * I'm gonna just update my iterator to be a do while loop instead of looping 1000
         * times.
         * 
         * Once the circuit ID is the same in the collection, I will know what to check.
         */

        /** Final Thoughts:
         *  Part 2 didn't really represent much difficulty to me, probably due to how I
         *  setup part 1 with my classes. This is a good example of how the foundational
         *  architecture of systems being designed for expansability and flexibility is
         *  important.
         *  
         *  My only issue was the result was larger than I anticipated and I got a
         *  wraparound until I made it uint64.
         */

        // Take in the input
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day8", "input.txt");
        var junctionArrays = File.ReadAllLines(filePath).Select(x =>
        {
            return x
                .Split(',')
                .Select(x => int.Parse(x))
                .ToArray();
        }).ToList();
        List<Junction> junctions = [];
        for (int i = 0; i < junctionArrays.Count; i++)
        {
            junctions.Add(new Junction(i, junctionArrays[i]));
        }

        // Calculate all the distances
        List<PointDistance> distances = [];
        for (int i = 0; i < junctions.Count; i++)
        {
            for (int j = 0; j < junctions.Count; j++)
            {
                if (i == j) continue;
                distances.Add(new(junctions[i].Coordinates, junctions[i].Id, junctions[j].Coordinates, junctions[j].Id));
            }
        }
        distances = [.. distances.DistinctBy(x => x.Distance).OrderBy(x => x.Distance)]; // I think it's easier to just filter later, the calculation is cheap

        int distanceIndex = 0;
        int lastJunctionAXCoords = 0;
        int lastJunctionBXCoords = 0;
        do
        {
            PointDistance currentClosest = distances[distanceIndex];
            distanceIndex++;
            Junction junctionA = junctions[currentClosest.PointAId];
            Junction junctionB = junctions[currentClosest.PointBId];

            lastJunctionAXCoords = junctionA.Coordinates[0];
            lastJunctionBXCoords = junctionB.Coordinates[0];

            junctionA.AddConnection(junctionB.Id, currentClosest.Distance);
            junctionB.AddConnection(junctionA.Id, currentClosest.Distance);

            Guid circuitID = Guid.NewGuid();
            HashSet<int> circuitJunctions = [];
            circuitJunctions.UnionWith(junctionA.ConnectedJunctions.Select(x => x.Id));
            circuitJunctions.UnionWith(junctionB.ConnectedJunctions.Select(x => x.Id));
            int addedJunctionCount = 0;
            do
            {
                int startingJunctions = circuitJunctions.Count;
                var loopJunctions = circuitJunctions.ToList();
                foreach (var circuitJunction in loopJunctions)
                {
                    circuitJunctions.UnionWith(junctions[circuitJunction].ConnectedJunctions.Select(x => x.Id));
                }
                addedJunctionCount = circuitJunctions.Count - startingJunctions;
            }
            while (addedJunctionCount > 0);

            foreach (var id in circuitJunctions)
            {
                junctions[id].CircuitId = circuitID;
            }
        }
        while (junctions.Select(x => x.CircuitId).Distinct().Count() > 1);

        return (ulong)lastJunctionAXCoords * (ulong)lastJunctionBXCoords;
    }
}


public class Junction(int id, int[] coordinates)
{
    public int Id { get; set; } = id;
    public int[] Coordinates { get; } = coordinates;
    public Guid? CircuitId { get; set; } = Guid.NewGuid();
    public List<(int Id, double Distance)> ConnectedJunctions { get; set; } = [];

    public void AddConnection(int id, double distance)
    {
        ConnectedJunctions.Add((id, distance));
    }
}

public class PointDistance(int[] pointA, int pointAId, int[] pointB, int pointBId)
{
    public int PointAId { get; set; } = pointAId;
    public int PointBId { get; set; } = pointBId;
    public double Distance { get; } = CalculateDistance(pointA, pointB);

    // Rather than calculate the distance myself I'll lean on others:
    // Removed guard clauses and changed to int for my problem
    // https://github.com/TheAlgorithms/C-Sharp/blob/5aee9651da068d50a64cfa159fb695904e4a8b5b/Algorithms/LinearAlgebra/Distances/Euclidean.cs
    public static double CalculateDistance(int[] point1, int[] point2)
    {
        return Math.Sqrt(point1.Zip(point2, (x1, x2) => (long)(x1 - x2) * (x1 - x2)).Sum());
    }
}
