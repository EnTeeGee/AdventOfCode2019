using System;
using System.Linq;

namespace AdventOfCode2019.Common
{
    class Mapper
    {
        public static string[] ToLines(string input)
        {
            return input.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] ToCsvs(string input)
        {
            return input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static int[] ToInts(string input)
        {
            return ToLines(input).Select(it => Convert.ToInt32(it)).ToArray();
        }

        public static int[] ToCsvInts(string input)
        {
            return ToCsvs(input).Select(it => Convert.ToInt32(it)).ToArray();
        }
    }
}
