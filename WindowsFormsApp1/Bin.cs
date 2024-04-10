using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
namespace WindowsFormsApp1
{
    internal class Bin
    {
        public static int X, Y, Z;
        public static short[] array;
        public int getZ() { return Z; }
        public Bin() { }
        public void readBIN(string path)
        {
            if (File.Exists(path))
            {
                int sum = 0;
                BinaryReader reader =
                    new BinaryReader(File.Open(path, FileMode.Open));
                X = reader.ReadInt32();
                Y = reader.ReadInt32();
                Z = reader.ReadInt32();
                int arraySize = X * Y * Z;
                array = new short[arraySize];
                for (int i = 0; i < arraySize; i++)
                {
                    array[i] = reader.ReadInt16();
                }
                for(int i=0;i<arraySize; i++) { sum += array[i]; }
                Console.WriteLine("Sum of array: "+sum);
            }

        }
    }
}
