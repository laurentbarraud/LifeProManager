/// <file>frmAddTopic.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.2</version>
/// <date>May 29th, 2026</date>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace LifeProManager
{
    public partial class frmAddTopic : Form
    {
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        private frmMain mainForm = null;

        public frmAddTopic(Form callingForm)
        {
            mainForm = callingForm as frmMain;
            InitializeComponent();
        }

        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAddTopic_Load(object sender, EventArgs e)
        {
            LocalizationManager.LoadLocalizedStringsFor(this);

            // Original images path mapping
            _buttonBaseResourceNames[cmdValidate] = "validate_task";
            _buttonBaseResourceNames[cmdCancel] = "delete_task";

            // Hover events for all buttons
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;

            cmdCancel.MouseEnter += Button_MouseEnter;
            cmdCancel.MouseLeave += Button_MouseLeave;
            
            txtTopic.SelectAll();
        }

        /// <summary>
        /// Handles the mouse-enter event for any button by replacing its background image
        /// with the corresponding hover version.
        /// </summary>
        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
            {
                return;
            }

            string baseName;

            if (!_buttonBaseResourceNames.TryGetValue(btn, out baseName))
            {
                return;
            }

            // Hover effect
            Image hoverImage = Properties.Resources.ResourceManager.GetObject(baseName + "_hover") as Image;

            if (hoverImage != null)
            {
                btn.BackgroundImage = hoverImage;
            }
        }

        /// <summary>
        /// Handles the mouse-leave event for any button by restoring its original background
        /// image. 
        /// </summary>
        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
            {
                return;
            }

            string baseName;

            if (!_buttonBaseResourceNames.TryGetValue(btn, out baseName))
            {
                return;
            }

            Image normalImage = Properties.Resources.ResourceManager.GetObject(baseName) as Image;

            if (normalImage != null)
            {
                btn.BackgroundImage = normalImage;
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
        public void cmdValidate_Click(object sender, EventArgs e)
        {
            if (txtTopic.Text == "")
            {
                MessageBox.Show(LocalizationManager.GetString("youMustFillInANameForYourNewTopic"), LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Inserts the topic into the database
                mainForm.dbConn.InsertTopic(txtTopic.Text);

                mainForm.LoadTopics();

                // Selects the newly created topic
                foreach (Lists topic in mainForm.cboTopics.Items)
                {
                    if (topic.Title == txtTopic.Text)
                    {
                        mainForm.cboTopics.SelectedItem = topic;
                        break;
                    }
                }

                mainForm.CheckIfPreviousNextTopicArrowButtonsUseful();
                mainForm.UpdateAddTaskButtonVisibility();

                this.Close();
                mainForm.cboTopics.Focus();
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

                return true; // Prevent default beep
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
