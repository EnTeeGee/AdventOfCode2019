﻿using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode2019.Solutions
{
    class Day19
    {
        [Solution(19, 1)]
        public int Problem1B(string input)
        {
            var found = new HashSet<Point>();

            for(var i = 0; i < 50; i++)
            {
                for (var j = 0; j < 50; j++)
                {
                    var robot = new Intcode(input);
                    robot.AddInput(i);
                    robot.AddInput(j);
                    var value = robot.RunToOutput();

                    if (value == 1)
                        found.Add(new Point(i, j));
                }
            }

            for (var i = 0; i < 50; i++)
            {
                var items = new char[50];

                for (var j = 0; j < 50; j++)
                    items[j] = found.Contains(new Point(j, i)) ? '#' : '.';

                Console.WriteLine(new string(items));
            }

            return found.Count;
        }

        [Solution(19, 2)]
        public long Problem2(string input)
        {
            var inputCsv = Mapper.ToCsvInts(input);
            // Hardcoded values from image generated by previous solution
            var lowerPoint = new Point(29, 49);
            var upperPoint = new Point(29, 40);

            while(upperPoint.X - lowerPoint.X < 99)
            {
                lowerPoint.Y += 1;
                var robot = new Intcode(inputCsv);
                robot.AddInput(lowerPoint.X);
                robot.AddInput(lowerPoint.Y);
                var result = robot.RunToOutput();
                if (result == 0)
                    lowerPoint.X += 1;

                upperPoint.X += 1;
                if(lowerPoint.Y - upperPoint.Y >= 100)
                    upperPoint.Y += 1;
                robot = new Intcode(inputCsv);
                robot.AddInput(upperPoint.X);
                robot.AddInput(upperPoint.Y);
                result = robot.RunToOutput();
                if (result == 0)
                    upperPoint.X -= 1;
            }

            var closest = new Point(lowerPoint.X, upperPoint.Y);
            return (closest.X * 10000L) + closest.Y;
        }
    }
}
