using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComputerConsole
{
    public class Cpu
    {
        public static readonly int registerCount = 8;
        public static readonly int hertz = 10;
        public short[] registers;
        public short programCounter;
        public short currentInstruction;
        public bool lastAddZero = false;
        public List<short> instructionCache;

        public Memory mainMemory;

        public List<string> HardwareInfo = new List<string>()
        {
            "Processor Version: v0.0.1a",
            "Instruction Set Version: v0.0.3a",
            "7 Instructions Loaded",
            "Processor Register Count: " + registerCount,
            "Processor Cycle Speed: " + hertz + " hz",
        };

        public Cpu(Memory mainMemory)
        {
            registers = new short[registerCount];
            instructionCache = new List<short>();
            currentInstruction = 0;
            this.mainMemory = mainMemory;
            programCounter = 0;
        }

        public bool cycle(bool debug)
        {
            fetch();
            decode();
            return execute();
        }

        public bool toDebugCycle()
        {
            printCPU();
            if(hertz != -1)
            {
                Thread.Sleep((int)((1.0 / hertz) * 1000.0));
            }
            fetch();
            printCPU();
            if (hertz != -1)
            {
                Thread.Sleep((int)((1.0 / hertz) * 1000.0));
            }
            decode();
            printCPU();
            if (hertz != -1)
            {
                Thread.Sleep((int)((1.0 / hertz) * 1000.0));
            }
            return execute();
        }

        public void printCPU()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("PC: " + programCounter);
            
            for(int i = 0; i < registerCount; i++)
            {
                Console.Write(" R" + i + " " + registers[i]);
            }

            Console.WriteLine();
            mainMemory.printL0(32);
        }

        public void fetch()
        {
            currentInstruction = mainMemory.ReadL0(programCounter);
            programCounter++;
        }
        
        public void decode()
        {
            int toFetch = 0;

            switch ((instructions)currentInstruction)
            {
                case instructions.halt:
                    toFetch = 0;
                    break;
                case instructions.setL0:
                    toFetch = 2;
                    break;
                case instructions.readL0:
                    toFetch = 2;
                    break;
                case instructions.write:
                    toFetch = 2;
                    break;
                case instructions.addR:
                    toFetch = 3;
                    break;
                case instructions.jump:
                    toFetch = 1;
                    break;
                case instructions.jzr:
                    toFetch = 1;
                    break;
                case instructions.jnz:
                    toFetch = 1;
                    break;
            }

            instructionCache.Clear();
            instructionCache.Add(currentInstruction);

            for (int i = 0; i < toFetch; i++)
            {
                instructionCache.Add(mainMemory.ReadL0(programCounter));
                programCounter++;
            }
        }

        public bool execute()
        {
            switch ((instructions)instructionCache[0])
            {
                case instructions.halt:
                    return false;
                case instructions.setL0:
                    mainMemory.SetL0(instructionCache[1], instructionCache[2]);
                    break;
                case instructions.readL0:
                    registers[instructionCache[2]] = mainMemory.ReadL0(instructionCache[1]);
                    break;
                case instructions.write:
                    mainMemory.SetL0(instructionCache[1], registers[instructionCache[2]]);
                    break;
                case instructions.addR:
                    registers[instructionCache[3]] = (short)((int)registers[instructionCache[1]] + (int)registers[instructionCache[2]]);
                    lastAddZero = registers[instructionCache[3]] == 0;
                    break;
                case instructions.jzr:
                    if(lastAddZero)
                    {
                        programCounter = instructionCache[1];
                    }
                    break;
                case instructions.jnz:
                    if (!lastAddZero)
                    {
                        programCounter = instructionCache[1];
                    }
                    break;
                case instructions.jump:
                    programCounter = instructionCache[1];
                    break;
            }

            return true;
        }

        public enum instructions
        {
            // 00 - Halt
            halt = 0,

            // 01 00 00 - Set MemoryLocation Value
            setL0 = 1,

            // 02 00 00 - Read MemoryLocation Register
            readL0 = 2,

            // 03 00 00 - Write MemoryLocation Register
            write = 3,

            // 04 00 00 00 - Add Register Register Register
            addR = 4,

            // 05 00 - Jump Value
            jump = 5,

            // 06 00 - Jump If Last Add Resulted In Zero
            jzr = 6,

            // 07 00 - Jump If Last Add Did not Result in Zero
            jnz = 7,
        }

        public static int instructionsParamsLength(instructions input)
        {
            switch (input)
            {
                case instructions.halt:
                    return 0;
                case instructions.setL0:
                    return 2;
                case instructions.readL0:
                    return 2;
                case instructions.write:
                    return 2;
                case instructions.addR:
                    return 3;
                case instructions.jump:
                    return 1;
                case instructions.jzr:
                    return 1;
                case instructions.jnz:
                    return 1;
                default:
                    return 0;
            }
        }

        public static instructions stringToInstructions(string input)
        {
            switch(input.ToLower())
            {
                case "m_set":
                    return instructions.setL0;
                case "m_read":
                    return instructions.readL0;
                case "m_write":
                    return instructions.write;
                case "add":
                    return instructions.addR;
                case "jmp":
                    return instructions.jump;
                case "jzr":
                    return instructions.jzr;
                case "jnz":
                    return instructions.jnz;
                case "halt":
                    return instructions.halt;
                default:
                    return instructions.halt;
            }
        }
    }
}
