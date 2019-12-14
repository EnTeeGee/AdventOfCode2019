using AdventOfCode2019.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Solutions
{
    class Day14
    {
        [Solution(14, 1)]
        public int Problem1(string input)
        {
            var lines = Mapper.ToLines(input);
            var mappings = new Dictionary<Requirement, Requirement[]>();
            var leftovers = new Dictionary<string, long>();
            foreach(var item in lines)
            {
                var pair = CreateFromLine(item);
                mappings.Add(pair.Item1, pair.Item2);
            }

            return (int)OreForItem("FUEL", 1, mappings, leftovers);
        }

        [Solution(14, 2)]
        public long Problem2(string input)
        {
            var lines = Mapper.ToLines(input);
            var mappings = new Dictionary<Requirement, Requirement[]>();
            var leftovers = new Dictionary<string, long>();
            foreach (var item in lines)
            {
                var pair = CreateFromLine(item);
                mappings.Add(pair.Item1, pair.Item2);
            }

            var trillion = 1000000000000;
            var forOneFuel = OreForItem("FUEL", 1, mappings, leftovers);
            var trialValue = trillion / forOneFuel;
            leftovers = new Dictionary<string, long>();
            var fromTrialValue = OreForItem("FUEL", trialValue, mappings, leftovers);
            var step = 2L;
            while (step < trialValue)
                step *= 2;
            var results = new Dictionary<long, long>();
            results.Add(fromTrialValue, trialValue);
            var lastResult = fromTrialValue;

            while(step > 1)
            {
                step /= 2;
                trialValue += (fromTrialValue > trillion ? -step : step);
                leftovers = new Dictionary<string, long>();
                fromTrialValue = OreForItem("FUEL", trialValue, mappings, leftovers);

                if(fromTrialValue <= trillion)
                    results.Add(fromTrialValue, trialValue);
            }

            var peak = results.Keys.Where(it => it <= trillion).OrderByDescending(it => it).First();

            return results[peak];
        }

        private long OreForItem(string substance, long quantity, Dictionary<Requirement, Requirement[]> mappings, Dictionary<string, long> leftovers)
        {
            var result = mappings.Keys.First(it => it.Substance == substance);
            var ingredients = mappings[result];
            var toReturn = 0L;

            var instancesToRun = (long)Math.Ceiling(quantity / (double)result.Quantity);
            foreach(var item in ingredients)
            {
                var required = instancesToRun * item.Quantity;

                if (item.Substance == "ORE")
                {
                    toReturn += required;
                    continue;
                }

                if (leftovers.ContainsKey(item.Substance))
                {
                    var change = Math.Min(leftovers[item.Substance], required);
                    leftovers[item.Substance] -= change;
                    required -= change;
                }

                toReturn += OreForItem(item.Substance, required, mappings, leftovers);
            }

            if(quantity % result.Quantity != 0)
            {
                var remaining = result.Quantity - (quantity % result.Quantity);

                if (!leftovers.ContainsKey(result.Substance))
                    leftovers.Add(result.Substance, remaining);
                else
                    leftovers[result.Substance] += remaining;
            }

            return toReturn;
        }

        private (Requirement, Requirement[]) CreateFromLine(string input)
        {
            var split = input.Split(new[] { "=>" }, StringSplitOptions.RemoveEmptyEntries).Select(it => it.Trim()).ToArray();
            var result = new Requirement(split[1]);
            var ingredients = split[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(it => new Requirement(it.Trim())).ToArray();

            return (result, ingredients);
        }

        private class Requirement
        {
            public Requirement(string input)
            {
                var items = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                Substance = items[1];
                Quantity = Convert.ToInt32(items[0]);
            }

            public string Substance { get; set; }

            public long Quantity { get; set; }
        }
    }
}
