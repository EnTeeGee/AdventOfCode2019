using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day05
    {
        [Solution(5, 1)]
        public int Problem1(string input)
        {
            var program = new Intcode(input);
            program.AddInput(1);

            var output = program.RunToEnd();

            return output.Last();
        }

        [Solution(5, 2)]
        public int Problem2(string input)
        {
            var program = new Intcode(input);
            program.AddInput(5);

            var output = program.RunToEnd();

            return output.Last();
        }
    }
}
