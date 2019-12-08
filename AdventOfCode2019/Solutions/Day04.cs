using AdventOfCode2019.Common;
using System;
using System.Linq;

namespace AdventOfCode2019.Solutions
{
    class Day04
    {
        [Solution(4, 1)]
        public int Problem1(string input)
        {
            var items = input.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(it => Convert.ToInt32(it)).ToArray();

            var totalMatches = 0;

            for(var i = items[0]; i <= items[1]; i++)
            {
                var str = i.ToString();

                var matches = str.Zip(str.Skip(1), (a, b) => b - a).ToArray();

                if (matches.All(it => it >= 0) && matches.Any(it => it == 0))
                    totalMatches++;
            }

            return totalMatches;
        }

        [Solution(4, 2)]
        public int Problem2(string input)
        {
            var items = input.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(it => Convert.ToInt32(it)).ToArray();

            var totalMatches = 0;

            for (var i = items[0]; i <= items[1]; i++)
            {
                var str = i.ToString();

                var matches = str.Zip(str.Skip(1), (a, b) => b - a).ToArray();

                if (matches.All(it => it >= 0) && str.GroupBy(it => it).Any(it => it.Count() == 2))
                    totalMatches++;
            }

            return totalMatches;
        }

    }
}
