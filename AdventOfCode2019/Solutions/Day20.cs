using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day20
    {
        [Solution(20, 1)]
        public int Problem1(string input)
        {
            var lines = Mapper.ToLines(input);
            var walkable = new HashSet<Point>();
            var letters = new Dictionary<Point, char>();

            for(var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for(var j = 0; j < line.Length; j++)
                {
                    if (line[j] == '.')
                        walkable.Add(new Point(j, i));
                    else if (char.IsLetter(line[j]))
                        letters.Add(new Point(j, i), line[j]);
                }
            }

            var groundedLetters = new Dictionary<Point, char>();
            var ungroundedLetters = new Dictionary<Point, char>();

            foreach(var item in letters)
            {
                var surrounding = item.Key.GetSurrounding();
                if (surrounding.Any(it => walkable.Contains(it)))
                    groundedLetters.Add(item.Key, item.Value);
                else
                    ungroundedLetters.Add(item.Key, item.Value);
            }

            var portalPoints = new List<(string, Point)>();

            foreach(var point in groundedLetters.Keys)
            {
                var surrounding = point.GetSurrounding();
                var ungrounded = surrounding.Where(it => ungroundedLetters.ContainsKey(it)).First();
                var connectingPoint = surrounding.Where(it => walkable.Contains(it)).First();

                var left = new Point(point.X + 1, point.Y);
                var down = new Point(point.X, point.Y + 1);
                var tag = string.Empty;
                if (ungrounded.Equals(left) || ungrounded.Equals(down))
                    tag = new string(new[] { groundedLetters[point], ungroundedLetters[ungrounded] });
                else
                    tag = new string(new[] { ungroundedLetters[ungrounded], groundedLetters[point] });

                portalPoints.Add((tag, connectingPoint));
            }

            var startPoint = new Point(0, 0);
            var endPoint = new Point(0, 0);
            var portals = new Dictionary<Point, Point>();

            foreach(var item in portalPoints)
            {
                if (item.Item1 == "AA")
                    startPoint = item.Item2;
                else if (item.Item1 == "ZZ")
                    endPoint = item.Item2;
                else
                {
                    var matching = portalPoints.Where(it => !it.Item2.Equals(item.Item2) && it.Item1 == item.Item1).First();
                    portals.Add(item.Item2, matching.Item2);
                }
            }

            return WalkMaze(walkable, portals, startPoint, endPoint);
        }

        private int WalkMaze(HashSet<Point> walkable, Dictionary<Point, Point> portals, Point start, Point end)
        {
            var availablePaths = new Queue<Point[]>();
            var coveredPaths = new HashSet<Point>();
            coveredPaths.Add(start);
            availablePaths.Enqueue(new[] { start });

            while (availablePaths.Any())
            {
                var active = availablePaths.Dequeue();
                var currentPoint = active.Last();
                var options = currentPoint.GetSurrounding().Where(it => walkable.Contains(it)).ToList();

                if (options.Any(it => end.Equals(it)))
                    return active.Length;

                if (portals.ContainsKey(currentPoint))
                    options.Add(portals[currentPoint]);
                options = options.Where(it => !coveredPaths.Contains(it)).ToList();

                foreach(var item in options)
                {
                    coveredPaths.Add(item);
                    var newPath = active.Concat(new[] { item }).ToArray();
                    availablePaths.Enqueue(newPath);
                }
            }

            //RenderMaze(walkable, coveredPaths);

            return 0;
        }

        [Solution(20, 2)]
        public int Problem2(string input)
        {
            var lines = Mapper.ToLines(input);
            var walkable = new HashSet<Point>();
            var letters = new Dictionary<Point, char>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for (var j = 0; j < line.Length; j++)
                {
                    if (line[j] == '.')
                        walkable.Add(new Point(j, i));
                    else if (char.IsLetter(line[j]))
                        letters.Add(new Point(j, i), line[j]);
                }
            }

            var groundedLetters = new Dictionary<Point, char>();
            var ungroundedLetters = new Dictionary<Point, char>();

            foreach (var item in letters)
            {
                var surrounding = item.Key.GetSurrounding();
                if (surrounding.Any(it => walkable.Contains(it)))
                    groundedLetters.Add(item.Key, item.Value);
                else
                    ungroundedLetters.Add(item.Key, item.Value);
            }

            var portalPoints = new List<(string, Point)>();

            foreach (var point in groundedLetters.Keys)
            {
                var surrounding = point.GetSurrounding();
                var ungrounded = surrounding.Where(it => ungroundedLetters.ContainsKey(it)).First();
                var connectingPoint = surrounding.Where(it => walkable.Contains(it)).First();

                var left = new Point(point.X + 1, point.Y);
                var down = new Point(point.X, point.Y + 1);
                var tag = string.Empty;
                if (ungrounded.Equals(left) || ungrounded.Equals(down))
                    tag = new string(new[] { groundedLetters[point], ungroundedLetters[ungrounded] });
                else
                    tag = new string(new[] { ungroundedLetters[ungrounded], groundedLetters[point] });

                portalPoints.Add((tag, connectingPoint));
            }

            var startPoint = new Point(0, 0);
            var endPoint = new Point(0, 0);
            var portals = new Dictionary<Point, Point>();

            foreach (var item in portalPoints)
            {
                if (item.Item1 == "AA")
                    startPoint = item.Item2;
                else if (item.Item1 == "ZZ")
                    endPoint = item.Item2;
                else
                {
                    var matching = portalPoints.Where(it => !it.Item2.Equals(item.Item2) && it.Item1 == item.Item1).First();
                    portals.Add(item.Item2, matching.Item2);
                }
            }

            return WalkRecursiveMaze(walkable, portals, startPoint, endPoint);
        }

        private int WalkRecursiveMaze(HashSet<Point> walkable, Dictionary<Point, Point> portals, Point start, Point end)
        {
            var availablePaths = new Queue<LocationInfo>();
            var coveredPaths = new Dictionary<int, HashSet<Point>>();
            coveredPaths.Add(0, new HashSet<Point>());
            coveredPaths[0].Add(start);
            availablePaths.Enqueue(new LocationInfo { Pos = start, Level = 0, Length = 1 });
            var innerPortals = GetInnerPortals(walkable, portals);

            while (availablePaths.Any())
            {
                var active = availablePaths.Dequeue();
                var options = active.Pos.GetSurrounding().Where(it => walkable.Contains(it)).ToList();

                if (active.Level == 0 && options.Any(it => end.Equals(it)))
                    return active.Length;

                if (portals.ContainsKey(active.Pos))
                {
                    if (innerPortals.Contains(active.Pos))
                    {
                        var newPath = new LocationInfo
                        {
                            Pos = portals[active.Pos],
                            Level = active.Level + 1,
                            Length = active.Length + 1
                        };
                        if(!IsCovered(newPath.Pos, newPath.Level, coveredPaths))
                        {
                            availablePaths.Enqueue(newPath);
                            AddToCovered(newPath.Pos, newPath.Level, coveredPaths);
                        }
                    }
                    else if(active.Level != 0)
                    {
                        var newPath = new LocationInfo
                        {
                            Pos = portals[active.Pos],
                            Level = active.Level - 1,
                            Length = active.Length + 1
                        };
                        if(!IsCovered(newPath.Pos, newPath.Level, coveredPaths))
                        {
                            availablePaths.Enqueue(newPath);
                            AddToCovered(newPath.Pos, newPath.Level, coveredPaths);
                        }
                    }
                }

                options = options.Where(it => !IsCovered(it, active.Level, coveredPaths)).ToList();

                foreach(var item in options)
                {
                    AddToCovered(item, active.Level, coveredPaths);
                    var newPath = new LocationInfo
                    {
                        Pos = item,
                        Level = active.Level,
                        Length = active.Length + 1
                    };
                    availablePaths.Enqueue(newPath);
                }
            }

            return 0;
        }

        private HashSet<Point> GetInnerPortals(HashSet<Point> walkable, Dictionary<Point, Point> portals)
        {
            var hori = walkable.OrderBy(it => it.X).ToList();
            var vert = walkable.OrderBy(it => it.Y).ToList();

            var top = vert.First().Y + 1;
            var bottom = vert.Last().Y - 1;
            var left = hori.First().X + 1;
            var right = hori.Last().X - 1;

            var output = new HashSet<Point>();
            foreach(var item in portals.Keys)
            {
                if (item.X > left && item.X < right
                    && item.Y > top && item.Y < bottom)
                    output.Add(item);
            }

            return output;
        }

        private void AddToCovered(Point point, int level, Dictionary<int, HashSet<Point>> dict)
        {
            if (!dict.ContainsKey(level))
                dict.Add(level, new HashSet<Point>());

            dict[level].Add(point);
        }

        private bool IsCovered(Point point, int level, Dictionary<int, HashSet<Point>> dict)
        {
            if (!dict.ContainsKey(level))
                return false;

            return dict[level].Contains(point);
        }

        private void RenderMaze(HashSet<Point> walkable, HashSet<Point> coveredPaths)
        {
            var right = walkable.OrderByDescending(it => it.X).First().X;
            var bottom = walkable.OrderByDescending(it => it.Y).First().Y;

            for (var i = 0; i < bottom; i++)
            {
                var line = new StringBuilder();
                for (var j = 0; j < right; j++)
                {
                    var point = new Point(j, i);
                    if (coveredPaths.Contains(point))
                        line.Append("0");
                    else if (walkable.Contains(point))
                        line.Append(" ");
                    else
                        line.Append("█");
                }
                Console.WriteLine(line.ToString());
            }
        }

        private class LocationInfo
        {
            public Point Pos { get; set; }

            public int Level { get; set; }

            public int Length { get; set; }
        }
    }
}
