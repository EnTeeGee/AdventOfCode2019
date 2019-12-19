using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day18
    {
        // 3524 too low
        [Solution(18, 1)]
        public int Problem1(string input)
        {
            var lines = Mapper.ToLines(input);
            var walkable = new HashSet<Point>();
            var keys = new Dictionary<string, Point>();
            var doors = new Dictionary<Point, string>();
            var startPoint = new Point(0, 0);

            for(var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                for(var j = 0; j < line.Length; j++)
                {
                    var symbol = line[j];
                    if (symbol == '#')
                        continue;

                    var point = new Point(j, i);
                    walkable.Add(point);

                    if (symbol == '.')
                        continue;

                    if (symbol == '@')
                        startPoint = point;
                    else if (char.IsLower(symbol))
                        keys.Add(symbol.ToString().ToUpper(), point);
                    else
                        doors.Add(point, symbol.ToString());
                }
            }

            // Option: Get every key that can be reached from origin. For each of these keys, try and reach every other key until best path is found.
            // Another option is to get the path between each key, tracking the length and which doors need to be opened to get to it. This list can then be recursed through,
            // returning the shortest sum

            // Get distances between every key
            var keyDistances = new List<PathInfo>();
            var sourceDict = new Dictionary<string, PathInfo[]>();
            var targetDict = new Dictionary<string, PathInfo[]>();
            var allKeys = new List<string>();

            foreach(var source in keys.Keys)
            {
                foreach(var target in keys.Keys)
                {
                    if (source == target)
                        continue;

                    var path = PathTo(keys[source], keys[target], walkable, doors);
                    var info = new PathInfo { SourceKey = source, TargetKey = target, Doors = path.Doors.ToArray(), Length = path.Path.Count - 1 };
                    keyDistances.Add(info);
                }
            }

            foreach(var item in keys.Keys)
            {
                sourceDict.Add(item, keyDistances.Where(it => it.SourceKey == item).ToArray());
                targetDict.Add(item, keyDistances.Where(it => it.TargetKey == item).ToArray());
            }
            allKeys = keys.Keys.ToList();

            // Get paths from origin to each accessable key
            var starterKeys = new Dictionary<string, PathBuilder>();
            foreach(var item in keys.Keys)
            {
                var path = PathTo(startPoint, keys[item], walkable, doors);
                if (!path.Doors.Any())
                    starterKeys.Add(item, path);
            }

            // Recurse to find shortest distance
            var bestDistance = int.MaxValue;

            foreach(var item in starterKeys)
            {
                //keyDistances.Where(it => it.TargetKey != item.Key).ToList()

                var dist = item.Value.Path.Count() - 1 + GetShortestDistance(new[] { item.Key }, new HashSet<string>(allKeys.Where(it => it != item.Key)), sourceDict, 0, bestDistance);
                if (dist < bestDistance)
                    bestDistance = dist;
            }

            return bestDistance;
        }

        private PathBuilder PathTo(Point source, Point target, HashSet<Point> valid, Dictionary<Point, string> doors)
        {
            var visited = new HashSet<Point>();
            visited.Add(source);
            var active = new Queue<PathBuilder>();
            active.Enqueue(new PathBuilder { Path = new List<Point> { source } });

            while (active.Any())
            {
                var working = active.Dequeue();
                var surrounding = working.Path.Last().GetSurrounding();
                if(surrounding.Any(it => it.Equals(target)))
                {
                    working.Path.Add(target);
                    return working;
                }

                var validOptions = surrounding.Where(it => valid.Contains(it) && !visited.Contains(it)).ToArray();
                foreach(var item in validOptions)
                {
                    var newWorking = new PathBuilder { Path = working.Path.Concat(new Point[] { item }).ToList(), Doors = working.Doors.ToList() };
                    if (doors.Keys.Any(it => it.Equals(item)))
                        newWorking.Doors.Add(doors[item]);

                    active.Enqueue(newWorking);
                    visited.Add(item);
                }
            }

            return null;
        }

        private int GetShortestDistance(string[] gotKeys, HashSet<string> remainingKeys, Dictionary<string, PathInfo[]> sourceDict, int currentDistance, int pathToBeat)
        {
            if (!remainingKeys.Any())
                return currentDistance;

            var latest = gotKeys.Last();
            //var possibleTargets = available.Where(it => it.SourceKey == latest && !gotKeys.Contains(it.TargetKey)).ToList();
            var possibleTargets = sourceDict[latest].Where(it => remainingKeys.Contains(it.TargetKey));
            if (!possibleTargets.Any()) // Reached the end, no more keys to get
                return currentDistance;

            possibleTargets = possibleTargets.Where(it => it.Doors.All(d => gotKeys.Contains(d))).ToList();
            var currentBest = pathToBeat;

            foreach(var item in possibleTargets.OrderBy(it => it.Length))
            {
                var newDistance = currentDistance + item.Length;
                if (newDistance > currentBest)
                    continue;

                var withKey = gotKeys.Concat(new[] { item.TargetKey }).ToArray();
                //var withoutKey = available.Where(it => it.TargetKey != item.TargetKey || it.SourceKey == latest).ToList();
                var dist = GetShortestDistance(withKey, new HashSet<string>(remainingKeys.Where(it => it != item.TargetKey)), sourceDict, newDistance, currentBest);
                if (dist < currentBest)
                    currentBest = dist;
            }

            return currentBest;
        }

        private class PathBuilder
        {
            public PathBuilder()
            {
                Doors = new List<string>();
            }

            public List<Point> Path { get; set; }

            public List<string> Doors { get; set; }
        }

        private class PathInfo
        {
            public string SourceKey { get; set; }

            public string TargetKey { get; set; }

            public string[] Doors { get; set; }

            public int Length { get; set; }
        }
    }
}
