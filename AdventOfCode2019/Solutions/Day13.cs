using AdventOfCode2019.Common;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Solutions
{
    class Day13
    {
        [Solution(13, 1)]
        public int Problem1(string input)
        {
            var program = new Intcode(input);
            var squareInfo = program.RunToEnd();

            var total = 0;
            for(var i = 2; i < squareInfo.Length; i += 3)
            {
                if (squareInfo[i] == 2)
                    total++;
            }

            return total;
        }

        [Solution(13, 2)]
        public int Problem2(string input)
        {
            var program = new Intcode(input);
            program.OverwriteAddress(0, 2);
            var blocks = new HashSet<Point>();
            var ball = new Point();
            var paddle = new Point();
            var score = 0;

            do
            {
                var info = program.RunToEnd();
                for(var i = 0; i < info.Length; i += 3)
                {
                    var point = new Point((int)info[i], (int)info[i + 1]);
                    if(point.Equals(new Point(-1, 0)))
                    {
                        score = (int)info[i + 2];
                        continue;
                    }

                    switch(info[i + 2])
                    {
                        case 0:
                            if (blocks.Contains(point))
                                blocks.Remove(point);
                            break;
                        case 2:
                            if (!blocks.Contains(point))
                                blocks.Add(point);
                            break;
                        case 3:
                            paddle = point;
                            break;
                        case 4:
                            ball = point;
                            break;
                    }
                }

                var direction = ball.X > paddle.X ? 1 : ball.X < paddle.X ? -1 : 0;
                program.AddInput(direction);

            } while (blocks.Any());

            return score;
        }
    }
}
