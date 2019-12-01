using AdventOfCode2019.Common;
using System;
using System.Linq;

namespace AdventOfCode2019.Solutions
{
    class Day01
    {
        [Solution(1, 1)]
        public string Solution1(string input)
        {
            return Mapper.ToInts(input).Select(it => ConvertValue(it)).Sum().ToString();
        }

        private int ConvertValue(int input)
        {
            return (input / 3) - 2;
        }


        [Solution(1, 2)]
        public string Solution2(string input)
        {
            return Mapper.ToInts(input).Select(it => CalcForModule(it)).Sum().ToString();
        }

        private int CalcForModule(int input)
        {
            var result = Math.Max(0, (input / 3) - 2);

            return result == 0 ? result : result + CalcForModule(result);
        }
    }
}
