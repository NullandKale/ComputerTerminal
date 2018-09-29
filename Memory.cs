using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerConsole
{
    public class Memory
    {
        public static readonly int L0Size = (1024 * 16);
        public static readonly int L1Size = (1024 * 16);
        public short[] L0;
        public short[] L1;

        public Memory()
        {
            L0 = new short[L0Size];
            L1 = new short[L0Size];
        }

        public List<string> HardwareInfo = new List<string>()
        {
            "L0 Size: " + L0Size + " words",
            "L1 Size: " + L1Size + " words",
        };

        public void printL0(int width)
        {
            int height = Console.WindowHeight - 2;
            int counter = 0;

            for (int i = 0; i < height; i++)
            {
                string toWrite = "";

                for (int j = 0; j < width; j++)
                {
                    toWrite += " ";
                    toWrite += L0[counter];
                    counter++;
                }

                Console.WriteLine(toWrite);
            }
        }

        public short ReadL0(short location)
        {
            return L0[location];
        }

        public void SetL0(short location, short value)
        {
            L0[location] = value;
        }
    }
}
