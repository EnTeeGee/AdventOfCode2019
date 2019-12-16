using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Solutions
{
    class Day16
    {
        [Solution(16, 1)]
        public string Problem1(string input)
        {
            var current = new Queue<int>();

            foreach (var item in input.Trim().Select(it => Convert.ToInt32(it.ToString())))
                current.Enqueue(item);

            var length = current.Count;

            for (var i = 0; i < 100; i++)
            {
                Queue<int> next = new Queue<int>();

                for (var j = 1; j < length + 1; j++)
                {
                    var result = current.Zip(CreatePattern(j).Skip(1), (a, b) => b == 0 ? 0 : a * b).Sum();
                    next.Enqueue(Math.Abs(result) % 10);
                }

                current = next;
                Console.WriteLine(new string(current.Take(80).Select(it => it.ToString()[0]).ToArray()));
            }

            return new string(current.Take(8).Select(it => it.ToString()[0]).ToArray());
        }


        // The targeted area is near the end of the list. Around here, each digit is basically just summing itself onwards for each step.
        [Solution(16, 2)]
        public string Problem2(string input)
        {
            input = input.Trim();
            var startPoint = Convert.ToInt32(new string(input.Take(7).ToArray()));
            var endPoint = input.Length * 10000;
            var current = new int[endPoint - startPoint];
            var initialArray = input.Select(it => Convert.ToInt32(it.ToString())).ToArray();
            var initialLength = input.Length;

            for(var i = endPoint - 1; i >= startPoint; i--)
                current[i - startPoint] = initialArray[i % initialLength];

            for(var i = 0; i < 100; i++)
            {
                var next = new int[current.Length];
                var runningTotal = 0L;

                for(var j = current.Length - 1; j >=0; j--)
                {
                    runningTotal += current[j];
                    next[j] = (int)(runningTotal % 10);
                }

                current = next;
            }

            return new string(current.Take(8).Select(it => it.ToString()[0]).ToArray());
        }

        private IEnumerable<int> CreatePattern(int repeats)
        {
            while (true)
            {
                for (var i = 0; i < repeats; i++)
                    yield return 0;
                for (var i = 0; i < repeats; i++)
                    yield return 1;
                for (var i = 0; i < repeats; i++)
                    yield return 0;
                for (var i = 0; i < repeats; i++)
                    yield return -1;
            }
        }
    }
}
