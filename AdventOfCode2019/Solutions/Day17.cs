using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day17
    {
        [Solution(17, 1)]
        public int Problem1(string input)
        {
            var camera = new Intcode(input);

            var data = camera.RunToEnd();
            var scaffold = new HashSet<Point>();
            var x = 0;
            var y = 0;

            foreach(var item in data)
            {
                if(item == 10)
                {
                    x = 0;
                    y++;
                    continue;
                }

                if (item == 35)
                    scaffold.Add(new Point(x, y));

                x++;
            }

            var junctions = scaffold.Where(it => it.GetSurrounding().All(s => scaffold.Contains(s))).ToArray();
            var alignmentParameter = junctions.Select(it => it.X * it.Y).Sum();

            return alignmentParameter;
        }

        private enum Direction { North, East, South, West }

        // not 46
        [Solution(17, 2)]
        public long Problem2(string input)
        {
            var camera = new Intcode(input);
            var data = camera.RunToEnd();
            var scaffold = new HashSet<Point>();
            var robot = new Point(0, 0);
            var direction = Direction.North;
            var x = 0;
            var y = 0;

            foreach (var item in data)
            {
                if (item == 10)
                {
                    x = 0;
                    y++;
                    continue;
                }

                if (item == 35)
                    scaffold.Add(new Point(x, y));
                else if (item != 46)
                {
                    robot = new Point(x, y);
                    direction = GetDirection(item);
                }

                x++;
            }

            var path = MapPath(scaffold, robot, direction);

            //OVERRIDE: steps used being calculated manually
            //var remainingPath = path.ToArray();
            //var seqA = GetBestRepeatedSequence(remainingPath);
            //MarkInstancesOf(remainingPath, seqA, "A");
            //RenderSteps(remainingPath);
            //var seqB = GetBestRepeatedSequence(remainingPath);
            //MarkInstancesOf(remainingPath, seqB, "B");
            //RenderSteps(remainingPath);
            //var seqC = GetBestRepeatedSequence(remainingPath);
            //MarkInstancesOf(remainingPath, seqC, "C");
            //RenderSteps(remainingPath);

            var remainingPath = path.ToArray();
            var seqA = new string[] { "R8", "L4", "R4", "R10", "R8" }.Select(it => new TurnAndStep(it)).ToArray();
            MarkInstancesOf(remainingPath, seqA, "A");
            var seqB = new string[] { "R10", "R4", "R4" }.Select(it => new TurnAndStep(it)).ToArray();
            MarkInstancesOf(remainingPath, seqB, "B");
            var seqC = new string[] { "L12", "L12", "R8", "R8" }.Select(it => new TurnAndStep(it)).ToArray();
            MarkInstancesOf(remainingPath, seqC, "C");

            var letterCodes = GetLetterCodes(remainingPath, seqA.Length, seqB.Length, seqC.Length);
            var funcA = GetFunctionDef(seqA);
            var funcB = GetFunctionDef(seqB);
            var funcC = GetFunctionDef(seqC);

            var program = new Intcode(input);
            program.OverwriteAddress(0, 2);
            foreach (var item in letterCodes)
                program.AddInput(item);
            program.AddInput(10);
            foreach (var item in funcA)
                program.AddInput(item);
            program.AddInput(10);
            foreach (var item in funcB)
                program.AddInput(item);
            program.AddInput(10);
            foreach (var item in funcC)
                program.AddInput(item);
            program.AddInput(10);
            program.AddInput('n');
            program.AddInput(10);

            var result = program.RunToEnd();




            //Render(data);

            //return result.Value;
            return result.Last();
        }

        private Direction GetDirection(long input)
        {
            switch (input)
            {
                case '^':
                    return Direction.North;
                case '>':
                    return Direction.East;
                case 'v':
                    return Direction.South;
                case '<':
                    return Direction.West;
                default:
                    throw new Exception("Unknown direction or character");
            }
        }

        private Direction Left(Direction input)
        {
            return (Direction)(((int)input + 3) % 4);
        }

        private Direction Right(Direction input)
        {
            return (Direction)(((int)input + 1) % 4);
        }

        private Point GetPointInDirection(Point point, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(point.X, point.Y - 1);
                case Direction.East:
                    return new Point(point.X + 1, point.Y);
                case Direction.South:
                    return new Point(point.X, point.Y + 1);
                case Direction.West:
                    return new Point(point.X - 1, point.Y);
                default:
                    throw new Exception("Unknown direction");
            }
        }

        private (Point, Point) GetLeftAndRightPoints(Point point, Direction direction)
        {
            var directions = new Direction[] { Left(direction), Right(direction) };

            return (GetPointInDirection(point, directions[0]), GetPointInDirection(point, directions[1]));
        }

        private TurnAndStep[] MapPath(HashSet<Point> scaffold, Point robot, Direction robotDir)
        {
            var currentPos = robot;
            var currentDir = robotDir;
            var output = new List<TurnAndStep>();
            var current = (TurnAndStep)null;

            while (true)
            {
                var sides = GetLeftAndRightPoints(currentPos, currentDir);
                if (!scaffold.Contains(sides.Item1) && !scaffold.Contains(sides.Item2))
                    return output.ToArray();

                current = new TurnAndStep();
                current.Turn = scaffold.Contains(sides.Item1) ? "L" : "R";
                currentDir = current.Turn == "L" ? Left(currentDir) : Right(currentDir);

                var steps = 0;
                while(scaffold.Contains(GetPointInDirection(currentPos, currentDir)))
                {
                    steps++;
                    currentPos = GetPointInDirection(currentPos, currentDir);
                }

                current.Step = steps;
                output.Add(current);
            }
        }

        private TurnAndStep[] GetBestRepeatedSequence(TurnAndStep[] input)
        {
            var length = 10; // Max possible length
            var bestSoFar = new TurnAndStep[0];
            var bestSoFarRemoves = 0;

            for(var i = length; i > 0; i--)
            {
                for(var j = 0; j < input.Length - i; j++)
                {
                    var sequence = input.Skip(j).Take(i).ToArray();
                    var testLength = GetFunctionDef(sequence).Length;
                    if (testLength > 20)
                        continue;

                    var resultingArray = RemoveInstancesOf(input, sequence);
                    if(input.Length - resultingArray.Length > bestSoFarRemoves)
                    {
                        bestSoFarRemoves = input.Length - resultingArray.Length;
                        bestSoFar = sequence;
                    }
                }
            }

            return bestSoFar;
        }

        private TurnAndStep[] RemoveInstancesOf(TurnAndStep[] input, TurnAndStep[] subArray)
        {
            var currentIndex = 0;
            var remainingArray = input.ToArray();

            while(currentIndex + subArray.Length <= remainingArray.Length)
            {
                if (subArray.Zip(remainingArray.Skip(currentIndex), (a, b) => a.IsSame(b)).All(it => it))
                    remainingArray = remainingArray.Take(currentIndex).Concat(remainingArray.Skip(currentIndex + subArray.Length)).ToArray();
                else
                    currentIndex++;
            }

            return remainingArray;
        }

        private void MarkInstancesOf(TurnAndStep[] input, TurnAndStep[] subArray, string letter)
        {
            var currentIndex = 0;

            while (currentIndex + subArray.Length <= input.Length)
            {
                if (subArray.Zip(input.Skip(currentIndex), (a, b) => a.IsSame(b)).All(it => it))
                {
                    foreach (var item in input.Skip(currentIndex).Take(subArray.Length))
                        item.Mark = letter;

                    currentIndex += subArray.Length - 1;
                }
                else
                    currentIndex++;
            }
        }

        private string GetLetterCodes(TurnAndStep[] input, int lengthA, int lengthB, int lengthC)
        {
            var output = new List<string>();
            var remaining = input.ToArray();

            while (remaining.Any())
            {
                if (remaining[0].Mark == "A")
                {
                    output.Add("A");
                    remaining = remaining.Skip(lengthA).ToArray();
                }
                else if (remaining[0].Mark == "B")
                {
                    output.Add("B");
                    remaining = remaining.Skip(lengthB).ToArray();
                }
                else if (remaining[0].Mark == "C")
                {
                    output.Add("C");
                    remaining = remaining.Skip(lengthC).ToArray();
                }
                else
                    throw new Exception("Unexpected mark");
            }

            return string.Join(",", output.ToArray());
        }

        private string GetFunctionDef(TurnAndStep[] input)
        {
            var steps = input.Select(it => $"{it.Turn},{it.Step}");

            return string.Join(",", steps);
        }


        private void Render(long[] data)
        {
            var lineLength = Array.IndexOf(data, 10);
            var rowNumber = data.Where(it => it == 10).Count() - 1;

            var view = new long[lineLength, rowNumber];

            for (var i = 0; i < rowNumber; i++)
            {
                for (var j = 0; j < lineLength; j++)
                {
                    view[j, i] = data[(i * (lineLength + 1)) + j];
                }

                var str = new string(data.Skip(i * (lineLength + 1)).Take(lineLength).Select(it => ConvertToChar(it)).ToArray());
                Console.WriteLine(str);
            }
        }

        private char ConvertToChar(long input)
        {
            if (input == 35)
                return '#';
            else if (input == 46)
                return '.';
            else
                return (char)input;
        }

        private void RenderSteps(TurnAndStep[] input)
        {
            StringBuilder output = new StringBuilder();
            foreach (var item in input)
                output.Append($"{item.Turn},{item.Step.ToString("D2")} ");
            Console.WriteLine(output.ToString());
            output = new StringBuilder();
            foreach (var item in input)
                output.Append($"  {item.Mark ?? " "}  ");
            Console.WriteLine(output.ToString());
            Console.WriteLine();
        }

        private class TurnAndStep
        {
            public TurnAndStep()
            {

            }

            public TurnAndStep(string input)
            {
                Turn = input[0].ToString();
                Step = Convert.ToInt32(input.Substring(1));
            }

            public string Turn { get; set; }

            public int Step { get; set; }

            public string Mark { get; set; }

            public bool IsSame(TurnAndStep input)
            {
                return Turn == input.Turn && Step == input.Step && input.Mark == null;
            }

            public override string ToString()
            {
                return $"{Turn},{Step}{(Mark == null ? null : " (" + Mark + ")")}";
            }
        }
    }
}
