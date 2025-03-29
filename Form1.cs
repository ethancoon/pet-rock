using System;
using System.Drawing;
using System.Windows.Forms;

namespace PetRock
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private Timer dropTimer;
        private int targetY = 400;
        private double acceleration = 9.8;
        private double velocity = 0;
        private double currentY;

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(200, -200);
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;

            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = Image.FromFile("rock.png");
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox.Location = new Point(0, 0);

            this.ClientSize = pictureBox.Size;

            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;

            this.Controls.Add(pictureBox);

            dropTimer = new Timer();
            dropTimer.Interval = 20;
            dropTimer.Tick += DropTimer_Tick;
            dropTimer.Start();
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x08000000;
                cp.ExStyle |= 0x00000080;
                return cp;
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                int diffX = Cursor.Position.X - dragCursorPoint.X;
                int diffY = Cursor.Position.Y - dragCursorPoint.Y;
                this.Location = new Point(dragFormPoint.X + diffX, dragFormPoint.Y + diffY);
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void DropTimer_Tick(object sender, EventArgs e)
        {
            if (!dragging && this.Location.Y < targetY)
            {
                double dt = dropTimer.Interval / 75.0;
                velocity += acceleration * dt;
                currentY += velocity * dt;
                if (currentY >= targetY)
                {
                    currentY = targetY;
                    dropTimer.Stop();
                }
                this.Location = new Point(this.Location.X, (int)Math.Round(currentY));
            }
        }


    }
}
