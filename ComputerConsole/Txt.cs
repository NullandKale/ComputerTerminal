using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerConsole
{
    public class Txt
    {
        public string path;
        private string[] lines;

        public Txt(string path)
        {
            if(path == null)
            {
                Console.SetCursorPosition(0, 0);
            }
            this.path = AppDomain.CurrentDomain.BaseDirectory + @"\" + @"\files\" + path;
        }

        public string[] Read()
        {
            if (lines != null)
            {
                return lines;
            }
            else
            {
                if (File.Exists(path))
                {
                    string text = File.ReadAllText(path);
                    lines = text.Split('\n');

                    if (lines.Length > 0)
                    {
                        return lines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public void writeLineToTerm(string line)
        {
            Console.WriteLine(line);
        }

        public void writeLinesToTerm(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(lines[i]);
            }
        }

        public void Write(string line)
        {
            using (var fileStream = File.Create(path))
            {
                using (var fileWriter = new StreamWriter(fileStream))
                {
                    fileWriter.WriteLine(line);
                }
            }
        }

        public void Write(List<string> lines)
        {
            Directory.CreateDirectory(Path.GetDirectoryName((path)));

            using (var fileStream = File.Create(path))
            {
                using (var fileWriter = new StreamWriter(fileStream))
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        fileWriter.WriteLine(lines[i].Trim('\n'));
                    }
                }
            }
        }
    }

}
