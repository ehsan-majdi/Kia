using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KiaGallery.Common
{
    public class CaptchaImage
    {
        private const string CaptchaText = "_ImageForCap__s";
        public CaptchaImage() { }
        public string Text { get; set; }
        public Bitmap Image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private readonly Random random = new Random();
        public static void RenewCaptcha()
        {
            Random r = new Random();
            HttpContext.Current.Session[CaptchaText] = r.Next(100, 99999).ToString();
        }
        public CaptchaImage(int width, int height)
        {
            Width = width;
            Height = height;
            Text = GenerateRandomCode();
            HttpContext.Current.Session[CaptchaText] = Text;
            GenerateImage();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Image.Dispose();
        }
        private void GenerateImage()
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.Percent20, Color.Green, Color.White);
            g.FillRectangle(hatchBrush, rect);
            //HatchBrush hatchBrush1 = new HatchBrush(HatchStyle.Horizontal, Color.Red, Color.Transparent);
            //g.FillRectangle(hatchBrush1, rect);

            SizeF size;
            float fontSize = rect.Height + 10;
            Font font;

            do
            {
                fontSize--;
                font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
                size = g.MeasureString(Text, font);
            }
            while (size.Width > rect.Width);
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,

            };
            GraphicsPath path = new GraphicsPath();
            path.AddString(Text, font.FontFamily, (int)font.Style, font.Size, rect, format);
            //path.AddString(this.Text, font.FontFamily, (int)font.Style, 75, rect, format);
            const float v = 9F;
            PointF[] points =
              {
                new PointF(random.Next(rect.Width) / v, random.Next(
                   rect.Height) / v),
                new PointF(rect.Width - random.Next(rect.Width) / v,
                    random.Next(rect.Height) / v),
                new PointF(random.Next(rect.Width) / v,
                    rect.Height - random.Next(rect.Height) / v),
                new PointF(rect.Width - random.Next(rect.Width) / v,
                    rect.Height - random.Next(rect.Height) / v)
          };
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            hatchBrush = new HatchBrush(HatchStyle.NarrowHorizontal, Color.FromArgb(0, 0, 255, 0), Color.FromArgb(255, 255, 0, 0));
            g.FillPath(hatchBrush, path);
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = random.Next(rect.Width);
                int y = random.Next(rect.Height);
                int w = random.Next(m / 50);
                int h = random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }
            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();
            Image = bitmap;
        }
        private string GenerateRandomCode()
        {
            Random r = new Random();
            return r.Next(100, 1397).ToString().Replace("2", "7");
        }
        public static bool isValid(string input)
        {
            try
            {
                string CorrectValue = (string)HttpContext.Current.Session[CaptchaText];
                return ((CorrectValue ?? "") == input);
            }
            catch
            {
                return false;
            }
        }
    }
}
