using AdventOfCode2019.Common;
using System;
using System.Linq;

namespace AdventOfCode2019.Solutions
{
    class Day02
    {
        [Solution(2, 1)]
        public string Problem1(string input)
        {
            var program = Mapper.ToCsvInts(input);
            var index = 0;
            program[1] = 12;
            program[2] = 2;

            while(true)
            {
                switch(program[index])
                {
                    case 99:
                        return program[0].ToString();
                    case 1:
                        program[program[index + 3]] = program[program[index + 1]] + program[program[index + 2]];
                        break;
                    case 2:
                        program[program[index + 3]] = program[program[index + 1]] * program[program[index + 2]];
                        break;
                    default:
                        Console.WriteLine("Error:");
                        Console.WriteLine(index);
                        Console.WriteLine(String.Join(",", program));
                        throw new Exception("Unexpected opcode");
                }

                index += 4;
            }
        }

        // Ha ha, time for brute force
        [Solution(2, 2)]
        public string Problem2(string input)
        {
            var parsed = Mapper.ToCsvInts(input);
            var target = "19690720";

            for (var i = 0; i < 100; i++)
            {
                for(var j = 0; j < 100; j++)
                {
                    var result = RunWithValues(parsed.ToArray(), i, j);

                    if (result == target)
                        return ((100 * i) + j).ToString();
                }
            }

            throw new Exception("No working pair found");
        }

        private string RunWithValues(int[] input, int noun, int verb)
        {
            var index = 0;
            input[1] = noun;
            input[2] = verb;

            while (true)
            {
                switch (input[index])
                {
                    case 99:
                        return input[0].ToString();
                    case 1:
                        input[input[index + 3]] = input[input[index + 1]] + input[input[index + 2]];
                        break;
                    case 2:
                        input[input[index + 3]] = input[input[index + 1]] * input[input[index + 2]];
                        break;
                    default:
                        Console.WriteLine("Error:");
                        Console.WriteLine(index);
                        Console.WriteLine(String.Join(",", input));
                        throw new Exception("Unexpected opcode");
                }

                index += 4;
            }
        }
    }
}
