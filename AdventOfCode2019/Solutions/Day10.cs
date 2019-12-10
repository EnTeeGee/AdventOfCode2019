using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day10
    {
        private static readonly int[] Divisors = Enumerable.Range(2, 34).ToArray();

        [Solution(10, 1)]
        public int Problem1(string input)
        {
            var lines = Mapper.ToLines(input);
            var points = new HashSet<Point>();

            for (var i = 0; i < lines.Length; i++)
            {
                for(var j = 0; j < lines[i].Length; j++)
                {
                    if (lines[i][j] == '#')
                        points.Add(new Point(j, i));
                }
            }

            var totalRocks = points.Count();
            var bestRocks = 0;

            foreach(var point in points)
            {
                var current = totalRocks;

                foreach(var targetPoint in points)
                {
                    if(targetPoint.Equals(point))
                    {
                        current--;
                        continue;
                    }

                    var interim = GetInterimPoints(point, targetPoint);
                    if (interim.Any(it => points.Contains(it)))
                        current--;
                }

                if (current > bestRocks)
                    bestRocks = current;
            }

            return bestRocks;
        }

        [Solution(10, 2)]
        public int Problem2(string input)
        {
            var lines = Mapper.ToLines(input);
            var points = new HashSet<Point>();

            for (var i = 0; i < lines.Length; i++)
            {
                for (var j = 0; j < lines[i].Length; j++)
                {
                    if (lines[i][j] == '#')
                        points.Add(new Point(j, i));
                }
            }

            var totalRocks = points.Count();
            var bestRocks = 0;
            var bestRockPoint = new Point(0, 0);

            foreach (var point in points)
            {
                var current = totalRocks;

                foreach (var targetPoint in points)
                {
                    if (targetPoint.Equals(point))
                    {
                        current--;
                        continue;
                    }

                    var interim = GetInterimPoints(point, targetPoint);
                    if (interim.Any(it => points.Contains(it)))
                        current--;
                }

                if (current > bestRocks)
                {
                    bestRocks = current;
                    bestRockPoint = point;
                }
            }

            var angles = points.Where(it => !it.Equals(bestRockPoint)).ToDictionary(it => it, it => GetAngle(bestRockPoint, it));
            var destroyedSoFar = 0;

            while (angles.Any())
            {
                var toTarget = angles.Keys.OrderBy(it => angles[it]).ToList();
                var toTargetHash = new HashSet<Point>(toTarget);
                var toRemove = new List<Point>();

                foreach(var point in toTarget)
                {
                    var interim = GetInterimPoints(bestRockPoint, point);
                    if (interim.Any(it => toTargetHash.Contains(it)))
                        continue;

                    toRemove.Add(point);
                    destroyedSoFar++;

                    if (destroyedSoFar == 200)
                        return (point.X * 100) + point.Y;
                }

                foreach (var item in toRemove)
                    angles.Remove(item);
            }

            throw new Exception("Never found 200th point");
        }

        private Point[] GetInterimPoints(Point source, Point target)
        {
            //if (source.Equals(new Point(5, 8)) && target.Equals(new Point(5, 2)))
            //{
            //    Console.WriteLine("Issue");
            //}
            var altTest = Enumerable.Range(2, 35);

            var output = new List<Point>();
            var isXNeg = target.X < source.X;
            var isYNeg = target.Y < source.Y;
            var xDiff = Math.Abs(target.X - source.X);
            var yDiff = Math.Abs(target.Y - source.Y);

            //var xDivs = Primes.Where(it => xDiff % it == 0).Concat(new[] { xDiff }).ToArray();
            //var yDivs = Primes.Where(it => yDiff % it == 0).Concat(new[] { yDiff }).ToArray();
            var xDivs = altTest.Where(it => xDiff % it == 0).Concat(new[] { xDiff }).ToArray();
            var yDivs = altTest.Where(it => yDiff % it == 0).Concat(new[] { yDiff }).ToArray();
            var common = xDivs.Where(it => yDivs.Contains(it)).ToList();

            if (xDiff == 0)
                common.Add(yDiff);
            if (yDiff == 0)
                common.Add(xDiff);

            foreach (var item in common)
            {
                var xStep = (xDiff / item) * (isXNeg ? -1 : 1);
                var yStep = (yDiff / item) * (isYNeg ? -1 : 1);

                for(var i = 1; i < item; i++)
                {
                    var point = new Point(source.X + (xStep * i), source.Y + (yStep * i));
                    if (!output.Contains(point))
                        output.Add(point);
                }
                    
            }

            return output.ToArray();
        }

        private double GetAngle(Point source, Point target)
        {
            var xDiff = target.X - source.X;
            var yDiff = target.Y - source.Y;

            return -Math.Atan2(xDiff, yDiff);
        }

        private struct Point
        {
            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public int X { get; set; }

            public int Y { get; set; }

            public override bool Equals(object obj)
            {
                if (!(obj is Point))
                    return false;

                return ((Point)obj).X == X && ((Point)obj).Y == Y;
            }

            public override int GetHashCode()
            {
                return X | (Y << 8);
            }

            public override string ToString()
            {
                return $"({X}, {Y})";
            }
        }
    }
}
