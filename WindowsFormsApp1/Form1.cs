using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        bool loaded = false;
        bool needReload = false;
        bool switcher;
        int FrameCount;
        DateTime NextFPSUpdate;
        Bin bin;
        View view;
        int currentLayer = 0;

      void displayFPS()
        {
            if (DateTime.Now > NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0}", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }
        public Form1()
        {
            InitializeComponent();
            bin = new Bin();
            view = new View();
            int X = 100;
            int Y = 100;
            int Z = 2;
            int arraySize = X * Y * Z;
            int []array = new int[arraySize];
            Random random = new Random();

            for (int i=0; i<arraySize; i++) {
                array[i] = Math.Abs(random.Next());
            }
            trackBar1.Maximum = bin.getZ();

            using (BinaryWriter writer = new BinaryWriter(File.Open("tomogram.bin", FileMode.Create)))
            {
                writer.Write(X);
                writer.Write(Y);
                writer.Write(Z);
                foreach (short value in array)
                {
                    writer.Write(value);
                }
            }

        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (switcher)
            {
                if (loaded)
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                    glControl1.SwapBuffers();
                }
            }
            else
            {
                if (loaded)
                {
                    view.DrawQuads(currentLayer);
                    glControl1.SwapBuffers();
                }
            }
        }
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            switcher = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            switcher = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string str = ofd.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }
    }
}
