using AdventOfCode2019.Common;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Solutions
{
    class Day06
    {
        [Solution(6, 1)]
        public int Problem1(string input)
        {
            var items = Mapper.ToLines(input).Select(it => GetInfo(it)).ToArray();
            var com = new Orbit("COM", null);
            Recurse(com, items);

            return com.GetNumOrbits(0);
        }

        [Solution(6, 2)]
        public int Problem2(string input)
        {
            var items = Mapper.ToLines(input).Select(it => GetInfo(it)).ToArray();
            var com = new Orbit("COM", null);
            Recurse(com, items);

            var you = com.Find("YOU");
            var santa = com.Find("SAN");

            var youPath = you.GetPathToCom();
            var santaPath = santa.GetPathToCom();

            while(youPath.Last() == santaPath.Last())
            {
                youPath.Remove(youPath.Last());
                santaPath.Remove(santaPath.Last());
            }

            return youPath.Count + santaPath.Count - 2;
        }

        private (string source, string orbiter) GetInfo(string line)
        {
            var items = line.Split(')');
            return (items[0], items[1]);
        }

        private void Recurse(Orbit source, (string source, string orbiter)[] items)
        {
            var orbiters = items.Where(it => it.source == source.Source).ToList();
            source.Orbiters = orbiters.Select(it => new Orbit(it.orbiter, source)).ToList();
            foreach (var item in source.Orbiters)
                Recurse(item, items);
        }

        private class Orbit
        {
            public Orbit(string source, Orbit parent)
            {
                this.Source = source;
                Orbiters = new List<Orbit>();
                this.Parent = parent;
            }

            public string Source { get; set; }

            public List<Orbit> Orbiters { get; set; }

            public Orbit Parent { get; set; }

            public int GetNumOrbits(int depth)
            {
                return depth + Orbiters.Select(it => it.GetNumOrbits(depth + 1)).Sum();
            }

            public Orbit Find(string target)
            {
                if (Source == target)
                    return this;

                if (!Orbiters.Any())
                    return null;

                return Orbiters.Select(it => it.Find(target)).Where(it => it != null).FirstOrDefault();
            }

            public List<Orbit> GetPathToCom()
            {
                var output = new List<Orbit>();
                output.Add(this);
                var current = this;

                while(current.Parent != null)
                {
                    current = current.Parent;
                    output.Add(current);
                }

                return output;
            }
        }
    }
}
