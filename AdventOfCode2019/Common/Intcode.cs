using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.Common
{
    public class Intcode
    {
        private int[] program;
        private int index;
        private bool hasHalted;
        private Queue<int> inputs;
        private List<int> outputs;

        public Intcode(string input)
        {
            program = Mapper.ToCsvInts(input);
            inputs = new Queue<int>();
            outputs = new List<int>();
        }

        public void AddInput(int input)
        {
            inputs.Enqueue(input);
        }

        public int[] RunToEnd()
        {
            while (!hasHalted)
                RunStep();

            return outputs.ToArray();
        }

        private void RunStep()
        {
            if (hasHalted)
                return;

            var instruction = program[index].ToString().PadLeft(5, '0');
            var param1Pos = instruction[2] == '0';
            var param2Pos = instruction[1] == '0';
            var param3Pos = instruction[0] == '0';

            switch (instruction.Substring(3))
            {
                case "01":
                    RunAddition(param1Pos, param2Pos);
                    break;
                case "02":
                    RunMultiply(param1Pos, param2Pos);
                    break;
                case "03":
                    RunRead();
                    break;
                case "04":
                    RunWrite(param1Pos);
                    break;
                case "05":
                    RunJumpIfTrue(param1Pos, param2Pos);
                    break;
                case "06":
                    RunJumpIfFalse(param1Pos, param2Pos);
                    break;
                case "07":
                    RunLessThan(param1Pos, param2Pos);
                    break;
                case "08":
                    RunEquals(param1Pos, param2Pos);
                    break;
                case "99":
                    hasHalted = true;
                    break;
                default:
                    throw new Exception("Unexpected opcode");
            }
        }

        private void RunAddition(bool param1Pos, bool param2Pos)
        {
            var a = GetAtPos(1, param1Pos);
            var b = GetAtPos(2, param2Pos);
            program[program[index + 3]] = a + b;
            index += 4;
        }

        private void RunMultiply(bool param1Pos, bool param2Pos)
        {
            var a = GetAtPos(1, param1Pos);
            var b = GetAtPos(2, param2Pos);
            program[program[index + 3]] = a * b;
            index += 4;
        }

        private void RunRead()
        {
            if (!inputs.Any())
                return;

            program[program[index + 1]] = Convert.ToInt32(inputs.Dequeue());
            index += 2;
        }

        private void RunWrite(bool param1Pos)
        {
            outputs.Add(GetAtPos(1, param1Pos));
            index += 2;
        }

        private void RunJumpIfTrue(bool param1Pos, bool param2Pos)
        {
            var val = GetAtPos(1, param1Pos);
            if (val != 0)
                index = GetAtPos(2, param2Pos);
            else
                index += 3;
        }

        private void RunJumpIfFalse(bool param1Pos, bool param2Pos)
        {
            var val = GetAtPos(1, param1Pos);
            if (val == 0)
                index = GetAtPos(2, param2Pos);
            else
                index += 3;
        }

        private void RunLessThan(bool param1Pos, bool param2Pos)
        {
            var a = GetAtPos(1, param1Pos);
            var b = GetAtPos(2, param2Pos);
            program[program[index + 3]] = (a < b ? 1 : 0);
            index += 4;
        }

        private void RunEquals(bool param1Pos, bool param2Pos)
        {
            var a = GetAtPos(1, param1Pos);
            var b = GetAtPos(2, param2Pos);
            program[program[index + 3]] = (a == b ? 1 : 0);
            index += 4;
        }


        private int GetAtPos(int offset, bool posMode)
        {
            return posMode ? program[program[index + offset]] : program[index + offset];
        }
    }
}
