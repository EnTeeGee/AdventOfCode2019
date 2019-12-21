using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day21
    {
        [Solution(21, 1)]
        public long Problem1(string input)
        {
            var commands = new string[]
            {
                "NOT T T",
                "AND A T",
                "AND B T",
                "AND C T",
                "NOT T T",
                "AND D T",
                "OR T J",
                "WALK"
            };

            var robot = new Intcode(input);
            foreach(var item in commands)
            {
                foreach (var letter in item)
                    robot.AddInput(letter);
                robot.AddInput(10);
            }

            var result = robot.RunToEnd();

            if (result.Last() > 128)
                return result.Last();

            var output = new string(result.Select(it => (char)it).ToArray());
            Console.WriteLine(output);

            return 0;
        }

        [Solution(21, 2)]
        public long Problem2(string input)
        {
            var commands = new string[]
            {
                "NOT T T",
                "AND A T",
                "AND B T",
                "AND C T",
                "NOT T T",
                "AND D T",
                "OR T J",
                "AND E T",
                "OR H T",
                // T contains true if already jumping or one of the extra places is available
                "AND T J",
                "RUN"
            };

            var robot = new Intcode(input);
            foreach (var item in commands)
            {
                foreach (var letter in item)
                    robot.AddInput(letter);
                robot.AddInput(10);
            }

            var result = robot.RunToEnd();

            if (result.Last() > 128)
                return result.Last();

            var output = new string(result.Select(it => (char)it).ToArray());
            Console.WriteLine(output);

            return 0;
        }
    }
}
