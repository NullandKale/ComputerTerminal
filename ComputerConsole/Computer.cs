using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerConsole
{
    public class Computer
    {
        public Cpu mainCPU;
        public Memory mainMemory;

        public Computer(int hertz)
        {
            mainMemory = new Memory();
            mainCPU = new Cpu(mainMemory, hertz);
        }

        private short diPointer = 0;

        public void doStringInput(string input)
        {
            switch (input)
            {
                case "RUN":
                    use();
                    break;
                default:
                    string[] toParse = input.Split(' ');

                    for (int i = 0; i < toParse.Length; i++)
                    {
                        short working;

                        if (short.TryParse(toParse[i], out working))
                        {
                            mainMemory.SetL0(diPointer, working);
                            diPointer++;
                        }
                    }

                    break;
            }
        }

        public void doDirectInput()
        { 
            doStringInput(Console.ReadLine());
        }

        public void use()
        {
            bool run = true;
            while(run)
            {
                run = mainCPU.toDebugCycle();
            }

            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write("HALTED");
        }
    }
}
