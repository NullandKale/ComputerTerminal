using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Computer computer = new Computer();
            computer.doConsoleInput();
        }

        static void Test0()
        {
            Computer computer = new Computer();
            computer.doStringInput("1 32 1 1 33 -1 2 32 1 2 33 1 4 0 1 1 5 12");
            computer.use();

            Console.Read();
        }
    }
}
