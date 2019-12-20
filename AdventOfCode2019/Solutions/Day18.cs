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
        // 4254 too high
        // 3694 too high
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
                //var dist = item.Value.Path.Count() - 1 + GetShortestDistance(new[] { item.Key }, new HashSet<string>(allKeys.Where(it => it != item.Key)), sourceDict, 0, bestDistance);
                //if (dist < bestDistance)
                //    bestDistance = dist;
                var dist = ShortestSearch2(item.Key, item.Value.Path.Count() - 1, allKeys.ToArray(), sourceDict);
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

        private int ShortestSearch2(string startingPoint, int startingDistance, string[] allKeys, Dictionary<string, PathInfo[]> sourceDict)
        {
            // start with an empty list of progressinfos. Add to it one item, representing the distance to the starting point
            // Then, while true:
            // remove the highest ranked item from the end of list
            // If this item has no more keys to find, return it. It's the best path
            // else, find all the keys can can be reached from this key that still need to be got
            // If there's no keys that can be reached, throw an error (Shouldn't get in this state)
            // For each of these, create a new Progress info with the updated totalDistance and keys
            // Add each of these to to the current list, then sort the list by total distnace, putting the shortest at the end

            // Possible improvement: move it to a linked list and insert items in their correct position, rather than sorting it each time.

            //var current = new List<ProgressInfo>();
            var current2 = new LinkedList<ProgressInfo>();
            var startingItem = new ProgressInfo();
            startingItem.CurrentKey = startingPoint;
            startingItem.CollectedKeys = new HashSet<string> { startingPoint };
            startingItem.RemainingKeys = new HashSet<string>(allKeys.Where(it => it != startingPoint));
            startingItem.TotalDistance = startingDistance;
            //current.Add(startingItem);
            current2.AddFirst(startingItem);

            while (current2.Any())
            {
                //var best = current.First();
                var best = current2.First.Value;
                //current.RemoveAt(0);
                current2.RemoveFirst();

                if (!best.RemainingKeys.Any())
                    return best.TotalDistance;

                var reachableKeys = sourceDict[best.CurrentKey].Where(it => best.RemainingKeys.Contains(it.TargetKey));
                reachableKeys = reachableKeys.Where(it => it.Doors.All(d => best.CollectedKeys.Contains(d))).ToArray();

                if (!reachableKeys.Any())
                    throw new Exception("Found point where no keys are reachable");

                foreach(var item in reachableKeys)
                {
                    var newInfo = new ProgressInfo();
                    newInfo.CurrentKey = item.TargetKey;
                    newInfo.CollectedKeys = new HashSet<string>(best.CollectedKeys);
                    newInfo.CollectedKeys.Add(item.TargetKey);
                    newInfo.RemainingKeys = new HashSet<string>(best.RemainingKeys);
                    newInfo.RemainingKeys.Remove(item.TargetKey);
                    newInfo.TotalDistance = best.TotalDistance + item.Length;

                    if (newInfo.TotalDistance >= 3694) // Limit returned by AoC
                        continue;

                    var matchingKeys = current2.FirstOrDefault(it => it.CurrentKey == newInfo.CurrentKey && HasSameKeys(newInfo.CollectedKeys, it.CollectedKeys));
                    if(matchingKeys != null)
                    {
                        if (matchingKeys.TotalDistance < newInfo.TotalDistance)
                            continue;
                        else
                            current2.Remove(matchingKeys);
                    }

                    //newInfo.WeightedDistance = (newInfo.TotalDistance * 2) / (double)(newInfo.CollectedKeys.Count * 3);
                    //newInfo.WeightedDistance = newInfo.TotalDistance / (double)newInfo.CollectedKeys.Count;

                    //current.Add(newInfo);

                    if (!current2.Any() || current2.First.Value.TotalDistance > newInfo.TotalDistance)
                    //if (!current2.Any() || current2.First.Value.WeightedDistance > newInfo.WeightedDistance)
                        current2.AddFirst(newInfo);
                    else if (current2.Last.Value.TotalDistance < newInfo.TotalDistance)
                    //else if (current2.Last.Value.WeightedDistance < newInfo.WeightedDistance)
                        current2.AddLast(newInfo);
                    else
                    {
                        var latest = current2.Last;
                        while (latest.Value.TotalDistance > newInfo.TotalDistance && latest.Previous != null)
                        //while (latest.Value.WeightedDistance > newInfo.WeightedDistance && latest.Previous != null)
                            latest = latest.Previous;

                        current2.AddAfter(latest, newInfo);
                    }
                }

                //current = current.OrderBy(it => it.TotalDistance).ToList();
            }

            return int.MaxValue;
        }

        private bool HasSameKeys(HashSet<string> first, HashSet<string> second)
        {
            if (first.Count != second.Count)
                return false;

            foreach (var item in first)
                if (!second.Contains(item))
                    return false;

            return true;
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

        private class ProgressInfo
        {
            public string CurrentKey { get; set; }

            public HashSet<string> CollectedKeys { get; set; }

            public HashSet<string> RemainingKeys { get; set; }

            public int TotalDistance { get; set; }

            public double WeightedDistance { get; set; }
        }
    }
}
