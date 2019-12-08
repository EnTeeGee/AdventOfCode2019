using AdventOfCode2019.Common;
using System;
using System.Linq;

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

            return Enumerable.Range(0, Height)
                .Select(it => result.Skip(it * Width).Take(Width).Select(isBlack => isBlack ? ' ' : '#').ToArray())
                .Select(it => new string(it))
                .Aggregate(string.Empty, (a, b) => a + b + Environment.NewLine);
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
