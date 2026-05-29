/// <file>frmEditTask.cs</file>
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
    public partial class frmEditTask : Form
    {
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        private DBConnection dbConn => Program.DbConn;

        // Declaration of the type of main form
        private frmMain mainForm = null;
        private Tasks task;

        public frmEditTask(Form callingForm, Tasks taskProvided)
        {
            // Allows to re-use the methods of frmMain
            mainForm = callingForm as frmMain;
            this.task = taskProvided;

            InitializeComponent();
        }

        /// <summary>
        /// Loads the priorities and topics in the combo boxes, 
        /// automatically selects the first topic, 
        /// fills in the year and loads the task in the form.
        /// </summary>
        private void frmEditTask_Load(object sender, EventArgs e)
        {
            LocalizationManager.LoadLocalizedStringsFor(this);

            // Loads the topics in the combo box
            cboTopics.Items.Clear();
            foreach (Lists topic in dbConn.ReadTopics())
            {
                cboTopics.Items.Add(topic);
                cboTopics.DisplayMember = "Title";
                cboTopics.ValueMember = "Id";
            }

            numYear.Maximum = DateTime.Now.Year;

            // Loads the task in the form
            txtTitle.Text = task.Title;
            txtDescription.Text = task.Description;

            // If priority 1 or 3 has been assigned for this task (odd number)
            if (task.Priorities_id % 2 != 0)
            {                 
                chkImportant.Checked = true;                    
            }

            // If priority 2 or above has been assigned for this task
            if (task.Priorities_id >= 2)
            {
                chkRepeatable.Checked = true;
            }

            // If priority 4 has been assigned for this task
            if (task.Priorities_id == 4)
            {
                chkBirthday.Checked = true;

                // Affects to the numeric up down control the value stored in the description field
                int.TryParse(task.Description, out var numYearValue);
                numYear.Value = numYearValue;
            }

            // Sets the deadline affected to the task in the date picker 
            dtpDeadline.Value = Convert.ToDateTime(task.Deadline);

            // Sets the topic affected to the task in the topic combobox
            cboTopics.Text = dbConn.ReadTopicName(task.Lists_id);

            // Original images path mapping
            _buttonBaseResourceNames[cmdValidate] = "validate_task";
            _buttonBaseResourceNames[cmdCancel] = "delete_task";

            // Hover events for all buttons
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;

            cmdCancel.MouseEnter += Button_MouseEnter;
            cmdCancel.MouseLeave += Button_MouseLeave;
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

        private void chkBirthday_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBirthday.Checked)
            {
                txtDescription.Visible = false;
                lblDescription.Visible = false;
                chkImportant.Visible = false;
                chkRepeatable.Visible = false;
                lblYear.Visible = true;
                numYear.Visible = true;
                lblPriority.Top += 36;
                txtTitle.MaxLength = 20;
                txtTitle.Width = 150;
                lblTitle.Text = LocalizationManager.GetString("firstName");
            }
            else
            {
                txtDescription.Visible = true;
                lblDescription.Visible = true;
                chkImportant.Visible = true;
                chkRepeatable.Visible = true;
                lblYear.Visible = false;
                numYear.Visible = false;
                lblPriority.Top -= 36;
                txtTitle.MaxLength = 70;
                txtTitle.Width = 206;
                lblTitle.Text = LocalizationManager.GetString("title");
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
        /// Edits the task in the database
        /// </summary>
        private void cmdValidate_Click(object sender, EventArgs e)
        {
            // Checks if the task's title is empty
            if (txtTitle.Text == "")
            {
                MessageBox.Show(LocalizationManager.GetString("youMustFillInANameForYourNewTopic"), LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
            else
            {
                // Gets the value of the date time picker and affects it to the deadline string variable
                // in the format used by the database
                string deadline = dtpDeadline.Value.ToString("yyyy-MM-dd");

                // Gets the selected topic from the combo box
                Lists currentTopic = cboTopics.SelectedItem as Lists;

                int priorityChosen = 0;

                // If only the important checkbox is checked
                if (chkImportant.Checked == true && chkRepeatable.Checked == false && chkBirthday.Checked == false)
                {
                    priorityChosen = 1;
                }

                // If only the repeatable checkbox is checked
                else if (chkImportant.Checked == false && chkRepeatable.Checked == true && chkBirthday.Checked == false)
                {
                    priorityChosen = 2;
                }

                // If both the important and repeatable checkboxes are checked
                else if (chkImportant.Checked == true && chkRepeatable.Checked == true && chkBirthday.Checked == false)
                {
                    priorityChosen = 3;
                }

                // If the birthday checkbox is checked
                else if (chkBirthday.Checked == true)
                {
                    priorityChosen = 4;
                }

                if (priorityChosen != 4)
                {
                    // Edit the task informations in the database
                    dbConn.EditTask(this.task.Id, txtTitle.Text, txtDescription.Text, deadline, priorityChosen, currentTopic.Id);
                }

                // If the priority of the task is a birthday (4)
                else
                {
                    dbConn.EditTask(this.task.Id, txtTitle.Text, numYear.Value.ToString(), deadline, priorityChosen, currentTopic.Id);
                }  

                // Reloads tasks in the main form
                mainForm.LoadTasks();

                // Closes the window
                this.Close();
            }            
        }

        /// <summary>
        /// Loads all the localized strings for the UI elements based on the current language setting.
        /// </summary>
        public void LoadLocalizedStrings()
        {
            // --- Window title ---
            this.Text = LocalizationManager.GetString("EditTask");

            // --- Labels ---
            lblDescription.Text = LocalizationManager.GetString("lblDescriptionText");
            lblPriority.Text = LocalizationManager.GetString("lblPriorityText");
            lblDeadline.Text = LocalizationManager.GetString("lblDeadlineText");
            lblTitle.Text = LocalizationManager.GetString("lblTitleText");
            lblTopic.Text = LocalizationManager.GetString("lblTopicText");
            lblYear.Text = LocalizationManager.GetString("lblYearText");

            // --- Checkboxes ---
            chkImportant.Text = LocalizationManager.GetString("chkImportantText");
            chkRepeatable.Text = LocalizationManager.GetString("chkRepeatableText");
            chkBirthday.Text = LocalizationManager.GetString("chkBirthdayText");
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


        /// <summary>
        /// If the user types certain characters in the title of a task, 
        /// it automatically checks the corresponding priority checkboxes 
        /// and moves the focus to the description field. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            if (txtTitle.TextLength == txtTitle.MaxLength)
            {
                txtDescription.Focus();
            }

            // If the user types an exclamation mark in the title of a task and the important checkbox isn't checked
            else if (txtTitle.Text.Contains("!") && chkImportant.Checked == false)
            {
                chkImportant.Checked = true;
                txtDescription.Focus();
            }

            // If the user types a question mark in the title of a task and the repeatable checkbox isn't checked
            else if (txtTitle.Text.Contains("?") && chkRepeatable.Checked == false)
            {
                chkRepeatable.Checked = true;
                txtDescription.Focus();
            }
        }
    }
}
