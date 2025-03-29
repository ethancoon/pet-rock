using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace PetRock
{
    public class NotificationForm : Form
    {
        private string message;
        private Font messageFont;
        private Color textColor = Color.White;

        public NotificationForm(string message)
        {
            this.message = message;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.StartPosition = FormStartPosition.Manual;
            this.messageFont = new Font("Segoe Script", 24, FontStyle.Bold);

            Size textSize;
            using (Graphics g = this.CreateGraphics())
            {
                textSize = Size.Ceiling(g.MeasureString(message, messageFont));
            }
            int padding = 20;
            this.Size = new Size(textSize.Width + padding, textSize.Height + padding);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (SolidBrush brush = new SolidBrush(textColor))
            {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                e.Graphics.DrawString(message, messageFont, brush, rect, format);
            }
        }
    }
}
