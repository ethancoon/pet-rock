using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetRock
{
    public class NotificationForm : Form
    {
        private Label messageLabel;

        public NotificationForm(string message)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;
            this.StartPosition = FormStartPosition.Manual;

            messageLabel = new Label();
            messageLabel.Text = message;
            messageLabel.AutoSize = false;
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messageLabel.ForeColor = Color.White;
            messageLabel.Font = new Font("Segoe Script", 24, FontStyle.Bold);
            messageLabel.BackColor = Color.Transparent;
            messageLabel.Dock = DockStyle.Fill;
            this.Controls.Add(messageLabel);

            Size textSize;
            using (Graphics g = this.CreateGraphics())
            {
                textSize = g.MeasureString(message, messageLabel.Font).ToSize();
            }
            int padding = 20;
            this.Size = new Size(textSize.Width + padding, textSize.Height + padding);
        }
    }
}
