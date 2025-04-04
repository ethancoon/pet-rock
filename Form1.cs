﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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
        private ContextMenuStrip contextMenu;
        private PictureBox pictureBox;

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

            pictureBox = new PictureBox();
            byte[] imageData = Properties.Resources.rock;
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                Image img = Image.FromStream(ms);
                pictureBox.Image = img;
            }
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

            // Context menu

            contextMenu = new ContextMenuStrip();
            ToolStripMenuItem customizeItem = new ToolStripMenuItem("Customize");
            ToolStripMenuItem renameItem = new ToolStripMenuItem("Rename");
            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");

            string[] styles = { "rock", "rockwizard", "rockhalo", "rockflowers", "rockbowtie" };
            foreach (string style in styles)
            {
                ToolStripMenuItem styleItem = new ToolStripMenuItem(style);
                styleItem.Click += (s, e) => ApplyStyle(style);
                customizeItem.DropDownItems.Add(styleItem);
            }

            renameItem.Click += RenameItem_Click;
            exitItem.Click += Quit;

            contextMenu.Items.Add(customizeItem);
            contextMenu.Items.Add(renameItem);
            contextMenu.Items.Add(exitItem);

            pictureBox.ContextMenuStrip = contextMenu;

            this.Controls.Add(pictureBox);

            dropTimer = new Timer();
            dropTimer.Interval = 20;
            dropTimer.Tick += DropTimer_Tick;
            dropTimer.Start();

            byte[] soundData = Properties.Resources.windwoosh;

            using (MemoryStream ms = new MemoryStream(soundData))
            {
                SoundPlayer player = new SoundPlayer(ms);
                player.Play(); // Use PlaySync() for blocking playback
            }


            idleTimer = new Timer();
            idleTimer.Interval = 20;
            idleTimer.Tick += IdleTimer_Tick;
            idleTimer.Start();
        }

        private void ApplyStyle(String str)
        {
            byte[] imageData = (byte[]) Properties.Resources.ResourceManager.GetObject(str);
            if (imageData == null)
            {
                imageData = Properties.Resources.rockwizard;
            }
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                Image img = Image.FromStream(ms);
                pictureBox.Image = img;
            }
        }

        private void RenameItem_Click(object sender, EventArgs e)
        {
            using (NameInputDialog dialog = new NameInputDialog(rockName))
            {
                if (dialog.ShowDialog() == DialogResult.OK)  // Check if OK was clicked
                {
                    rockName = dialog.InputValue;
                }
            }
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

            byte[] soundData = Properties.Resources.rockquiet;
            using (MemoryStream ms = new MemoryStream(soundData))
            {
                SoundPlayer player = new SoundPlayer(ms);
                player.Play();
            }

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
                    byte[] soundData = Properties.Resources.rockimpact;

                    using (MemoryStream ms = new MemoryStream(soundData))
                    {
                        SoundPlayer player = new SoundPlayer(ms);
                        player.Play(); // Use PlaySync() for blocking playback
                    }
                    dropTimer.Stop();

                }
                this.Location = new Point(this.Location.X, (int)Math.Round(currentY));
            }
        }


        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            nameLabelForm = new NameLabelForm(rockName);

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
                byte[] soundData = Properties.Resources.windwoosh;

                using (MemoryStream ms = new MemoryStream(soundData))
                {
                    SoundPlayer player = new SoundPlayer(ms);
                    player.Play(); // Use PlaySync() for blocking playback
                }
            }
        }


        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            if (nameLabelForm != null)
                nameLabelForm.Hide();
        }

        private void Quit(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

}
