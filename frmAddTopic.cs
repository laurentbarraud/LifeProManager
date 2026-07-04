/// <file>frmAddTopic.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Forms;
using System.Drawing;

namespace LifeProManager
{
    public partial class frmAddTopic : Form
    {
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        private frmMain? _frmMain = null;

        public frmAddTopic(Form callingForm)
        {
            _frmMain = callingForm as frmMain;
            InitializeComponent();
            ApplyStoredColors();

            cmdValidate.BackColor = this.BackColor;
            cmdCancel.BackColor = this.BackColor;
        }

        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAddTopic_Load(object sender, EventArgs e)
        {
            LoadLocalizedStrings();

            // Original images path mapping
            _buttonBaseResourceNames[cmdValidate] = "validate";
            _buttonBaseResourceNames[cmdCancel] = "cancel";

            // Hover events for all buttons
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;
            cmdCancel.MouseEnter += Button_MouseEnter;
            cmdCancel.MouseLeave += Button_MouseLeave;

            txtTopic.SelectAll();
        }

        /// <summary>
        /// Applies the stored theme colors by reading the hex values from application settings
        /// and updating the controls of this window accordingly.
        /// </summary>
        internal void ApplyStoredColors()
        {
            Color secondaryWindowsBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfSecondaryWindowsBackground);
            Color inputFieldsBackground = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsBackground);
            Color inputFieldsForeground = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsText);
            Color taskTitlesTextColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText);
            Color panelsBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfPanelsBackground);

            // Window background
            this.BackColor = secondaryWindowsBackgroundColor;

            void ApplyThemeToControl(Control ctrl)
            {
                if (ctrl is TextBox)
                {
                    ctrl.BackColor = inputFieldsBackground;
                    ctrl.ForeColor = inputFieldsForeground;
                }
                else if (ctrl is Label)
                {
                    ctrl.ForeColor = taskTitlesTextColor;
                }

                foreach (Control childCtrl in ctrl.Controls)
                {
                    ApplyThemeToControl(childCtrl);
                }
            }

            foreach (Control ctrl in this.Controls) 
            { 
                ApplyThemeToControl(ctrl);
            }

            this.Invalidate();
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

        /// <summary>
        /// Closes the form without any change
        /// </summary>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Inserts a topic into the database
        /// </summary>
        /// <summary>
        /// Inserts a topic into the database
        /// </summary>
        public void cmdValidate_Click(object sender, EventArgs e)
        {
            if (txtTopic.Text == "")
            {
                MessageBox.Show(LocalizationManager.GetString("youMustFillInANameForYourNewTopic"),
                    LocalizationManager.GetString("error"), MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            if (_frmMain != null)
            {
                _frmMain.dbConn.InsertTopic(txtTopic.Text);
                _frmMain.LoadTopics();

                // Selects the newly created topic
                foreach (Lists topic in _frmMain.cboTopics.Items)
                {
                    if (topic.Title == txtTopic.Text)
                    {
                        _frmMain.cboTopics.SelectedItem = topic;
                        break;
                    }
                }

                _frmMain.CheckIfPreviousNextTopicArrowButtonsUseful();
                _frmMain.UpdateAddTaskButtonVisibility();

                this.Close();
                _frmMain.cboTopics.Focus();
            }
        }

        /// <summary>
        /// Loads all the localized strings for the UI elements based on the current language setting.
        /// </summary>
        public void LoadLocalizedStrings()
        {
            // --- Window title ---
            this.Text = LocalizationManager.GetString("AddTopic");

            // --- Labels ---
            lblTopic.Text = LocalizationManager.GetString("lblTopicText");

            // --- TextBox placeholder / default text ---
            txtTopic.Text = LocalizationManager.GetString("txtTopicText");
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
                // If the active control is a multiline TextBox, allows newline insertion
                if (this.ActiveControl is TextBox tb && tb.Multiline)
                {
                    return false; // Lets the TextBox handle Enter normally
                }

                cmdValidate.PerformClick();

                return true; // Prevents default beep
            }

            else if (keyData == Keys.Escape)
            {
                cmdCancel.PerformClick();
                return true; 
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
