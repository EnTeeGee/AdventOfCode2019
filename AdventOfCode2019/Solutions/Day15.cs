using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day15
    {
        private enum Direction { North = 1, Sourth = 2, West = 3, East = 4 }

        [Solution(15, 1)]
        public int Problem1(string input)
        {
            var robot = new Intcode(input);
            var explored = new Dictionary<Point, bool>();
            var unexplored = new HashSet<Point>();
            var currentPos = new Point(0, 0);
            var oxySystem = (Point?)null;

            explored.Add(currentPos, true);
            var surroundingStart = GetSurroundingPoints(currentPos);
            foreach (var item in surroundingStart)
                unexplored.Add(item);

            while (unexplored.Any())
            {
                // Find nearest unexplored
                var nearest = unexplored.OrderBy(it => it.ManDistanceTo(currentPos)).First();

                // Path to unexplored
                var path = PathTo(currentPos, nearest, explored);
                var lastOutput = 0L;

                for(var i = 0; i < path.Length - 1; i++)
                {
                    var direction = GetDirection(path[i], path[i + 1]);
                    robot.AddInput((int)direction);
                    lastOutput = robot.RunToOutput().Value;
                }

                // Update explored, unexplored and oxy system based on final value of output
                unexplored.Remove(path.Last());
                if(lastOutput == 0)
                {
                    currentPos = path[path.Length - 2];
                    explored.Add(path.Last(), false);
                }
                else
                {
                    currentPos = path.Last();

                    if (lastOutput == 2)
                        oxySystem = path.Last();

                    explored.Add(path.Last(), true);
                    var surrounding = GetSurroundingPoints(path.Last()).Where(it => !unexplored.Contains(it) && !explored.ContainsKey(it)).ToArray();
                    foreach (var item in surrounding)
                        unexplored.Add(item);
                }
            }

            //Console.WriteLine(DumpMap(explored, oxySystem.Value));

            return PathTo(new Point(0, 0), oxySystem.Value, explored).Length - 1;
        }

        [Solution(15, 2)]
        public int Problem2(string input)
        {
            var robot = new Intcode(input);
            var explored = new Dictionary<Point, bool>();
            var unexplored = new HashSet<Point>();
            var currentPos = new Point(0, 0);
            var oxySystem = (Point?)null;

            explored.Add(currentPos, true);
            var surroundingStart = GetSurroundingPoints(currentPos);
            foreach (var item in surroundingStart)
                unexplored.Add(item);

            while (unexplored.Any())
            {
                var nearest = unexplored.OrderBy(it => it.ManDistanceTo(currentPos)).First();

                var path = PathTo(currentPos, nearest, explored);
                var lastOutput = 0L;

                for (var i = 0; i < path.Length - 1; i++)
                {
                    var direction = GetDirection(path[i], path[i + 1]);
                    robot.AddInput((int)direction);
                    lastOutput = robot.RunToOutput().Value;
                }

                unexplored.Remove(path.Last());
                if (lastOutput == 0)
                {
                    currentPos = path[path.Length - 2];
                    explored.Add(path.Last(), false);
                }
                else
                {
                    currentPos = path.Last();

                    if (lastOutput == 2)
                        oxySystem = path.Last();

                    explored.Add(path.Last(), true);
                    var surrounding = GetSurroundingPoints(path.Last()).Where(it => !unexplored.Contains(it) && !explored.ContainsKey(it)).ToArray();
                    foreach (var item in surrounding)
                        unexplored.Add(item);
                }
            }

            var validPointMap = new HashSet<Point>(explored.Keys.Where(it => explored[it]));

            return DistFromOxySystem(validPointMap, oxySystem.Value);
        }

        private Point[] GetSurroundingPoints(Point origin)
        {
            return new Point[]
            {
                new Point(origin.X, origin.Y - 1),
                new Point(origin.X + 1, origin.Y),
                new Point(origin.X, origin.Y + 1),
                new Point(origin.X - 1, origin.Y)
            };
        }

        private Point[] PathTo(Point source, Point target, Dictionary<Point, bool> exploredPoints)
        {
            var activePaths = new Queue<Point[]>();
            activePaths.Enqueue(new Point[] { source });
            var visitedPoints = new HashSet<Point>();

            while (activePaths.Any())
            {
                var workingPath = activePaths.Dequeue();
                var surrounding = GetSurroundingPoints(workingPath.Last());
                if (surrounding.Any(it => it.Equals(target)))
                    return workingPath.Concat(new Point[] { target }).ToArray();

                var validOptions = surrounding.Where(it => !visitedPoints.Contains(it) && exploredPoints.ContainsKey(it) && exploredPoints[it]).ToArray();
                foreach (var item in validOptions)
                    visitedPoints.Add(item);

                var newPaths = validOptions.Select(it => workingPath.Concat(new Point[] { it }).ToArray()).ToArray();
                foreach (var item in newPaths)
                    activePaths.Enqueue(item);
            }

            throw new Exception("No valid path found");
        }

        private Direction GetDirection(Point source, Point target)
        {
            if (source.X - target.X == 0 && source.Y - target.Y == 1)
                return Direction.North;
            else if (source.X - target.X == 0 && source.Y - target.Y == -1)
                return Direction.Sourth;
            else if (source.X - target.X == 1 && source.Y - target.Y == 0)
                return Direction.West;
            else if (source.X - target.X == -1 && source.Y - target.Y == 0)
                return Direction.East;

            throw new Exception("Points not next to each other");
        }

        private string DumpMap(Dictionary<Point, bool> map, Point target)
        {
            var top = map.Keys.Min(it => it.Y);
            var bottom = map.Keys.Max(it => it.Y);
            var left = map.Keys.Min(it => it.X);
            var right = map.Keys.Max(it => it.X);
            var output = new StringBuilder();

            for(var i = top; i <= bottom; i++)
            {
                var charLine = Enumerable.Range(left, right - left + 1).Select(it =>
                {
                    var point = new Point(it, i);

                    if (point.Equals(target))
                        return 'X';

                    if (point.Equals(new Point(0, 0)))
                        return '0';

                    if (!map.ContainsKey(point))
                        return '█';

                    return map[point] ? '.' : '█';
                }).ToArray();

                output.AppendLine(new string(charLine));
            }

            return output.ToString();
        }

        private int DistFromOxySystem(HashSet<Point> validPoints, Point startPoint)
        {
            var availablePaths = new Queue<Point[]>();
            var coveredPoints = new HashSet<Point>();
            coveredPoints.Add(startPoint);
            var maxDistance = 0;
            availablePaths.Enqueue(new Point[] { startPoint });

            while (availablePaths.Any())
            {
                var workingPath = availablePaths.Dequeue();
                var options = GetSurroundingPoints(workingPath.Last()).Where(it => validPoints.Contains(it) && !coveredPoints.Contains(it)).ToArray();
                if(options.Length == 0)
                {
                    if (workingPath.Length - 1 > maxDistance)
                        maxDistance = workingPath.Length - 1;

                    continue;
                }

                foreach (var item in options)
                    coveredPoints.Add(item);

                var newArrays = options.Select(it => workingPath.Concat(new Point[] { it }).ToArray()).ToArray();
                foreach (var item in newArrays)
                    availablePaths.Enqueue(item);
            }

            return maxDistance;
        }
    }
}
