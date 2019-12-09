using AdventOfCode2019.Common;
using System.Linq;

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

            return (int)output.Last();
        }

        [Solution(5, 2)]
        public int Problem2(string input)
        {
            var program = new Intcode(input);
            program.AddInput(5);

            var output = program.RunToEnd();

            return (int)output.Last();
        }
    }
}
