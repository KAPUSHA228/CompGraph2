using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace WindowsFormsApp1
{
    internal class View
    {
        int VBOtexture;
        Bitmap textureImage;
        Form1 form1;
        public View( Form1 form) { form1 = form; }
        public void Load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);
            BitmapData data = textureImage.LockBits(
                new System.Drawing.Rectangle(0, 0, textureImage.Width,
                textureImage.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);
            textureImage.UnlockBits(data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
               (int)TextureMagFilter.Linear);
            ErrorCode er = GL.GetError();
            string str = er.ToString();
        }
        public void generateTextureImage(int layerNumber)
        {
            textureImage = new Bitmap(Bin.X, Bin.Y);
            for (int i = 0; i < Bin.X; ++i)
                for (int j = 0; j < Bin.Y; ++j)
                {
                    int pixelNumber = i + j * Bin.X + layerNumber * Bin.X * Bin.Y;
                    textureImage.SetPixel(i, j, TransferFunction(Bin.array[pixelNumber], form1.getMin(), form1.getWidth()));
                }
        }
        public void DrawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOtexture);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.Y);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.X, Bin.Y);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.X, 0);

            GL.End();
            GL.Disable(EnableCap.Texture2D);

        }
        public void SetupView(int width, int height)
        { 
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }
        public int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        Color TransferFunction(short value, int min, int width)
        {
            int max = width + min; ;
            int newVal = Clamp((value - min) * 255 / (max - min), 0, 255);
            return Color.FromArgb(255, newVal, newVal, newVal);
        }
        public void DrawQuads(int layerNumber)
{
    int[] cop1 = new int[2];  int[]  cop2 = new int[2]; int[] cop3 = new int[2]; int[]  cop4 = new int[2];
    int[] copcop1 = new int[2]; int[] copcop2 = new int[2]; int[] copcop3 = new int[2]; int[] copcop4 = new int[2];
   
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    GL.Begin(BeginMode.Quads);
    for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
        for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
        {
            //начальная точка у которой придётся отрисовывать все 4 точки
            //if ((x_coord == 0) && (y_coord == 0))
           // {
                short value;
                //1 вершина
                value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                cop1[0] = copcop1[0] = x_coord;
                cop1[1] = copcop1[1] = y_coord;
                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
                GL.Vertex2(x_coord, y_coord);
                //2 вершина
                value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                cop2[0] = copcop2[0]= x_coord;
                cop2[1]= copcop2[1]=y_coord;
                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
                GL.Vertex2(x_coord, y_coord + 1);
                //3 вершина
                value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
                cop3[0] = copcop3[0]=x_coord;
                cop3[1]= copcop3[1]=y_coord;
                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
                GL.Vertex2(x_coord + 1, y_coord + 1);
                //4 вершина
                value = Bin.array[x_coord + 1 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                cop4[0] =copcop4[0]= x_coord;
                cop4[1] =copcop4[1]= y_coord;
                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
                GL.Vertex2(x_coord + 1, y_coord);
            //}
            //новая строка, сверху есть что то, дорисовываем  2 точки низ
            //if ((y_coord > 0) && (x_coord == 0)) {
            //    short value;
            //    //1 вершина
            //    value =
            //    //2 вершина
            //    value = Bin.array[copcop1[0] + (copcop1[1] + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
            //    GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
            //    GL.Vertex2(copcop1[0], copcop1[1] + 1);
            //    //3 вершина
            //    value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
            //    GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
            //    GL.Vertex2(x_coord + 1, y_coord + 1);
            //    //4 вершина
            //}
            ////новый столбец, слева есть что то, дорисовываем 2 точки право
            //if ((x_coord > 0) && (y_coord == 0))
            //{
            //    short value;
            //    //3 вершина
            //    value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
            //    GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
            //    GL.Vertex2(x_coord + 1, y_coord + 1);
            //    //4 вершина
            //    value = Bin.array[x_coord + 1 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
            //    GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
            //    GL.Vertex2(x_coord + 1, y_coord);

            //}
            ////где то в центре, достаточно дорисовать 1 точку
            //else {
            //    short value;
            //    //3 вершина
            //    value = Bin.array[x_coord + 1 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
            //    GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
            //    GL.Vertex2(x_coord + 1, y_coord+1);
            //}
        }
    GL.End();
}
        //public void DrawQuads(int layerNumber)
        //{
        //    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        //    GL.Begin(BeginMode.Quads);

        //    bool isStartPoint = true;

        //    for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
        //    {
        //        for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
        //        {
        //            short value;

        //            if (isStartPoint)
        //            {
        //                // Используем все четыре вершины
        //                value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
        //                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
        //                GL.Vertex2(x_coord, y_coord);

        //                value = Bin.array[x_coord + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
        //                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
        //                GL.Vertex2(x_coord, y_coord + 1);

        //                value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
        //                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
        //                GL.Vertex2(x_coord + 1, y_coord + 1);

        //                value = Bin.array[x_coord + 1 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
        //                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
        //                GL.Vertex2(x_coord + 1, y_coord);
        //            }
        //            else
        //            {
        //                // Используем только две вершины для присоединения к предыдущему прямоугольнику
        //                value = Bin.array[x_coord + 1 + (y_coord + 1) * Bin.X + layerNumber * Bin.X * Bin.Y];
        //                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
        //                GL.Vertex2(x_coord + 1, y_coord + 1);

        //                value = Bin.array[x_coord + 1 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
        //                GL.Color3(TransferFunction(value, form1.getMin(), form1.getWidth()));
        //                GL.Vertex2(x_coord + 1, y_coord);
        //            }

        //            isStartPoint = !isStartPoint; // Меняем флаг для следующего прямоугольника
        //        }
        //    }

        //    GL.End();
        //}

    }
}
