using System;
using System.Diagnostics;
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
        private Timer idleTimer;
        private const int ticksUntilDropMax = 100;
        private int ticksUntilDrop = ticksUntilDropMax;
        // Probably adapt this to be based on where the taskbar is
        private int targetY = 400;
        private double acceleration = 9.8;
        private double velocity = 0;
        private double currentY;

        private string rockName = "Rocky"; 
        private NameLabelForm nameLabelForm;


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
            pictureBox.Cursor = Cursors.Hand;

            this.ClientSize = pictureBox.Size;

            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.DoubleClick += PictureBox_DoubleClick;
            pictureBox.MouseEnter += PictureBox_MouseEnter;
            pictureBox.MouseLeave += PictureBox_MouseLeave;
    

            this.Controls.Add(pictureBox);

            dropTimer = new Timer();
            dropTimer.Interval = 20;
            dropTimer.Tick += DropTimer_Tick;
            dropTimer.Start();

            idleTimer = new Timer();
            idleTimer.Interval = 20;
            idleTimer.Tick += IdleTimer_Tick;
            idleTimer.Start();
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
                currentY = this.Location.Y;

                if (nameLabelForm != null && nameLabelForm.Visible)
                {
                    int labelX = this.Location.X + 100;
                    int labelY = this.Location.Y - nameLabelForm.Height + 100;
                    nameLabelForm.Location = new Point(labelX, labelY);
                }
            }
        }


        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void PictureBox_DoubleClick(object sender, EventArgs e)
        {
            // generate random number to choose message
            string[] outcomes =
            {
                "the rock seems pleased",
                "the rock seems unphased",
                "the rock seems displeased"
            };

            Random rand = new Random();
            int index = rand.Next(outcomes.Length);
            string selectedMessage = outcomes[index];

            NotificationForm notification = new NotificationForm(selectedMessage);
            int notificationX = this.Location.X + this.Width;
            int notificationY = this.Location.Y + (this.Height / 2) - (notification.Height / 2);
            notification.Location = new Point(notificationX, notificationY);
            notification.Show();

            Timer closeTimer = new Timer();
            closeTimer.Interval = 2000;
            closeTimer.Tick += (s, args) =>
            {
                closeTimer.Stop();
                notification.Close();
            };
            closeTimer.Start();

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
                    velocity = 0;
                    dropTimer.Stop();
                }
                this.Location = new Point(this.Location.X, (int)Math.Round(currentY));
            }
        }


        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            if (nameLabelForm == null)
            {
                nameLabelForm = new NameLabelForm(rockName);
            }

            int labelX = this.Location.X + 100;
            int labelY = this.Location.Y - nameLabelForm.Height + 100;
            nameLabelForm.Location = new Point(labelX, labelY);
            nameLabelForm.Show();
            nameLabelForm.Update();
        }
        private void IdleTimer_Tick(Object sender, EventArgs e)
        {
            ticksUntilDrop -= 1;
            if (dropTimer.Enabled || Location.Y >= targetY)
            {
                ticksUntilDrop = ticksUntilDropMax;
            }
            else if (ticksUntilDrop < 0)
            {
                dropTimer.Start();
                ticksUntilDrop = ticksUntilDropMax;
            }
        }


        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            if (nameLabelForm != null)
                nameLabelForm.Hide();
        }
    }
}
