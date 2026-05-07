/// <file>frmAbout.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.</version>
/// <date>May 7th, 2026</date>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LifeProManager
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            LocalizationManager.LoadLocalizedStringsFor(this);
        }
        private void cmdConfirm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void LoadLocalizedStrings()
        {
            // --- Window title ---
            this.Text = LocalizationManager.GetString("aboutThisApp");

            // --- Label ---
            lblLicence.Text = LocalizationManager.GetString("lblLicenceText");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Cloud gradient 
            Color leftColor = Color.FromArgb(230, 255, 255, 255);
            Color rightColor = Color.FromArgb(180, 221, 241, 251); 

            using (LinearGradientBrush gradientBrush = new LinearGradientBrush(this.ClientRectangle,
                leftColor, rightColor, LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(gradientBrush, this.ClientRectangle);
            }
        }

        /// <summary>
        /// Handles Enter key behavior for the form. 
        /// If the active control is a multiline TextBox, Enter inserts a newline.
        /// Otherwise, Enter triggers the validation button.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                cmdValidate.PerformClick();

                return true; // Prevent default beep
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
