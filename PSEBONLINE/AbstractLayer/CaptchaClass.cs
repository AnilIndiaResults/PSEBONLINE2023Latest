using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;

namespace PSEBONLINE.AbstractLayer
{
    public class CaptchaClass
    {
        private Random rand = new Random();
        public string captchaCode = string.Empty;
        public string captchaImg = string.Empty;

        public CaptchaClass()
        {
            CreateCaptcha();
        }


        private void CreateCaptcha()
        {
            string code = GetRandomText();

            Bitmap bitmap = new Bitmap(200, 30, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);
            Pen pen = new Pen(Color.Black);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 200, 30);

            SolidBrush b = new SolidBrush(Color.YellowGreen);
            SolidBrush blue = new SolidBrush(Color.Black);

            int counter = 0;

            g.DrawRectangle(pen, rect);
            g.FillRectangle(b, rect);

            for (int i = 0; i < code.Length; i++)
            {
                g.DrawString(code[i].ToString(), new Font("Verdena", 7 + rand.Next(10, 12)), blue, new PointF(5 + counter, 0));
                counter += 20;
            }

            DrawRandomLines(g);

            //bitmap.Save(Response.OutputStream, ImageFormat.Gif);

            byte[] bytes = (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[]));
            g.Dispose();
            bitmap.Dispose();
            captchaImg = "data:image/jpg;base64," + Convert.ToBase64String(bytes);

        }

        private void DrawRandomLines(Graphics g)
        {
            SolidBrush green = new SolidBrush(Color.Snow);
            for (int i = 0; i < 20; i++)
            {
                g.DrawLines(new Pen(green, 1), GetRandomPoints());
            }

        }

        private System.Drawing.Point[] GetRandomPoints()
        {
            System.Drawing.Point[] points = { new System.Drawing.Point(rand.Next(10, 150), rand.Next(10, 150)), new System.Drawing.Point(rand.Next(10, 100), rand.Next(10, 100)) };
            return points;
        }

        private string GetRandomText()
        {
            StringBuilder randomText = new StringBuilder();

            if (captchaCode.Trim() == "")
            {
                // string numbers = "ABCDEFGHIIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";                
                string numbers = "0123456789";
                Random r = new Random();
                for (int j = 0; j < 6; j++)
                {
                    randomText.Append(numbers[r.Next(numbers.Length)]);
                }                
                captchaCode = randomText.ToString();
            }
            return randomText.ToString();
        }
    }
}