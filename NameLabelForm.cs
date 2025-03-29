using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetRock
{
    public class NameLabelForm : Form
    {
        private Label nameLabel;

        public NameLabelForm(string name)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Magenta;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.AutoSize = true;

            nameLabel = new Label();
            nameLabel.Text = name;
            nameLabel.Font = new Font("Segoe Script", 18, FontStyle.Bold);
            nameLabel.ForeColor = Color.White;
            nameLabel.AutoSize = true;
            nameLabel.BackColor = Color.Transparent;

            this.Controls.Add(nameLabel);


            Size textSize;
            using (Graphics g = this.CreateGraphics())
            {
                textSize = Size.Ceiling(g.MeasureString(nameLabel.Text, nameLabel.Font));
            }
            int padding = 20;
            this.Size = new Size(textSize.Width + padding, textSize.Height + padding);
        }
    }
}
