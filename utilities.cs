using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerConsole
{
    public class utilities
    {
        public static readonly List<string> helpTXT = new List<string>()
        {
            "$Variable // a storage location allocated for you starting at the bottom of memory",
            "@GoTo // a location saved for a jmp",
            "m_set $Variable (RAW_NUMBER) // set memory @ $Variable to (RAW_NUMBER -(2^8) - (2^8-1))",
            "m_read $Variable [REGISTER_NUMBER] // set [REGISTER_NUMBER] to value in $Variable",
            "m_write $Variable [REGISTER_NUMBER] // set $Variable to value in [REGISTER]",
            "add [REGISTER_NUMBER] [REGISTER_NUMBER] [REGISTER_NUMBER] // add [REGISRER] to [REGISTER] and store in [REGISTER]",
            "jmp @Goto // jmp to @Goto",
            "jzr @Goto // jmp if last add was equal to 0",
            "jnz @Goto // jmp if last add was not 0",
        };

        public static void ls()
        {
            string rootDir = AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\files\";
            DirectoryInfo d = new DirectoryInfo(rootDir);
            FileInfo[] Files = new FileInfo[0];

            if (d.Exists)
            {
                Files = d.GetFiles("*.*");
            }

            foreach (FileInfo file in Files)
            {
                if (File.GetAttributes(file.Directory.FullName).HasFlag(FileAttributes.Directory))
                {
                    //FileInfo[] innerFiles = file.Directory.GetFiles("*.*");
                    //for (int i = 0; i < innerFiles.Length; i++)
                    //{
                    //    Console.Write(" |-- " + file.Name + "/" + innerFiles[i].Name);
                    //}
                    Console.WriteLine("" + file.Name);
                }
                else
                {
                    Console.WriteLine("" + file.Name);
                }
            }
        }

        public static void load(string fileName, Memory toLoad)
        {
            Txt txtFile = new Txt(fileName);

            string[] lines = txtFile.Read();

            if(lines != null)
            {
                int counter = 0;

                for(int i = 0; i < lines.Length; i++)
                {
                    int parsed = stringToInt(lines[i]);
                    toLoad.SetL0((short)counter, (short)parsed);
                    counter++;
                }

                Console.WriteLine("Loaded " + counter + " words.");
                Console.WriteLine("Enter run to start loaded program.");
            
            }
        }

        private static Txt asmFile;
        private static List<string> asmLines;
        private static string asmFileName;
        private static Dictionary<string, symbol> symbolTable;
        private static Dictionary<string, symbol> goToTable;

        public static void assembleFile(string param0)
        {
            asmFile = new Txt(param0);
            asmFileName = param0;
            symbolTable = new Dictionary<string, symbol>();
            goToTable = new Dictionary<string, symbol>();

            string[] fileLines = asmFile.Read();

            if (fileLines != null)
            {
                asmLines = new List<string>();
                for (int i = 0; i < fileLines.Length; i++)
                {
                    asmLines.Add(fileLines[i]);
                }
            }
            else
            {
                return;
            }

            Txt output = null;

            if (param0.Contains("."))
            {
               output = new Txt(param0.Substring(0,param0.IndexOf('.')) + ".bin");
            }
            else
            {
                output = new Txt(param0 + ".bin");
            }


            List<int> toWrite = new List<int>();

            for (int i = 0; i < fileLines.Length; i++)
            {
                string[] line = fileLines[i].Split(' ');

                if (line.Length > 0)
                {
                    if (line[0][0] == '$')
                    {
                        symbolTable.Add(line[0].Trim(), new symbol(line[0].Substring(1)));
                    }
                    else if (line[0][0] == '@')
                    {
                        goToTable.Add(line[0].Trim(), new symbol(line[0].Substring(1), toWrite.Count));
                    }
                    else
                    {
                        Cpu.instructions current = Cpu.stringToInstructions(line[0]);
                        int toGrab = Cpu.instructionsParamsLength(current);

                        toWrite.Add((int)current);

                        if (line.Length == toGrab + 1)
                        {
                            for (int j = 1; j <= toGrab; j++)
                            {
                                if (current == Cpu.instructions.setL0 || current == Cpu.instructions.readL0 || current == Cpu.instructions.write)
                                {
                                    if (j == 1)
                                    {
                                        int pointer = getSymbolPointer(line[j]);
                                        if (pointer != -1)
                                        {
                                            toWrite.Add(pointer);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error @" + i + " line = " + line[0] + " " + line[1]);
                                            Console.WriteLine("Problem Loading Symbol.");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        int parsed = stringToInt(line[j]);
                                        toWrite.Add(stringToInt(line[j]));
                                    }
                                }
                                else if (current == Cpu.instructions.jump || current == Cpu.instructions.jzr || current == Cpu.instructions.jnz)
                                {
                                    int pointer = getGoToPointer(line[1]);
                                    if (pointer != -1)
                                    {
                                        toWrite.Add(pointer);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error @" + i + " line = " + line[0] + " " + line[1]);
                                        Console.WriteLine("Problem Loading GoTo.");
                                        return;
                                    }
                                }
                                else
                                {
                                    int parsed = stringToInt(line[j]);
                                    if (parsed != -1)
                                    {
                                        toWrite.Add(parsed);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error @" + i + " line[0] = " + line[0]);
                                        Console.WriteLine("Problem Parsing Line");
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error @" + i + " line[0] = " + line[0]);
                            Console.WriteLine("No Enough Instruction Params For: " + current);
                            return;
                        }
                    }
                }
            }

            if (toWrite.Count < symbol.tablePointer)
            {
                List<string> outputLines = new List<string>();
                for (int i = 0; i < toWrite.Count; i++)
                {
                    outputLines.Add(toWrite[i] + "");
                }

                output.Write(outputLines);
            }
            else
            {
                Console.WriteLine("Error Program Size > symbolTablePointer");
                Console.WriteLine("Program Size: " + toWrite.Count);
                return;
            }

        }

        private static int getGoToPointer(string name)
        {
            if (goToTable.ContainsKey(name.Trim()))
            {
                return goToTable[name.Trim()].pointer;
            }
            else
            {
                return -1;
            }
        }

        private static int getSymbolPointer(string name)
        {
            if (symbolTable.ContainsKey(name.Trim()))
            {
                return symbolTable[name].pointer;
            }
            else
            {
                return -1;
            }
        }

        private static int stringToInt(string input)
        {
            int lineNumber = -1;

            if (!int.TryParse(input, out lineNumber))
            {
                return -1;
            }
            else
            {
                return lineNumber;
            }
        }

        private struct symbol
        {
            public static int tablePointer = Memory.L0Size - 1;
            public string name;
            public int pointer;

            public symbol(string name)
            {
                this.name = name;
                pointer = tablePointer;
                tablePointer--;
            }

            public symbol(string name, int pointer)
            {
                this.name = name;
                this.pointer = pointer;
            }
        }

        private static Txt file;
        private static List<string> lines;
        private static string fileName;
        private static int scrollPos;
        private static int height = Console.WindowHeight - 2;
        private static int width = Console.WindowWidth - 1;

        public static void edit(string param0)
        {
            file = new Txt(param0);
            fileName = param0;

            string[] fileLines = file.Read();

            if (fileLines == null)
            {
                lines = new List<string>();
            }
            else
            {
                lines = new List<string>();
                for (int i = 0; i < fileLines.Length; i++)
                {
                    lines.Add(fileLines[i]);
                }
            }

            if (param0 == "help")
            {
                lines = helpTXT;
            }

            scrollPos = 0;


            bool run = true;

            while (run)
            {
                printLines();

                string toProcess = Console.ReadLine();

                string[] sections = toProcess.Split(' ');

                switch (sections[0].ToLower())
                {
                    case "save":
                        file.Write(lines);
                        break;
                    case "del":
                        lines.RemoveAt(scrollPos);
                        break;
                    case "g":
                        int lineNumber = -1;

                        if (int.TryParse(sections[1], out lineNumber))
                        {
                            if (lines.Count() < lineNumber)
                            {
                                scrollPos = lines.Count();
                            }
                            else
                            {
                                scrollPos = lineNumber;
                            }
                        }

                        break;
                    case "u":
                        if (scrollPos > 0)
                        {
                            scrollPos--;
                        }
                        break;
                    case "clr":
                        if (lines.Count > scrollPos)
                        {
                            lines[scrollPos] = string.Empty;
                        }
                        break;
                    case "d":
                        if (scrollPos < lines.Count - 1)
                        {
                            scrollPos++;
                        }
                        break;
                    case "add":
                        List<string> newLines = new List<string>();
                        for(int i = 0; i < lines.Count; i++)
                        {
                            if(i == scrollPos)
                            {
                                newLines.Add("");
                            }
                            newLines.Add(lines[i]);
                        }

                        lines = newLines;
                        break;
                    case "exit":
                        run = false;
                        break;
                    default:
                        if (lines.Count() > scrollPos)
                        {
                            lines[scrollPos] = toProcess;
                        }
                        else
                        {
                            lines.Add(toProcess);
                        }
                        scrollPos++;
                        break;
                }
            }
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

        private static void printLines()
        {
            clearScreen();
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < height; i++)
            {
                if (i == scrollPos)
                {
                    string toWrite = i.ToString("00") + " : ";
                    if (lines.Count > i)
                    {
                        toWrite += lines[i];
                    }

                    Console.WriteLine(toWrite);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(i.ToString("00") + " : ");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    string toWrite = i.ToString("00") + " : ";
                    if (lines.Count > i)
                    {
                        toWrite += lines[i];
                    }

                    Console.WriteLine(toWrite);
                }
            }
            Console.SetCursorPosition(4, scrollPos + 1);
        }
    }
}
