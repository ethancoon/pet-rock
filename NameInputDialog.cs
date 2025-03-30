using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace PetRock
{
    public partial class NameInputDialog : Form
    {

        private TextBox textBox;
        private Button okButton, cancelButton;

        public string InputValue { get; private set; }  // Property to return value

        public NameInputDialog(String originalName)
        {
            this.Text = "Rename your pet rock";
            this.Size = new System.Drawing.Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            textBox = new TextBox { Left = 20, Top = 20, Width = 240 };
            textBox.Text = originalName;
            okButton = new Button { Text = "OK", Left = 150, Top = 60, DialogResult = DialogResult.OK };
            okButton.Left = (this.Size.Width - okButton.Width) / 2;

            okButton.Click += (s, e) => { InputValue = textBox.Text; Close(); };

            this.Controls.Add(textBox);
            this.Controls.Add(okButton);

            this.AcceptButton = okButton;   // Pressing Enter triggers OK
        }
    }
}
