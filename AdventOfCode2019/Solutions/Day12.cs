using AdventOfCode2019.Common;
using System;
using System.Linq;

namespace AdventOfCode2019.Solutions
{
    class Day12
    {
        [Solution(12, 1)]
        public int Problem1(string input)
        {
            var lines = Mapper.ToLines(input);
            var moons = lines.Select(it => new Moon(it)).ToArray();
            foreach(var item in moons)
                item.OtherMoons = moons.Where(it => it != item).ToArray();

            for(var i = 0; i < 1000; i++)
            {
                foreach (var item in moons)
                    item.StepVelocity();
                foreach (var item in moons)
                    item.StepPos();
            }

            return moons.Sum(it => it.GetEnergy());
        }

        [Solution(12, 2)]
        public long Problem2(string input)
        {
            var lines = Mapper.ToLines(input);
            var moons = lines.Select(it => new Moon(it)).ToArray();
            var xSteps = (long)FindTimeUntilRepeat(moons.Select(it => it.Pos.X).ToArray(), new int[4]);
            var ySteps = (long)FindTimeUntilRepeat(moons.Select(it => it.Pos.Y).ToArray(), new int[4]);
            var zSteps = (long)FindTimeUntilRepeat(moons.Select(it => it.Pos.Z).ToArray(), new int[4]);

            var lcm = (xSteps * ySteps) / GetGcd(xSteps, ySteps);
            lcm = (lcm * zSteps) / GetGcd(lcm, zSteps);

            return lcm;
        }

        private int FindTimeUntilRepeat(int[] posArray, int[] velocityArray)
        {
            var inititalPos = posArray.ToArray();
            var initialVelocity = velocityArray.ToArray();

            var currentStep = 0;

            do
            {
                for (var i = 0; i < velocityArray.Length; i++)
                {
                    for (var j = 0; j < posArray.Length; j++)
                    {
                        if (i == j)
                            continue;

                        velocityArray[i] += Moon.GetChange(posArray[i], posArray[j]);
                    }
                }

                for (var i = 0; i < posArray.Length; i++)
                    posArray[i] += velocityArray[i];

                currentStep++;

            } while (!AreSame(posArray, inititalPos) || !AreSame(velocityArray, initialVelocity));

            return currentStep;
        }

        private bool AreSame(int[] first, int[] second)
        {
            return first.Zip(second, (a, b) => a == b).All(it => it);
        }

        private long GetGcd(long a, long b)
        {
            var larger = Math.Max(a, b);
            var smaller = Math.Min(a, b);

            while(smaller != 0)
            {
                var result = larger % smaller;
                larger = smaller;
                smaller = result;
            }

            return larger;
        }

        private class Moon
        {
            public Vector3 Velocity { get; }

            public Vector3 Pos { get; }

            public Moon[] OtherMoons { get; set; }

            public Moon(string input)
            {
                var values = input.Split(new[] { '<', '>', 'x', 'y', 'z', '=', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Velocity = new Vector3();
                Pos = new Vector3();
                Pos.X = Convert.ToInt32(values[0]);
                Pos.Y = Convert.ToInt32(values[1]);
                Pos.Z = Convert.ToInt32(values[2]);
            }

            public void StepVelocity()
            {
                foreach(var item in OtherMoons)
                {
                    Velocity.X += GetChange(Pos.X, item.Pos.X);
                    Velocity.Y += GetChange(Pos.Y, item.Pos.Y);
                    Velocity.Z += GetChange(Pos.Z, item.Pos.Z);
                }
            }

            public void StepPos()
            {
                Pos.X += Velocity.X;
                Pos.Y += Velocity.Y;
                Pos.Z += Velocity.Z;
            }

            public int GetEnergy()
            {
                var pot = Math.Abs(Pos.X) + Math.Abs(Pos.Y) + Math.Abs(Pos.Z);
                var kin = Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);

                return pot * kin;
            }

            public static int GetChange(int current, int target)
            {
                if (current < target)
                    return 1;
                else if (current > target)
                    return -1;

                return 0;
            }
        }

        private class Vector3
        {
            public int X { get; set; }

            public int Y { get; set; }

            public int Z { get; set; }
        }
    }
}
