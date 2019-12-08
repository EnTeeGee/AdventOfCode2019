using AdventOfCode2019.Common;
using System;
using System.Linq;
using System.Text;

namespace AdventOfCode2019.Solutions
{
    class Day08
    {
        const int Width = 25;
        const int Height = 6;
        const int CharPerLayer = Width * Height;

        [Solution(8, 1)]
        public int Problem1(string input)
        {
            var layers = input.Length / CharPerLayer;

            var targetLayer = Enumerable.Range(0, layers)
                .Select(it => input.Skip(it * CharPerLayer).Take(CharPerLayer))
                .Select(it => it.GroupBy(digit => digit).ToDictionary(digit => digit.Key, digit => digit.Count()))
                .OrderBy(it => it['0'])
                .First();

            return targetLayer['1'] * targetLayer['2'];
        }

        [Solution(8, 2)]
        public string Problem2(string input)
        {
            var layers = input.Length / CharPerLayer;

            var result = Enumerable.Range(0, CharPerLayer)
                .Select(it => Enumerable.Range(0, layers).Select(index => (index * CharPerLayer) + it).ToArray())
                .Select(it => it.Select(index => input[index]).ToArray())
                .Select(it => GetIsBlack(it))
                .ToArray();

            var result1 = Enumerable.Range(0, CharPerLayer).ToList();
            var result2 = result1.Select(it => Enumerable.Range(0, layers).Select(index => (index * CharPerLayer) + it).ToArray()).ToList();
            var result3 = result2.Select(it => it.Select(index => input[index]).ToArray()).ToList();
            var result4 = result3.Select(it => GetIsBlack(it)).ToList();

            return Enumerable.Range(0, Height)
                .Select(it => result.Skip(it * Width).Take(Width).Select(isBlack => isBlack ? '_' : '#').ToArray())
                .Select(it => new string(it))
                .Aggregate(new StringBuilder(), (a, b) => a.Append(b + Environment.NewLine))
                .ToString();
        }

        private bool GetIsBlack(char[] array)
        {
            var indexBlack = Array.IndexOf(array, '0');
            var indexWhite = Array.IndexOf(array, '1');

            if (indexBlack == -1)
                return false;

            if (indexWhite == -1)
                return true;

            return indexBlack < indexWhite;
        }
    }
}
