using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PetRock
{
    public class LayeredNotificationForm : Form
    {
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize,
            IntPtr hdcSrc, ref POINT pptSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll", ExactSpelling = true)]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll", ExactSpelling = true)]
        static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int x; public int y; }
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE { public int cx; public int cy; }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }
        const int ULW_ALPHA = 0x00000002;
        const byte AC_SRC_OVER = 0x00;
        const byte AC_SRC_ALPHA = 0x01;

        private Bitmap bitmap;

        public LayeredNotificationForm(string message)
        {
            // Set up a borderless, layered window.
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;

            // Create a bitmap with per-pixel alpha containing the text.
            bitmap = CreateTextBitmap(message);
            this.Size = bitmap.Size;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cp;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetBitmap(bitmap);
        }

        private Bitmap CreateTextBitmap(string message)
        {
            Font font = new Font("Segoe Script", 24, FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF textSize;
            using (Bitmap temp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(temp))
            {
                textSize = g.MeasureString(message, font);
            }
            Bitmap bmp = new Bitmap((int)Math.Ceiling(textSize.Width), (int)Math.Ceiling(textSize.Height), PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    g.DrawString(message, font, brush, new PointF(0, 0));
                }
            }
            return bmp;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            IntPtr screenDC = GetDC(IntPtr.Zero);
            IntPtr memDC = CreateCompatibleDC(screenDC);
            IntPtr hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
            IntPtr oldBitmap = SelectObject(memDC, hBitmap);

            POINT topPos = new POINT { x = this.Left, y = this.Top };
            SIZE size = new SIZE { cx = bitmap.Width, cy = bitmap.Height };
            POINT pointSource = new POINT { x = 0, y = 0 };

            BLENDFUNCTION blend = new BLENDFUNCTION();
            blend.BlendOp = AC_SRC_OVER;
            blend.BlendFlags = 0;
            blend.SourceConstantAlpha = 255; // Fully opaque
            blend.AlphaFormat = AC_SRC_ALPHA;

            UpdateLayeredWindow(this.Handle, screenDC, ref topPos, ref size, memDC, ref pointSource, 0, ref blend, ULW_ALPHA);

            ReleaseDC(IntPtr.Zero, screenDC);
            DeleteDC(memDC);
            DeleteObject(hBitmap);
        }
    }
}
