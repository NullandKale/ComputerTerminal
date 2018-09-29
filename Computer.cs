using StarField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComputerConsole
{
    public class Computer
    {
        private static int height = Console.WindowHeight - 2;
        private static int width = Console.WindowWidth - 1;

        public Cpu mainCPU;
        public Memory mainMemory;

        public Computer()
        {
            mainMemory = new Memory();
            mainCPU = new Cpu(mainMemory);
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

        public void Boot()
        {
            List<string> toPrint = new List<string>(HardwareInfo);
            toPrint.AddRange(mainCPU.HardwareInfo);
            toPrint.AddRange(mainMemory.HardwareInfo);

            slowPrint(toPrint, true);
        }

        public void doConsoleInput()
        {
            string input = Console.ReadLine();

            string[] inputParams = input.Split(' ');

            switch (inputParams[0].ToLower())
            {
                case "run":
                    use();
                    Console.ReadLine();
                    //run = false;
                    break;
                case "load":
                    utilities.load(inputParams[1], mainMemory);
                    break;
                case "edit":
                    if (inputParams.Length > 1)
                    {
                        utilities.edit(inputParams[1]);
                    }
                    break;
                case "assemble":
                    if (inputParams.Length > 1)
                    {
                        utilities.assembleFile(inputParams[1]);
                    }
                    break;
                case "list":
                    utilities.ls();
                    break;
                case "clr":
                    clearScreen();
                    Console.SetCursorPosition(0, 0);
                    break;
                case "help":
                    slowPrint(HelpText, false);
                    break;
                case "exit":
                    game.run = false;
                    break;
                case "look":
                    game.look = true;
                    break;
                case "clear":
                    clearScreen();
                    break;
                default:
                    Console.Write(input + " is not valid try again ");
                    break;
            }
        }

        public List<string> HelpText = new List<string>()
        {
            "help [page number] : display this message",
            "run : Start Cpu @ M0",
            "load [filename] : load bin file",
            "assemble [filename] : assemble .asm file",
            "edit [filename] : edit text files do \"edit help\" for assembly instructions",
            "list : list files",
            "clr : clear console",
            "exit : exit Terminal",
        };

        public List<string> HardwareInfo = new List<string>()
        {
            "NullTerm Loading........",
            "Power Initialized.",
            "Cpu..... Loaded.",
            "Memory.. Loaded.",
            "Display Ready.",
            "Rom Loaded.",
            "Fusion Battery Remaining: 99.763% time left: +999 years",
        };

        public void slowPrint(List<string> toPrint, bool ClearConsole)
        {
            if(ClearConsole)
            {
                clearScreen();
                Console.SetCursorPosition(0, 0);
            }

            int toWait = 250;

            Random r = new Random();

            for(int i = 0; i < toPrint.Count(); i++)
            {
                for(int j = 0; j < toPrint[i].Length; j++)
                {
                    Console.Write(toPrint[i].ToCharArray()[j]);
                    Thread.Sleep(toWait);
                    toWait = toWait - (int)((double)toWait * 0.15);
                }
                Console.Write("\n");
            }

            Console.WriteLine("Input Ready: ");
        }

        private static void clearScreen()
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < height; i++)
            {
                string toWrite = "";

                for (int j = 0; j < width; j++)
                {
                    toWrite += " ";
                }

                Console.WriteLine(toWrite);
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
