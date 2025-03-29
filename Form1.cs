using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetRock
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragImagePoint;

        public Form1()
        {
            InitializeComponent();

            this.Text = "Drag Image Example";
            this.Size = new Size(800, 600);

            pictureBox = new PictureBox();
            pictureBox.Image = Image.FromFile("rock.png");
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox.Location = new Point(100, 100);

            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;

            this.Controls.Add(pictureBox);
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragImagePoint = pictureBox.Location;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                int diffX = Cursor.Position.X - dragCursorPoint.X;
                int diffY = Cursor.Position.Y - dragCursorPoint.Y;
                pictureBox.Location = new Point(dragImagePoint.X + diffX, dragImagePoint.Y + diffY);
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
    }
}
