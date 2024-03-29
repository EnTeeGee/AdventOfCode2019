﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2019.Common
{
    struct Point
    {
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int ManDistanceTo(Point to)
        {
            return Math.Abs(X - to.X) + Math.Abs(Y - to.Y);
        }

        public Point[] GetSurrounding()
        {
            return new Point[]
            {
                new Point(X, Y - 1),
                new Point(X + 1, Y),
                new Point(X, Y + 1),
                new Point(X - 1, Y)
            };
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;

            return ((Point)obj).X == X && ((Point)obj).Y == Y;
        }

        public override int GetHashCode()
        {
            return X | (Y << 8);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
