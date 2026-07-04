/// <file>frmAddTask.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using System;
using System.Drawing;

namespace LifeProManager
{
    public partial class frmAddTask : Form
    {
        // Dictionary to map buttons to their base resource names for hover effect
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        private DBConnection dbConn => Program.DbConn;

        // Declaration of the type of main form
        private frmMain? _frmMain = null;
        private Tasks? task;

        public frmAddTask(Form? callingForm, Tasks? task)
        {
            // Allows to re-use the methods of frmMain
            _frmMain = callingForm as frmMain;
            this.task = task;

            InitializeComponent();
            ApplyStoredColors();

            LoadLocalizedStrings();

            // Original images path mapping
            _buttonBaseResourceNames[cmdValidate] = "validate";
            _buttonBaseResourceNames[cmdCancel] = "cancel";

            // Hover events for all buttons
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;

            cmdCancel.MouseEnter += Button_MouseEnter;
            cmdCancel.MouseLeave += Button_MouseLeave;
        }

        /// <summary>
        /// Loads the topics and priorities in the combo boxes, then selects the first topic, lower priority and today's date automatically
        /// </summary>
        private void frmAddTask_Load(object? sender, EventArgs? e)
        {
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

            if (_frmMain != null && _frmMain.CopyLastTaskValues && task != null)
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

            // Background of the window
            this.BackColor = secondaryWindowsBackgroundColor;

            void ApplyThemeToControl(Control ctrl)
            {
                if (ctrl is TextBox || ctrl is ComboBox || ctrl is NumericUpDown)
                {
                    ctrl.BackColor = inputFieldsBackground;
                    ctrl.ForeColor = inputFieldsForeground;
                }
                
                else if (ctrl is Label || ctrl is CheckBox)
                {
                    ctrl.ForeColor = taskTitlesTextColor;
                }

                else if (ctrl is Panel)
                {
                    ctrl.BackColor = panelsBackgroundColor;
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
        /// If the birthday checkbox is checked, the description field, the important and repeatable checkboxes are hidden
        /// and the year numeric up down control is shown. 
        /// The title field changes to "First name" and its maximum length is reduced to 20 characters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkBirthday_CheckedChanged(object? sender, EventArgs? e)
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
        private void cmdCancel_Click(object? sender, EventArgs? e)
        {
            this.Close();
        }

        /// <summary>
        /// Adds the task specified in the textboxes for the date specified in the comboboxes into the database
        /// </summary>
        private void cmdValidate_Click(object? sender, EventArgs?    e)
        {
            // Checks if the task's title is empty
            if (txtTitle.Text == "")
            {
                MessageBox.Show(LocalizationManager.GetString("youMustGiveATitleToYourTask"), LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                // Escapes single quotes to prevent SQL errors
                if (txtTitle.Text.Contains("'"))
                {
                    txtTitle.Text = txtTitle.Text.Replace("'", "''");
                }

                // Converts the deadline to the database format
                string deadline = dtpDeadline.Value.ToString("yyyy-MM-dd");

                // Retrieves the selected topic
                Lists? currentTopic = cboTopics.SelectedItem as Lists;

                int priorityChosen;

                if (chkBirthday.Checked)
                {
                    // Birthday tasks always have priority 4
                    priorityChosen = 4;
                }
                
                else if (chkImportant.Checked && chkRepeatable.Checked)
                {

                    priorityChosen = 3;
                }
                
                else if (chkRepeatable.Checked)
                {
                    priorityChosen = 2;
                }

                else if (chkImportant.Checked)
                {
                    priorityChosen = 1;
                }

                else
                {
                    // Default priority (no checkbox selected)
                    priorityChosen = 0;
                }

                // Inserts the task into the database
                if (priorityChosen == 4)
                {
                    // Birthday tasks store the year instead of a description
                    dbConn.InsertTask(txtTitle.Text, numYear.Value.ToString(),
                        deadline, priorityChosen, currentTopic!.Id, 1);
                }
                else
                {
                    dbConn.InsertTask(txtTitle.Text, txtDescription.Text, deadline,
                        priorityChosen, currentTopic!.Id, 1);
                }

                _frmMain?.LoadTasks();
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

            if (_frmMain != null && _frmMain.CopyLastTaskValues)
            {
                _frmMain.CopyLastTaskValues = false;
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

                return true; // Prevents default beep
            }

            else if (keyData == Keys.Escape)
            {
                cmdCancel.PerformClick();
                return true;
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
        private void txtTitle_TextChanged(object? sender, EventArgs? e)
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

