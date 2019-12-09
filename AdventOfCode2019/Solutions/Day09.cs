using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day09
    {
        [Solution(9, 1)]
        public long Problem1(string input)
        {
            var program = new Intcode(input);
            program.AddInput(1);

            return program.RunToEnd()[0];
        }

        [Solution(9, 2)]
        public long Problem2(string input)
        {
            var program = new Intcode(input);
            program.AddInput(2);

            return program.RunToEnd()[0];
        }
    }
}
