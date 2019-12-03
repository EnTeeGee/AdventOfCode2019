using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day03
    {
        [Solution(3, 1)]
        public string Problem1(string input)
        {
            var lines = Mapper.ToLines(input);
            var wire1 = ToSegments(Mapper.ToCsvs(lines[0]));
            var wire2 = ToSegments(Mapper.ToCsvs(lines[1]));

            var closest = int.MaxValue;
            var leftRight = wire2.Where(it => it.Orientation == Orientation.Left || it.Orientation == Orientation.Right).ToArray();
            var upDown = wire2.Where(it => it.Orientation == Orientation.Up || it.Orientation == Orientation.Down).ToArray();
            
            foreach(var item in wire1)
            {
                var targetWires = item.IsUpDown() ? leftRight : upDown;

                foreach(var target in targetWires)
                {
                    var val1 = item.Intersects(target);
                    var val2 = target.Intersects(item);

                    if (val1 == null || val2 == null)
                        continue;

                    closest = Math.Min(closest, Math.Abs(val1.Value) + Math.Abs(val2.Value));
                }
            }

            return closest.ToString();
        }

        [Solution(3, 2)]
        public string Problem2(string input)
        {
            var lines = Mapper.ToLines(input);
            var wire1 = ToSegments(Mapper.ToCsvs(lines[0]));
            var wire2 = ToSegments(Mapper.ToCsvs(lines[1]));

            var shortest = int.MaxValue;
            var leftRight = wire2.Where(it => it.Orientation == Orientation.Left || it.Orientation == Orientation.Right).ToArray();
            var upDown = wire2.Where(it => it.Orientation == Orientation.Up || it.Orientation == Orientation.Down).ToArray();

            foreach (var item in wire1)
            {
                var targetWires = item.IsUpDown() ? leftRight : upDown;

                foreach (var target in targetWires)
                {
                    var val1 = item.IntersectsSteps(target);
                    var val2 = target.IntersectsSteps(item);

                    if (val1 == null || val2 == null)
                        continue;

                    shortest = Math.Min(shortest, val1.Value + val2.Value);
                }
            }

            return shortest.ToString();
        }

        private LineSegment[] ToSegments(string[] inputs)
        {
            var current = (x: 0, y: 0);
            var currentDistance = 0;
            var output = new List<LineSegment>();

            foreach(var item in inputs)
            {
                var toAdd = new LineSegment { StartPoint = current, StepsFromStart = currentDistance };
                current = (x: current.x, y: current.y);
                var value = Convert.ToInt32(item.Substring(1));

                switch(item[0])
                {
                    case 'U':
                        toAdd.Orientation = Orientation.Up;
                        current.y += value;
                        break;
                    case 'R':
                        toAdd.Orientation = Orientation.Right;
                        current.x += value;
                        break;
                    case 'D':
                        toAdd.Orientation = Orientation.Down;
                        current.y -= value;
                        break;
                    case 'L':
                        toAdd.Orientation = Orientation.Left;
                        current.x -= value;
                        break;
                    default:
                        throw new Exception("Unexpected orientation");
                }

                toAdd.EndPoint = current;
                output.Add(toAdd);
                currentDistance += value;
            }

            return output.ToArray();
        }

        private class LineSegment
        {
            public Orientation Orientation { get; set; }

            public (int x, int y) StartPoint { get; set; }

            public (int x, int y) EndPoint { get; set; }

            public int StepsFromStart { get; set; }

            public bool IsUpDown()
            {
                return this.Orientation == Orientation.Up || this.Orientation == Orientation.Down;
            }

            public int? Intersects(LineSegment input)
            {
                if (IsUpDown() == input.IsUpDown())
                    return null;

                switch (Orientation)
                {
                    case Orientation.Up:
                        if (input.StartPoint.y > StartPoint.y && input.StartPoint.y < EndPoint.y)
                            return input.StartPoint.y;
                        else
                            return null;
                    case Orientation.Down:
                        if (input.StartPoint.y > EndPoint.y && input.StartPoint.y < StartPoint.y)
                            return input.StartPoint.y;
                        else
                            return null;
                    case Orientation.Right:
                        if (input.StartPoint.x > StartPoint.x && input.StartPoint.x < EndPoint.x)
                            return input.StartPoint.x;
                        else
                            return null;
                    case Orientation.Left:
                        if (input.StartPoint.x > EndPoint.x && input.StartPoint.x < StartPoint.x)
                            return input.StartPoint.x;
                        else
                            return null;
                }

                return null;
            }

            public int? IntersectsSteps(LineSegment input)
            {
                if (IsUpDown() == input.IsUpDown())
                    return null;

                switch (Orientation)
                {
                    case Orientation.Up:
                        if (input.StartPoint.y > StartPoint.y && input.StartPoint.y < EndPoint.y)
                            return StepsFromStart + (input.StartPoint.y - StartPoint.y);
                        else
                            return null;
                    case Orientation.Down:
                        if (input.StartPoint.y > EndPoint.y && input.StartPoint.y < StartPoint.y)
                            return StepsFromStart + (StartPoint.y - input.StartPoint.y);
                        else
                            return null;
                    case Orientation.Right:
                        if (input.StartPoint.x > StartPoint.x && input.StartPoint.x < EndPoint.x)
                            return StepsFromStart + (input.StartPoint.x - StartPoint.x);
                        else
                            return null;
                    case Orientation.Left:
                        if (input.StartPoint.x > EndPoint.x && input.StartPoint.x < StartPoint.x)
                            return StepsFromStart + (StartPoint.x - input.StartPoint.x);
                        else
                            return null;
                }

                return null;
            }
        }

        private enum Orientation { Up, Right, Down, Left }
    }
}
