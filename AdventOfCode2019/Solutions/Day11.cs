using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2019.Solutions
{
    class Day11
    {
        private enum Orientation { North = 0, East = 1, South = 2, West = 3 }

        [Solution(11, 1)]
        public int Problem1(string input)
        {
            var robot = new Intcode(input);
            robot.SetConstInput(0);
            var seenPoints = new Dictionary<Point, bool>();
            var currentPos = new Point(0, 0);
            var currentOrientation = Orientation.North;

            while (!robot.HasHalted)
            {
                var paintColour = robot.RunToOutput();
                var direction = robot.RunToOutput();

                if (paintColour != null)
                {
                    if (!seenPoints.ContainsKey(currentPos))
                        seenPoints.Add(currentPos, paintColour == 1);
                    else
                        seenPoints[currentPos] = paintColour == 1;
                }

                if (direction == null)
                    break;

                currentOrientation = GetNewOrientation(currentOrientation, direction.Value);
                currentPos = GetNewPos(currentPos, currentOrientation);
                if (seenPoints.ContainsKey(currentPos))
                    robot.SetConstInput(seenPoints[currentPos] ? 1 : 0);
                else
                    robot.SetConstInput(0);
            }

            return seenPoints.Count;
        }

        [Solution(11, 2)]
        public string Problem2(string input)
        {
            var robot = new Intcode(input);
            robot.SetConstInput(1);
            var seenPoints = new Dictionary<Point, bool>();
            var currentPos = new Point(0, 0);
            var currentOrientation = Orientation.North;
            seenPoints.Add(currentPos, true);

            while (!robot.HasHalted)
            {
                var paintColour = robot.RunToOutput();
                var direction = robot.RunToOutput();

                if (paintColour != null)
                {
                    if (!seenPoints.ContainsKey(currentPos))
                        seenPoints.Add(currentPos, paintColour == 1);
                    else
                        seenPoints[currentPos] = paintColour == 1;
                }

                if (direction == null)
                    break;

                currentOrientation = GetNewOrientation(currentOrientation, direction.Value);
                currentPos = GetNewPos(currentPos, currentOrientation);
                if (seenPoints.ContainsKey(currentPos))
                    robot.SetConstInput(seenPoints[currentPos] ? 1 : 0);
                else
                    robot.SetConstInput(0);
            }

            var top = seenPoints.Keys.Min(it => it.Y);
            var bottom = seenPoints.Keys.Max(it => it.Y);
            var left = seenPoints.Keys.Min(it => it.X);
            var right = seenPoints.Keys.Max(it => it.X);
            var output = new StringBuilder();

            for(var i = top; i <= bottom; i++)
            {
                for(var j = left; j <= right; j++)
                {
                    var point = new Point(j, i);
                    var item = seenPoints.ContainsKey(point) ? (seenPoints[point] ? '#' : '.') : '.';
                    output.Append(item);
                }

                output.AppendLine();
            }

            return output.ToString();
        }

        private Orientation GetNewOrientation(Orientation current, long fromRobot)
        {
            return (Orientation)((((int)current) + 4 + (fromRobot == 0 ? -1 : 1)) % 4);
        }

        private Point GetNewPos(Point current, Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.North:
                    return new Point(current.X, current.Y - 1);
                case Orientation.East:
                    return new Point(current.X + 1, current.Y);
                case Orientation.South:
                    return new Point(current.X, current.Y + 1);
                case Orientation.West:
                    return new Point(current.X - 1, current.Y);
            }

            throw new Exception("Unepected orientation");
        }
    }
}
