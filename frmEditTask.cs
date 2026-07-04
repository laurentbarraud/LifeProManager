/// <file>frmEditTask.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using System;

namespace LifeProManager
{
    public partial class frmEditTask : Form
    {
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        private DBConnection dbConn => Program.DbConn;

        // Declaration of the type of main form
        private frmMain? _frmMain = null;
        private Tasks task;

        public frmEditTask(Form callingForm, Tasks taskProvided)
        {
            // Allows to re-use the methods of frmMain
            _frmMain = callingForm as frmMain;
            this.task = taskProvided;

            InitializeComponent();
            ApplyStoredColors();

            cmdValidate.BackColor = this.BackColor;
            cmdCancel.BackColor = this.BackColor;
        }

        /// <summary>
        /// Loads the priorities and topics in the combo boxes, 
        /// automatically selects the first topic, 
        /// fills in the year and loads the task in the form.
        /// </summary>
        private void frmEditTask_Load(object sender, EventArgs e)
        {
            LoadLocalizedStrings();

            // Loads the topics in the combo box
            cboTopics.Items.Clear();
            
            foreach (Lists topic in dbConn.ReadTopics())
            {
                cboTopics.Items.Add(topic);
                cboTopics.DisplayMember = "Title";
                cboTopics.ValueMember = "Id";
            }

            // Selects the topic affected to the task in the combo box
            foreach (Lists topic in cboTopics.Items)
            {
                if (topic.Id == task.Lists_id)
                {
                    cboTopics.SelectedItem = topic;
                    break;
                }
            }

            // If no topic is selected, automatically selects the first topic in the combo box
            if (cboTopics.SelectedItem == null && cboTopics.Items.Count > 0)
            {
                cboTopics.SelectedIndex = 0;
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
            _buttonBaseResourceNames[cmdValidate] = "validate";
            _buttonBaseResourceNames[cmdCancel] = "cancel";

            // Hover events for all buttons
            cmdValidate.MouseEnter += Button_MouseEnter;
            cmdValidate.MouseLeave += Button_MouseLeave;

            cmdCancel.MouseEnter += Button_MouseEnter;
            cmdCancel.MouseLeave += Button_MouseLeave;
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
                if (ctrl is TextBox || ctrl is ComboBox || ctrl is NumericUpDown)
                {
                    ctrl.BackColor = inputFieldsBackground;
                    ctrl.ForeColor = inputFieldsForeground;
                }

                else if (ctrl is Label || ctrl is CheckBox)
                {
                    ctrl.ForeColor = taskTitlesTextColor;
                }

                foreach (Control child in ctrl.Controls)
                {
                    ApplyThemeToControl(child);
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
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show(
                    LocalizationManager.GetString("youMustGiveATitleToYourTask"),
                    LocalizationManager.GetString("error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Gets the selected topic from the combo box
            Lists? currentTopic = cboTopics.SelectedItem as Lists;

            if (currentTopic == null)
            {
                MessageBox.Show(
                    LocalizationManager.GetString("topic"),
                    LocalizationManager.GetString("error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Gets the value of the date time picker and affects it to the deadline string variable
            string deadline = dtpDeadline.Value.ToString("yyyy-MM-dd");

            // Determine priority
            int priorityChosen = 0;

            if (chkBirthday.Checked)
            {
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

            // Edit the task informations in the database
            if (priorityChosen == 4)
            {
                // Birthday tasks use the year instead of description
                dbConn.EditTask(this.task.Id, txtTitle.Text, numYear.Value.ToString(), deadline,
                    priorityChosen, currentTopic.Id);
            }
            else
            {
                dbConn.EditTask(this.task.Id, txtTitle.Text, txtDescription.Text, deadline,
                    priorityChosen, currentTopic.Id);
            }

            _frmMain?.LoadTasks();
            this.Close();
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
