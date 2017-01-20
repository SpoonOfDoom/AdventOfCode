using System;
using System.Collections.Generic;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day23 : Day
    {
        public Day23() : base(23) { }

        class Instruction
        {
            public enum InstructionType
            {
                hlf,
                tpl,
                inc,
                jmp,
                jie,
                jio
            }

            public InstructionType InstructionName;
            public string Register;
            public int Offset;
        }

        Dictionary<string, int> Registers = new Dictionary<string, int> {{"a", 0}, {"b", 0}};
        List<Instruction> InstructionSet = new List<Instruction>();

        private Instruction ParseLine(string line)
        {
            line = line.Replace(",", "");
            string[] parts = line.Split(' ');

            Instruction.InstructionType instructionType;
            Enum.TryParse(parts[0], out instructionType);
            string register = null;
            int offset = 0;
            switch (instructionType)
            {
                case Instruction.InstructionType.jie:
                case Instruction.InstructionType.jio:
                    register = parts[1];
                    int multi1 = parts[2][0] == '-' ? -1 : 1;
                    offset = parts[2].Substring(1).ToInt() * multi1;
                    break;
                case Instruction.InstructionType.jmp:
                    int multi2 = parts[1][0] == '-' ? -1 : 1;
                    offset = parts[1].Substring(1).ToInt() * multi2;
                    break;

                case Instruction.InstructionType.hlf:
                case Instruction.InstructionType.tpl:
                case Instruction.InstructionType.inc:
                    register = parts[1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Instruction instruction = new Instruction {InstructionName = instructionType, Offset = offset, Register = register};
            return instruction;
        }

        private int ExecuteInstruction(Instruction instruction)
        {
            int moveInstructions = 1;
            switch (instruction.InstructionName)
            {
                case Instruction.InstructionType.hlf:
                    Registers[instruction.Register] /= 2;
                    break;
                case Instruction.InstructionType.tpl:
                    Registers[instruction.Register] *= 3;
                    break;
                case Instruction.InstructionType.inc:
                    Registers[instruction.Register]++;
                    break;
                case Instruction.InstructionType.jmp:
                    moveInstructions = instruction.Offset;
                    break;
                case Instruction.InstructionType.jie:
                    if (Registers[instruction.Register] % 2 == 0)
                    {
                        moveInstructions = instruction.Offset;
                    }
                    break;
                case Instruction.InstructionType.jio:
                    if (Registers[instruction.Register] == 1)
                    {
                        moveInstructions = instruction.Offset;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return moveInstructions;
        }


        public override string GetSolutionPart1()
        {
            foreach (string line in inputLines)
            {
                InstructionSet.Add(ParseLine(line));
            }
            int nextInstruction = 0;

            while (nextInstruction >= 0 && nextInstruction < InstructionSet.Count)
            {
                nextInstruction += ExecuteInstruction(InstructionSet[nextInstruction]);
            }
            return Registers["b"].ToString(); //255
        }

        public override string GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}