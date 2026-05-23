/// <file>frmAddTask.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.1</version>
/// <date>May 23th, 2026</date>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LifeProManager
{
    public partial class frmAddTask : Form
    {
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        private DBConnection dbConn => Program.DbConn;

        // Declaration of the type of main form
        private frmMain mainForm = null;
        private Tasks task;

        public frmAddTask(Form callingForm, Tasks task)
        {
            // Allows us to re-use the methods of frmMain
            mainForm = callingForm as frmMain;
            this.task = task;

            InitializeComponent();
        }

        /// <summary>
        /// Loads the topics and priorities in the combo boxes, then selects the first topic, lower priority and today's date automatically
        /// </summary>
        private void frmAddTask_Load(object sender, EventArgs e)
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

            cboTopics.SelectedIndex = 0;
            numYear.Maximum = DateTime.Now.Year;
            txtTitle.Focus();

            if (mainForm.CopyLastTaskValues)
            {
                // Marks the task as important if priority is 1 or 3
                chkImportant.Checked = (task.Priorities_id == 1 || task.Priorities_id == 3);

                // Marks the task as repeatable if priority is 2 or 3
                chkRepeatable.Checked = (task.Priorities_id == 2 || task.Priorities_id == 3);

                // Birthday tasks (priority 4)
                if (task.Priorities_id == 4)
                {
                    chkBirthday.Checked = true;

                    // Birthday tasks default to next year
                    dtpDeadline.Value = DateTime.Today.AddYears(1);

                    // Extracts the birth year only if the description is exactly 4 digits long
                    // and is a valid year, otherwise defaults to the current year
                    numYear.Value = ExtractBirthYear(task.Description);
                }

                else
                {
                    // All other tasks default to tomorrow
                    dtpDeadline.Value = DateTime.Today.AddDays(1);
                }

                // Copies the title and description from the last task
                txtTitle.Text = task.Title;
                txtDescription.Text = task.Description;

                // Sets the topic of the new task to the same topic as the last task
                cboTopics.Text = dbConn.ReadTopicName(task.Lists_id);
            }

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

        /// <summary>
        /// If the birthday checkbox is checked, the description field, the important and repeatable checkboxes are hidden
        /// and the year numeric up down control is shown. 
        /// The title field changes to "First name" and its maximum length is reduced to 20 characters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Adds the task specified in the textboxes for the date specified in the comboboxes into the database
        /// </summary>
        private void cmdValidate_Click(object sender, EventArgs e)
        {
            // Checks if the task's title is empty
            if (txtTitle.Text == "")
            {
                MessageBox.Show(LocalizationManager.GetString("youMustGiveATitleToYourTask"), LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            else
            {
                // Escapes single quotes by doubling them to prevent the SQL insert from crashing the app
                if (txtTitle.Text.Contains("'"))
                {
                    txtTitle.Text = txtTitle.Text.Replace("'", "''");
                }

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

                // Task insertion into the database 
                if (priorityChosen == 4)
                {
                    dbConn.InsertTask(txtTitle.Text, numYear.Value.ToString(), deadline, priorityChosen, currentTopic.Id, 1);
                    
                }

                else
                {
                    dbConn.InsertTask(txtTitle.Text, txtDescription.Text, deadline, priorityChosen, currentTopic.Id, 1);
                }

                mainForm.LoadTasks();
                this.Close();
            }
        }

        /// <summary>
        /// Extracts a valid birth year from a string.
        /// Accepts only a strict 4-digit format like "1984".
        /// Returns the current year if the format is invalid or out of range.
        /// </summary>
        private int ExtractBirthYear(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return DateTime.Now.Year;
            }

            // Accepts only a pure 4-digit string
            if (description.Length == 4 && int.TryParse(description, out int year))
            {
                // Ensures the year is not in the future
                if (year <= DateTime.Now.Year)
                {
                    return year;
                }
            }

            // Default fallback: current year
            return DateTime.Now.Year;
        }

        /// <summary>
        /// Prevents unwanted copying of values when adding a new task after editing a task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAddTask_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (mainForm.CopyLastTaskValues)
            {
                mainForm.CopyLastTaskValues = false;
            }
        }

        /// <summary>
        /// Loads all the localized strings for the UI elements based on the current language setting.
        /// </summary>
        public void LoadLocalizedStrings()
        {
            // --- Window title ---
            this.Text = LocalizationManager.GetString("AddTask");

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

