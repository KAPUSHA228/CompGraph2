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
        short switcher;
        int FrameCount;
        DateTime NextFPSUpdate;
        Bin bin;
        View view;
        int currentLayer = 0;
        int min, width;
        public int getWidth() {  return width; }
        public int getMin() { return min; }
        void displayFPS()
        {
            if (DateTime.Now > NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }
        public Form1()
        {
            InitializeComponent();
            bin = new Bin();
            view = new View(this);
            int X = 2560;
            int Y = 1080;
            int Z = 8;
            int arraySize = X * Y * Z;
            int []array = new int[arraySize];
            Random random = new Random();
            
            for (int i=0; i<arraySize; i++) {
                array[i] = Math.Abs(random.Next());
            }

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

            if (switcher==1)
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
            if(switcher==0)
            {
                if (loaded)
                {
                    view.DrawQuads(currentLayer);
                    glControl1.SwapBuffers();
                }
            }
            else {
                if (loaded)
                {
                    view.DrawQuadStrips(currentLayer);
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
            Console.WriteLine(currentLayer);
            needReload = true;
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            min=trackBar2.Value;
            label1.Text=trackBar2.Value.ToString();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            width = trackBar3.Value;
            label2.Text=trackBar3.Value.ToString();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            switcher = 0;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            switcher = 1;
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            switcher = 2;
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
            trackBar1.Maximum = bin.getZ() - 1;
            
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void glControl1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
