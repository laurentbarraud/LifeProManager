/// <file>frmAbout.cs</file>
/// <author>Laurent Barraud</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using System;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace LifeProManager
{
    public partial class frmAbout : Form
    {
        // Dictionary to map buttons to their base resource names for
        // effect
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();
        
        public frmAbout()
        {
            InitializeComponent();
            ApplyStoredColors();

            // Original image path mapping
            _buttonBaseResourceNames[cmdValidate] = "validate";

            // Hover events for the button
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            LoadLocalizedStrings();
        }

        /// <summary>
        /// Applies all stored theme colors to this window.
        /// Reads the hex values from application settings and updates the controls accordingly.
        /// </summary>
        private void ApplyStoredColors()
        {
            lblLicence.ForeColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText);
        }

        private void cmdConfirm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Changes the button's background image to the hover version 
        /// when the mouse enters the button area.
        /// </summary>
        private void Button_MouseEnter(object? sender, EventArgs? e)
        {
            if (sender is Button btn)
            {
                UIHover.HandleMouseEnter(btn, _buttonBaseResourceNames);
            }
        }

        /// <summary>
        /// Changes back the button's background image to the normal version 
        /// when the mouse leaves the button area.
        /// </summary>
        private void Button_MouseLeave(object? sender, EventArgs? e)
        {
            if (sender is Button btn)
            {
                UIHover.HandleMouseLeave(btn, _buttonBaseResourceNames);
            }
        }

        public void LoadLocalizedStrings()
        {
            // --- Window title ---
            this.Text = LocalizationManager.GetString("aboutThisApp");

            // --- Label ---
            lblLicence.Text = LocalizationManager.GetString("licenceText");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Loads the colors from settings
            Color leftColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfMainWindowBackground);
            Color rightColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfPanelsBackground);

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
