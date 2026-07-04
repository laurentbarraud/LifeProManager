/// <file>frmMain.cs</file>
/// <author>Laurent Barraud, David Rossy and Julien Terrapon</author>
/// <version>1.8.3</version>
/// <date>July 4th, 2026</date>

using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using static LifeProManager.LayoutBuilder;

namespace LifeProManager
{
    public partial class frmMain : Form
    {
        // Private members

        /// Maps each button to its base image resource name for easy retrieval during hover events
        private readonly Dictionary<Button, string> _buttonBaseResourceNames = new Dictionary<Button, string>();

        // Allows to copy last task values if it has been set with "repeatable" priority
        private bool _copyLastTaskValues = false;

        // Indicates whether the form should play the fade‑in animation when shown.
        private readonly bool _enableFadeIn = false;

        // Timer used to perform the fade‑in animation when the form is shown.
        private readonly System.Windows.Forms.Timer fadeInTimer = new System.Windows.Forms.Timer();

        /// <summary>
        /// Indicates whether the main window is still initializing.
        /// Used to prevent UI refresh operations (ReloadUI) from running too early,
        /// which avoids layout corruption and null reference issues.
        /// </summary>
        private bool _isInitializing;

        // Language codes mapped to ComboBox indices
        private readonly string[] _languageCodes = { "en", "fr", "es" };

        private LayoutBuilder layoutBuilder;

        // Array to store the next seven days in "yyyy-MM-dd" format for quick access
        private string[] plusSevenDays = new string[7];

        private int nbTasksToComplete = 0;

        // Stores the currently selected date
        private string selectedDateString = DateTime.Today.ToString("yyyy-MM-dd");

        // Stores the ID of the currently selected task
        private int selectedTaskId = -1;

        private SmartSearch smartSearch;

        // Public properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CopyLastTaskValues
        {
            get { return _copyLastTaskValues; }
            set { _copyLastTaskValues = value; }
        }

        // Provides access to the global database connection created in Program.cs.
        // This ensures all forms use the same connection instance.
        public DBConnection dbConn => Program.DbConn;

        /// <summary>
        /// Returns true if the specified task is currently selected.
        /// Used to support click‑to‑toggle selection logic.
        /// </summary>
        public bool IsTaskSelected(int taskId)
        {
            return selectedTaskId == taskId;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedDateString
        {
            get { return selectedDateString; }
            set { selectedDateString = value; }
        }

        // Stores, for each task list panel, the list of SelectableTaskRow objects
        // currently displayed inside it.
        // This allows each panel to manage its own independent selection state.
        // When a task row is clicked, the application can refer to this dictionary to
        // determine which task is selected in that specific panel.
        //        
        // Each panel shows its own independent list of tasks.
        // When the user clicks on a task row, the application must know which panel
        // the click happened in and which task inside that panel is currently selected.
        //
        // How it works:
        // ---------------------------------------------
        // The dictionary maps:
        //      Panel  →  List<SelectableTaskRow>
        //
        // Example:
        //      pnlToday    → [ row1, row2, row3 ]
        //      pnlWeek     → [ row4, row5 ]
        //      pnlTopics   → [ row6, row7, row8, row9 ]
        //      pnlFinished → [ row10, row11 ]
        //
        // Each SelectableTaskRow contains:
        //      - TaskId
        //      - TitleLabel (the clickable label)
        //      - RowPanel   (the container panel for the row)
        //      - Priority, Description, etc.
        //
        // Why this is useful:
        // ---------------------------------------------
        // When the user clicks a label, we can scan the list of rows for the panel
        // that contains this label and immediately find the corresponding task.
        // This allows:
        //      - Correct selection highlighting
        //      - Correct context menu behavior
        //      - Correct task editing/deleting/validating
        //      - Independent selection per panel
        //
        // Without this structure, the UI would not be able to determine which task
        // the user clicked on, especially when multiple panels are visible or when
        // tasks move between panels.
        public Dictionary<Panel, List<SelectableTaskRow>> selectionByPanel
            = new Dictionary<Panel, List<SelectableTaskRow>>();

        public frmMain(bool enableFadeIn = false)
        {
            _isInitializing = true;
            InitializeComponent();
            LoadLocalizedStrings();

            // Original images path mapping for hover effects
            _buttonBaseResourceNames[cmdPreviousDay] = "leftChevron";
            _buttonBaseResourceNames[cmdPreviousTopic] = "leftChevron";
            _buttonBaseResourceNames[cmdDeleteTopic] = "deleteTrash";
            _buttonBaseResourceNames[cmdDeleteFinishedTasks] = "deleteTrash";
            _buttonBaseResourceNames[cmdToday] = "calendarToday";
            _buttonBaseResourceNames[cmdNextDay] = "rightChevron";
            _buttonBaseResourceNames[cmdNextTopic] = "rightChevron";
            _buttonBaseResourceNames[cmdExportToHtml] = "exportToHtml";
            _buttonBaseResourceNames[cmdBirthdayCalendar] = "birthdayCake";
            _buttonBaseResourceNames[cmdAddTask] = "addTask";
            _buttonBaseResourceNames[cmdAddTopic] = "addTopic";
            _buttonBaseResourceNames[cmdSearch] = "search";

            // Hover events for all buttons
            cmdPreviousDay.MouseEnter += Button_MouseEnter;
            cmdPreviousDay.MouseLeave += Button_MouseLeave;

            cmdPreviousTopic.MouseEnter += Button_MouseEnter;
            cmdPreviousTopic.MouseLeave += Button_MouseLeave;

            cmdToday.MouseEnter += Button_MouseEnter;
            cmdToday.MouseLeave += Button_MouseLeave;

            cmdNextDay.MouseEnter += Button_MouseEnter;
            cmdNextDay.MouseLeave += Button_MouseLeave;

            cmdNextTopic.MouseEnter += Button_MouseEnter;
            cmdNextTopic.MouseLeave += Button_MouseLeave;

            cmdExportToHtml.MouseEnter += Button_MouseEnter;
            cmdExportToHtml.MouseLeave += Button_MouseLeave;

            cmdBirthdayCalendar.MouseEnter += Button_MouseEnter;
            cmdBirthdayCalendar.MouseLeave += Button_MouseLeave;

            cmdAddTopic.MouseEnter += Button_MouseEnter;
            cmdAddTopic.MouseLeave += Button_MouseLeave;

            cmdAddTask.MouseEnter += Button_MouseEnter;
            cmdAddTask.MouseLeave += Button_MouseLeave;

            cmdSearch.MouseEnter += Button_MouseEnter;
            cmdSearch.MouseLeave += Button_MouseLeave;

            cmdDeleteTopic.MouseEnter += Button_MouseEnter;
            cmdDeleteTopic.MouseLeave += Button_MouseLeave;

            cmdDeleteFinishedTasks.MouseEnter += Button_MouseEnter;
            cmdDeleteFinishedTasks.MouseLeave += Button_MouseLeave;

            ApplyStoredColors();

            // Panel selection dictionary initialization
            selectionByPanel = new Dictionary<Panel, List<SelectableTaskRow>>
            {
                { pnlToday, new List<SelectableTaskRow>() },
                { pnlWeek, new List<SelectableTaskRow>() },
                { pnlTopics, new List<SelectableTaskRow>() },
                { pnlFinished, new List<SelectableTaskRow>() }
            };

            smartSearch = new SmartSearch();

            // Layouts
            layoutBuilder = new LayoutBuilder(this, LayoutBuilder.LayoutType.Today);
            layoutBuilder = new LayoutBuilder(this, LayoutBuilder.LayoutType.Week);
            layoutBuilder = new LayoutBuilder(this, LayoutBuilder.LayoutType.Topics);
            layoutBuilder = new LayoutBuilder(this, LayoutBuilder.LayoutType.Finished);

            // Fade-in effect
            _enableFadeIn = enableFadeIn;

            if (_enableFadeIn)
            {
                InitializeFadeInAnimation();
            }

            RestoreWindowWidthFromSettings();
            this.SizeChanged += frmMain_SizeChanged;
            _isInitializing = false;
        }

        /// <summary>
        /// Initializes the application by verifying the database, loading topics and tasks,
        /// wiring UI events, and applying the user's language preferences.
        /// Also configures the calendar, date values, and various interface elements.
        /// </summary>
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Language selection ComboBox is populated with localized names and the current language is selected.
            string currentLangCode = Properties.Settings.Default.appLanguageCode;
            int langIndex = Array.IndexOf(_languageCodes, currentLangCode);
            cboAppLanguage.SelectedIndex = (langIndex >= 0) ? langIndex : 0;

            // Dates and calendar initialization
            selectedDateString = DateTime.Today.ToString("yyyy-MM-dd");

            // Precomputes the next seven days in "yyyy-MM-dd" format for quick access
            plusSevenDays = new string[7];

            for (int i = 0; i < 7; i++)
            {
                plusSevenDays[i] = DateTime.Today.AddDays(i + 1).ToString("yyyy-MM-dd");
            }

            // Export mode checkboxes are set based on the stored user preference.
            switch (Properties.Settings.Default.exportMode)
            {
                case 1:
                    chkDescriptions.Checked = true;
                    break;

                case 2:
                    chkTopics.Checked = true;
                    break;

                case 3:
                    chkDescriptions.Checked = true;
                    chkTopics.Checked = true;
                    break;
            }

            // Windows startup checkbox is set based on the presence of the application in the Windows registry.
            RegistryKey? runKey = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            chkRunAtWindowsStartup.Checked = (runKey?.GetValue("LifeProManager") != null);

            // Description font size is restored from user settings and applied to the task description label.
            int savedFontSize = Properties.Settings.Default.taskDescriptionFontSize;
            lblTaskDescription.Font = new Font(lblTaskDescription.Font.FontFamily, savedFontSize);
            nudTaskDescriptionFontSize.Value = savedFontSize;

            // Once everything is initialized, loads all topics and tasks from the database to populate the UI.
            LoadTasks();
        }

        /// <summary>
        /// Applies stored colors to all currently opened secondary windows.
        /// </summary>
        internal void ApplyColorsToSecondaryWindows()
        {
            Form? detectedForm;

            // Checks if the add task form is open
            detectedForm = Application.OpenForms["frmAddTask"];

            if (detectedForm != null)
            {
                var addTaskForm = detectedForm as frmAddTask;

                if (addTaskForm != null)
                {
                    // Applies the stored colors to the add task form
                    addTaskForm.ApplyStoredColors();
                }
            }

            detectedForm = Application.OpenForms["frmEditTask"];

            if (detectedForm != null)
            {
                var editTaskForm = detectedForm as frmEditTask;

                if (editTaskForm != null)
                {
                    editTaskForm.ApplyStoredColors();
                }
            }

            detectedForm = Application.OpenForms["frmAddTopic"];

            if (detectedForm != null)
            {
                var addTopicForm = detectedForm as frmAddTopic;

                if (addTopicForm != null)
                {
                    addTopicForm.ApplyStoredColors();
                }
            }

            detectedForm = Application.OpenForms["frmCustomizeAppColors"];

            if (detectedForm != null)
            {
                var customizeAppColorsForm = detectedForm as frmCustomizeAppColors;

                if (customizeAppColorsForm != null)
                {
                    customizeAppColorsForm.ApplyStoredColors();
                }
            }
        }

        /// <summary>
        /// Applies the localized language names to the language selection ComboBox 
        /// and selects the current language.
        /// </summary>
        private void ApplyLanguageComboBoxItems()
        {
            cboAppLanguage.Items.Clear();

            // Adds localized language names in the same order as _languageCodes
            cboAppLanguage.Items.Add(LocalizationManager.GetString("langEnglish"));
            cboAppLanguage.Items.Add(LocalizationManager.GetString("langFrench"));
            cboAppLanguage.Items.Add(LocalizationManager.GetString("langSpanish"));

            // Selects the current language based on its tokenIndex in _languageCodes
            string currentLanguageCode = LocalizationManager.GetCurrentLanguageCode();

            int index = Array.IndexOf(_languageCodes, currentLanguageCode);

            if (index == -1)
            {
                index = 0; // fallback to English
            }

            cboAppLanguage.SelectedIndex = index;
        }

        /// <summary>
        /// Applies the responsive layout rules to the main window.
        /// </summary>
        private void ApplyResponsiveLayout()
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            // Adjusts the gradientWidth of the TabControl to keep it aligned with the side panel
            tabMain.Width = this.ClientSize.Width - pnlRight.Width - tabMain.Left - 30;

            // Adjust the internal panels
            pnlToday.Width = tabDates.Width - 60;
            pnlWeek.Width = tabDates.Width - 60;
            pnlTopics.Width = tabMain.Width - 60;
            pnlFinished.Width = tabMain.Width - 60;

            // Adjusts the tasksFound in each panel
            ResizeTasksInPanel(pnlToday);
            ResizeTasksInPanel(pnlWeek);
            ResizeTasksInPanel(pnlTopics);
            ResizeTasksInPanel(pnlFinished);

            // If Topics tab is selected: places cmdDeleteTopic a few pixels from the right edge of its parent
            if (tabMain.SelectedTab == tabTopics && cmdDeleteTopic.Parent != null)
            {
                cmdDeleteTopic.Left = cmdDeleteTopic.Parent.ClientSize.Width
                                      - cmdDeleteTopic.Width
                                      - 35;
            }

            // If Finished tab is selected: places cmdDeleteFinishedTasks a few pixels from the right edge of its parent
            if (tabMain.SelectedTab == tabFinished && cmdDeleteFinishedTasks.Parent != null)
            {
                cmdDeleteFinishedTasks.Left = cmdDeleteFinishedTasks.Parent.ClientSize.Width
                                             - cmdDeleteFinishedTasks.Width
                                             - 35;
            }

        }

        /// <summary>
        /// Applies a mixed layout directly to the Settings tab:
        /// - Most controls are left-aligned with a fixed margin.
        /// - A selected group of controls is centered horizontally.
        /// This keeps the layout visually clean and desktop-friendly.
        /// </summary>
        private void ApplySettingsLayout()
        {
            int containerWidth = tabSettings.ClientSize.Width;

            if (containerWidth <= 0)
            {
                return;
            }

            // Controls that should remain centered horizontally
            Control[] centeredControls =
            {
                chkDescriptions,
                chkTopics,
                nudTaskDescriptionFontSize,
                chkRunAtWindowsStartup,
                lnkChangeAppColors,
                lnkInsertTasksFromSql,
                lnkExportTasksToSql,
                lnkAppInLanguage,
                cboAppLanguage
            };

            foreach (Control ctrl in tabSettings.Controls)
            {
                // Centers selected controls
                if (centeredControls.Contains(ctrl))
                {
                    ctrl.Left = (containerWidth - ctrl.Width) / 2;
                }
                else
                {
                    // Left-align all other controls with a fixed margin
                    ctrl.Left = 40;
                }
            }
        }

        /// <summary>
        /// Applies all stored theme colors to static and dynamic controls
        /// of the main window. 
        /// </summary>
        internal void ApplyStoredColors()
        {
            // Loads all theme colors once for performance
            Color mainWindowBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfMainWindowBackground);
            Color rightPanelBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfRightPanelBackground);
            Color panelsBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfPanelsBackground);
            Color lblTaskDescriptionBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfLblTaskDescriptionBackground);
            Color lblTaskDescriptionForegroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfLblTaskDescriptionText);
            Color inputFieldsBackgroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsBackground);
            Color inputFieldsForegroundColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsText);
            Color taskTitlesTextColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText);

            // Main window
            this.BackColor = mainWindowBackgroundColor;

            // Right panel
            pnlRight.BackColor = rightPanelBackgroundColor;

            // Task panels
            Panel[] taskPanels = { pnlToday, pnlWeek, pnlTopics, pnlFinished };

            foreach (Panel taskPanel in taskPanels)
            {
                taskPanel.BackColor = panelsBackgroundColor;
            }

            // Task description panel
            lblTaskDescription.BackColor = lblTaskDescriptionBackgroundColor;
            lblTaskDescription.ForeColor = lblTaskDescriptionForegroundColor;

            // TabControl
            tabMain.BackColor = mainWindowBackgroundColor;
            tabMain.ForeColor = taskTitlesTextColor;
            tabMain.DrawMode = TabDrawMode.Normal;

            // TabPages
            TabPage[] tabPages = { tabDates, tabTopics, tabFinished, tabSettings };

            foreach (TabPage tabPage in tabPages)
            {
                tabPage.BackColor = mainWindowBackgroundColor;
                tabPage.ForeColor = taskTitlesTextColor;
            }

            // ComboBoxes
            ComboBox[] mainWindowComboboxes = { cboTopics, cboAppLanguage };

            foreach (ComboBox combobox in mainWindowComboboxes)
            {
                combobox.BackColor = inputFieldsBackgroundColor;
                combobox.ForeColor = inputFieldsForegroundColor;
            }

            // Task title labels
            Label[] taskTitleLabels = { lblTopic, lblWeek, lblToday, lblTaskDescriptionFontSize, lblExportDeadlineAndTitle };

            foreach (Label label in taskTitleLabels)
            {
                label.ForeColor = taskTitlesTextColor;
            }

            // LinkLabels
            LinkLabel[] linksLabels = { lnkAppInLanguage, lnkInsertTasksFromSql, lnkExportTasksToSql, lnkChangeAppColors };

            foreach (LinkLabel linkLabel in linksLabels)
            {
                linkLabel.LinkColor = taskTitlesTextColor;
            }

            // CheckBoxes
            CheckBox[] checkBoxes = { chkTopics, chkDescriptions, chkRunAtWindowsStartup };

            foreach (CheckBox checkBox in checkBoxes)
            {
                checkBox.ForeColor = taskTitlesTextColor;
            }

            RefreshAllValidateButtons();
            RecolorDynamicTaskRows();

            // Forces repaint
            this.Invalidate();
        }

        /// <summary>
        /// Dynamically centers the topic header block within the topics panel. 
        /// The method adjusts the horizontal layout based on control visibility, 
        /// measured text gradientWidth, and the available panel gradientWidth.
        /// </summary>
        private void ApplyTopicHeaderResponsive()
        {
            int containerWidth = pnlTopics.ClientSize.Width;

            if (containerWidth <= 0)
            {
                return;
            }

            // Measures label gradientWidth based on its current text and font
            int lblTopicWidth = TextRenderer.MeasureText(lblTopic.Text, lblTopic.Font).Width;

            // Dynamic spacing between arrows and title:
            // - "lblTopicWidth / 8" makes the spacing grow with the title length.
            // - "12" is the minimum spacing to avoid elements being too close.
            // - "40" is the maximum spacing to prevent the layout from stretching too far.
            // The final value is clamped between 12 and 40.
            int spacingBetweenArrowsAndTitle = Math.Max(12, Math.Min(lblTopicWidth / 8, 40));

            // Arrow widths depending on visibility
            int cmdPreviousTopicWidth = cmdPreviousTopic.Visible ? cmdPreviousTopic.Width : 0;
            int cmdNextTopicWidth = cmdNextTopic.Visible ? cmdNextTopic.Width : 0;

            // Total gradientWidth of the full header block: [Prev] [Title] [Next]
            int totalHeaderWidth = cmdPreviousTopicWidth + (cmdPreviousTopic.Visible ?
                spacingBetweenArrowsAndTitle : 0) + lblTopicWidth + (cmdNextTopic.Visible ?
                spacingBetweenArrowsAndTitle : 0) + cmdNextTopicWidth;

            // Left coordinate of the centered header block
            int blockLeftPos = (containerWidth - totalHeaderWidth) / 2;
            int currentPosX = blockLeftPos;

            // Previous arrow
            if (cmdPreviousTopic.Visible)
            {
                cmdPreviousTopic.Left = currentPosX;
                cmdPreviousTopic.Top = lblTopic.Top;
                currentPosX += cmdPreviousTopicWidth + spacingBetweenArrowsAndTitle;
            }

            // Title label
            lblTopic.Left = currentPosX;
            currentPosX += lblTopicWidth + (cmdNextTopic.Visible ? spacingBetweenArrowsAndTitle : 0);

            // Next arrow
            if (cmdNextTopic.Visible)
            {
                cmdNextTopic.Left = currentPosX;
                cmdNextTopic.Top = lblTopic.Top;
            }
        }

        /// <summary>
        /// Applies the user‑selected color to the validate button icon.
        /// The original icon colors are ignored: only the alpha (shape) is kept.
        /// A clean diagonal gradient is applied.
        /// </summary>
        /// <param name="btn">The button receiving the recolored icon.</param>
        public void ApplyValidateButtonColor(Button btn)
        {
            try
            {
                // Gets the user‑selected color from settings and converts it to a Color object
                Color targetColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfValidateButton);

                // Loads the base icon from a task mask to preserve the shape and transparency
                Bitmap baseIcon = Properties.Resources.validateTaskMask;

                // Creates a new bitmap to hold the recolored icon
                Bitmap recoloredBitmap = new Bitmap(baseIcon.Width, baseIcon.Height);

                int baseIconWidth = baseIcon.Width;
                int baseIconHeight = baseIcon.Height;

                for (int y = 0; y < baseIconHeight; y++)
                {
                    for (int x = 0; x < baseIconWidth; x++)
                    {
                        Color px = baseIcon.GetPixel(x, y);

                        // Only recoloring non‑transparent pixels to preserve the icon shape
                        if (px.A > 0)
                        {
                            // Gradient factor (0..1) from top-left to bottom-right
                            float gradientFactor = (float)(x + y) / (baseIconWidth + baseIconHeight);

                            // Gradient intensity (lighter → darker)
                            float gradientIntensity = 0.65f + 0.35f * gradientFactor;

                            int redValue = (int)(targetColor.R * gradientIntensity);
                            int greenValue = (int)(targetColor.G * gradientIntensity);
                            int blueValue = (int)(targetColor.B * gradientIntensity);

                            recoloredBitmap.SetPixel(x, y, Color.FromArgb(px.A, redValue, greenValue, blueValue));
                        }
                        else
                        {
                            recoloredBitmap.SetPixel(x, y, Color.Transparent);
                        }
                    }
                }

                // Dispose previous image to avoid stacking multiple bitmaps
                if (btn.Image != null)
                {
                    btn.Image.Dispose();
                    btn.Image = null;
                }

                // Applies the recolored icon to the button
                btn.Image = recoloredBitmap;
            }
            catch
            {
                // Fallback to the default icon stored in resources
                btn.Image = Properties.Resources.validateTask;
            }
        }


        /// <summary>
        /// Asks the user if he/she wants to copy last approved task to repeat it in the future
        /// </summary>
        /// <param name="task">The task that will be copied</param>
        public void AskForCopyingTask(Tasks task)
        {
            // Localized confirmation dialog
            var confirmCopy = MessageBox.Show(LocalizationManager.GetString("repeatTaskAnotherDay"), LocalizationManager.GetString("confirmCopy"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmCopy == DialogResult.Yes)
            {
                // Allows to pre-fill title and description of the task
                _copyLastTaskValues = true;

                new frmAddTask(this, task).ShowDialog();
            }
        }
        /// <summary>
        /// Applies the correct hover image when the mouse enters a button.
        /// Includes special super-hover effects for specific buttons.
        /// </summary>
        private void Button_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                UIHover.HandleMouseEnterMainForm(btn, _buttonBaseResourceNames);
            }
        }

        /// <summary>
        /// Changes back the button's background image to the normal version 
        /// when the mouse leaves the button area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                UIHover.HandleMouseLeave(btn, _buttonBaseResourceNames);
            }
        }

        /// <summary>
        /// Handles the event when the user selects a date in the calendar.
        /// Updates the Today label using localized text and reloads tasksFound.
        /// </summary>
        private void calMonth_DateChanged(object sender, DateRangeEventArgs e)
        {
            string? labelText = GetCurrentDateLabel();
            DateTime selectedDate = calMonth.SelectionStart;

            // Update selectedDateString for DB queries
            selectedDateString = selectedDate.ToString("yyyy-MM-dd");

            if (labelText == null)
            {
                // For dates beyond ±2 days: show only the date
                lblToday.Text = selectedDate.ToString("d", CultureInfo.CurrentUICulture);
            }
            else
            {
                // For dates close to today: show currentDateLabel and date
                lblToday.Text = $"{labelText} ({selectedDate.ToString("d", CultureInfo.CurrentUICulture)})";
            }

            LoadTasksForDate();
        }

        /// <summary>
        /// Handles live application localization when the user selects a new language
        /// from the ComboBox.  
        /// If the selected language differs from the stored one, the UI culture is
        /// updated first, then the main form is recreated so that all resources reload
        /// in the new language without restarting the application.
        /// </summary>
        private void cboAppLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAppLanguage.SelectedIndex < 0)
            {
                return;
            }

            // Converts the selected tokenIndex into a language code
            string selectedLanguageCode = _languageCodes[cboAppLanguage.SelectedIndex];

            // Only proceeds if the language has actually changed
            if (Properties.Settings.Default.appLanguageCode != selectedLanguageCode)
            {
                Properties.Settings.Default.appLanguageCode = selectedLanguageCode;
                Properties.Settings.Default.Save();

                // Applies the culture before recreating the form
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguageCode);
                Thread.CurrentThread.CurrentCulture = new CultureInfo(selectedLanguageCode);

                // Applies culture to the custom localization manager
                LocalizationManager.SetLanguage(selectedLanguageCode);

                // Creates a new instance of the main form using the updated culture
                frmMain newForm = new frmMain(enableFadeIn: true);

                // Selects the settings tab in the new form
                newForm.tabMain.SelectedTab = newForm.tabSettings;

                // Replaces the current main form without restarting the application
                Program.SwitchMainForm(newForm);
            }
        }

        /// <summary>
        /// Loads the tasksFound for the selected topic
        /// </summary>
        private void cboTopics_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ignores placeholder text when no topic is selected
            if (cboTopics.SelectedIndex == -1)
            {
                return;
            }

            tabMain.SelectedTab = tabTopics;
            LoadTasksInTopic();
        }

        /// <summary>
        /// Checks if the previous topic and next topic arrow buttons should be displayed
        /// </summary>
        public void CheckIfPreviousNextTopicArrowButtonsUseful()
        {
            if (cboTopics.Items.Count <= 1)
            {
                cmdPreviousTopic.Visible = false;
                cmdNextTopic.Visible = false;
            }
            else
            {
                cmdPreviousTopic.Visible = true;
                cmdNextTopic.Visible = true;
            }
        }

        private void chkDescriptions_CheckedChanged(object sender, EventArgs e)
        {
            ExportCheckboxesResult();
        }

        private void chkRunAtWindowsStartup_CheckedChanged(object sender, EventArgs e)
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey? registryRunKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            // The checkbox has been checked by the user and the value in the registry doesn't exist
            if (chkRunAtWindowsStartup.Checked && registryRunKey?.GetValue("LifeProManager") == null)
            {
                // Add the value in the registry so that the application runs at startup
                registryRunKey?.SetValue("LifeProManager", Application.ExecutablePath);
            }

            // If the checkbox has been unchecked by the user and the application has been set to run at startup
            else if (chkRunAtWindowsStartup.Checked == false && registryRunKey?.GetValue("LifeProManager") != null)
            {
                // Remove the value from the registry so that the application doesn't start
                registryRunKey.DeleteValue("LifeProManager", false);
            }
        }

        private void chkTopics_CheckedChanged(object sender, EventArgs e)
        {
            ExportCheckboxesResult();
        }

        /// <summary>
        /// Shows the form to add a task or the form to add a topic if none has been created yet
        /// </summary>

        private void cmdAddTask_Click(object sender, EventArgs e)
        {
            // If no topic has been created yet
            if (cboTopics.Items.Count == 0)
            {
                cmdAddTopic.PerformClick();
            }

            else
            {
                new frmAddTask(this, null).ShowDialog();
            }
        }

        private void cmdAddTask_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cmdAddTask_MouseLeave(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Shows the form to add a topic when the user clicks on the small plus button,
        /// next to the topics drop-down list
        /// </summary>
        private void cmdAddTopic_Click(object sender, EventArgs e)
        {
            frmAddTopic addTopicForm = new frmAddTopic(this);
            addTopicForm.ShowDialog();
        }

        private void cmdAddTopic_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cmdAddTopic_MouseLeave(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Displays the birthday calendar form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdBirthdayCalendar_Click(object sender, EventArgs e)
        {
            new frmBirthdayCalendar(this).ShowDialog();
        }

        private void cmdBirthdayCalendar_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cmdBirthdayCalendar_MouseLeave(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Deletes all finished tasks from the database.
        /// </summary>
        private void cmdDeleteFinishedTasks_Click(object sender, EventArgs e)
        {
            // Deletes all finished tasks
            dbConn.DeleteAllDoneTasks();
            LoadDoneTasks();
            lblTaskDescription.Visible = false;
            cmdDeleteFinishedTasks.Visible = false;
        }

        /// <summary>
        /// Deletes the currently displayed topic and therefore all tasks associated with it.
        /// SHIFT + Click = deletes all topics and all tasks.
        /// </summary>
        private void cmdDeleteTopic_Click(object sender, EventArgs e)
        {
            // Hidden shortcut: SHIFT + Click
            if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control)
            {
                DialogResult dialogResult = MessageBox.Show(LocalizationManager.GetString("deleteAllTopicsWarning"),
                    LocalizationManager.GetString("deleteAllTopicsTitle"), MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.OK)
                {
                    try
                    {
                        // Deletes Tasks before Lists
                        dbConn.ExecuteRawSql("DELETE FROM Tasks;");
                        dbConn.ExecuteRawSql("DELETE FROM Lists;");

                        // Reset auto-increment counters (safe only when DB is fully empty)
                        dbConn.ExecuteRawSql("DELETE FROM sqlite_sequence;");

                        // Compacts database
                        dbConn.ExecuteRawSql("VACUUM;");

                        // Refreshes UI
                        LoadTopics();
                        LoadTasks();
                        UpdateAddTaskButtonVisibility();

                        CheckIfPreviousNextTopicArrowButtonsUseful();

                        MessageBox.Show(LocalizationManager.GetString("deleteAllTopicsSuccess"), LocalizationManager.GetString("success"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Switches to Dates tab so the refreshed layout is visible
                        tabMain.SelectTab(tabDates);
                    }

                    catch
                    {
                        MessageBox.Show(LocalizationManager.GetString("deleteAllTopicsError"), LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                return;
            }

            // Normal behavior: delete only the selected topic
            Lists? currentTopic = cboTopics.SelectedItem as Lists;

            if (cboTopics.Items.Count == 0)
            {
                return;
            }

            var confirmResult = MessageBox.Show(LocalizationManager.GetString("delTopicWillRemoveRelTasks"),
                LocalizationManager.GetString("confirmDeletion"), MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.OK && currentTopic != null)
            {
                dbConn.DeleteTopic(currentTopic.Id);

                LoadTopics();
                UpdateAddTaskButtonVisibility();
                LoadTasks();

                if (cboTopics.Items.Count == 0)
                {
                    tabMain.SelectTab(tabDates);
                }
                else
                {
                    cboTopics.SelectedIndex = 0;
                }

                CheckIfPreviousNextTopicArrowButtonsUseful();
            }
        }

        private void cmdDeleteTopic_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cmdDeleteTopic_MouseLeave(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Export all data to a chronologically sorted HTML tabPage.
        /// </summary>
        private void cmdExportToHtml_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = LocalizationManager.GetString("exportHtmlFilter"),
                Title = LocalizationManager.GetString("exportHtmlTitle"),
                FileName = "LPM-data.html"
            };

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Reads active tasks
            List<Tasks> lstActiveTasks = dbConn.ReadTask("WHERE Status_id = 1;");
            int exportMode = Properties.Settings.Default.exportMode;

            // Parses and sorts by date ASC
            // Uses var to avoid anonymous type issues,
            // since a new object is created with Task and Date properties.
            var sortedTasks = lstActiveTasks
                .Select(task =>
                {
                    DateTime parsedDate;

                    // Universal date parser
                    // Extracts all digit groups from the deadline string
                    var parsedDigits = System.Text.RegularExpressions.Regex.Matches(task.Deadline, @"\d+")
                                .Cast<System.Text.RegularExpressions.Match>()
                                .Select(month => month.Value)
                                .ToList();

                    // Case 1: YYYY-MM-DD or similar (year has 4 digits)
                    if (parsedDigits.Count >= 3 && parsedDigits[0].Length == 4)
                    {
                        parsedDate = new DateTime(int.Parse(parsedDigits[0]), int.Parse(parsedDigits[1]), int.Parse(parsedDigits[2]));
                    }

                    // Case 2: DD-MM-YYYY or similar (year has 4 digits at the end)
                    else if (parsedDigits.Count >= 3 && parsedDigits[2].Length == 4)
                    {
                        int parsedYear = int.Parse(parsedDigits[2]);
                        int firstParsedNumber = int.Parse(parsedDigits[0]);
                        int secondParsedNumber = int.Parse(parsedDigits[1]);

                        // If the first number is > 12, it must be the day (DD-MM-YYYY)
                        // Otherwise, it's MM-DD-YYYY
                        bool firstParsedNumberIsDay = firstParsedNumber > 12;

                        int parsedDay = firstParsedNumberIsDay ? firstParsedNumber : secondParsedNumber;
                        int parsedMonth = firstParsedNumberIsDay ? secondParsedNumber : firstParsedNumber;

                        parsedDate = new DateTime(parsedYear, parsedMonth, parsedDay);
                    }

                    else
                    {
                        // Fallback to a general parsing attempt if the above patterns don't match
                        parsedDate = DateTime.TryParse(task.Deadline, out var dateTime) ? dateTime : DateTime.MinValue;
                    }

                    return new { Task = task, Date = parsedDate };
                })

                // Orders by the parsed date, with tasks that couldn't be parsed (DateTime.MinValue) appearing at the top
                .OrderBy(x => x.Date)
                .ToList();

            var sb = new StringBuilder();

            // HTML and CSS are generated with a simple design,
            // using light colors for better readability on a smartphone screen.
            sb.Append(@"
                    <html>
                    <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            background: #FFFFFF;
                            margin: 20px;
                        }

                        .year {
                            font-size: 22px;
                            font-weight: bold;
                            color: #355A62;
                            margin-top: 30px;
                            margin-bottom: 4px;
                        }

                        .date {
                            font-size: 16px;
                            color: #355A62;
                            margin-bottom: 10px;
                        }

                        .separator {
                            border-bottom: 1px solid #F0F0F0;
                            margin-top: 10px;
                            margin-bottom: 10px;
                        }

                        .category {
                            font-size: 14px;
                            color: #000000;
                            margin-bottom: 2px;
                        }

                        .title {
                            font-size: 14px;
                            color: #000000;
                            margin-bottom: 2px;
                        }

                        .title-important {
                            font-size: 14px;
                            font-weight: bold;
                            color: #000000;
                            margin-bottom: 2px;
                        }

                        .desc {
                            font-size: 14px;
                            color: #000000;
                            margin-bottom: 10px;
                        }
                    </style>
                    </head>
                    <body>
            ");

            // Local function to format the date according to the user's selected language
            string FormatDateLocalized(DateTime dateTimeToFormat)
            {
                string storedAppLanguageCode = Properties.Settings.Default.appLanguageCode;

                if (storedAppLanguageCode == "fr")
                {
                    return dateTimeToFormat.ToString("dd-MMM", new System.Globalization.CultureInfo("fr-FR"));
                }

                if (storedAppLanguageCode == "es")
                {
                    return dateTimeToFormat.ToString("dd-MMM", new System.Globalization.CultureInfo("es-ES"));
                }

                // Default: English
                return dateTimeToFormat.ToString("MMM-dd", System.Globalization.CultureInfo.InvariantCulture);
            }


            // Builds HTML content based on the export mode setting,
            // which determines which fields to include for each task.
            int currentYear = -1;

            foreach (var entry in sortedTasks)
            {
                var task = entry.Task;

                DateTime entryDateTime = entry.Date;
                string rawDate = task.Deadline;

                string formattedDate = (entryDateTime == DateTime.MinValue)
                    ? rawDate
                    : FormatDateLocalized(entryDateTime);

                int parsedYear = (entryDateTime == DateTime.MinValue) ? -1 : entryDateTime.Year;

                // Shows year only when it changes
                if (parsedYear != currentYear && parsedYear != -1)
                {
                    sb.Append($"<div class='year'>{parsedYear}</div>");
                    currentYear = parsedYear;
                }

                sb.Append($"<div class='date'>{formattedDate}</div>");

                string titleClass = (task.Priorities_id % 2 != 0)
                    ? "title-important"
                    : "title";

                // Gets the name of the topic associated with the task
                string? taskTopic = dbConn.ReadTopicName(task.Lists_id);

                switch (exportMode)
                {
                    // Only title
                    case 0:
                        sb.Append($"<div class='{titleClass}'>{task.Title}</div>");
                        break;

                    // Only title and description
                    case 1:
                        sb.Append($"<div class='{titleClass}'>{task.Title}</div>");
                        sb.Append($"<div class='desc'>{task.Description}</div>");
                        break;

                    // Only topic and title
                    case 2:
                        sb.Append($"<div class='category'>{taskTopic}</div>");
                        sb.Append($"<div class='{titleClass}'>{task.Title}</div>");
                        break;

                    // default : topic, title and description
                    default:
                        sb.Append($"<div class='category'>{taskTopic}</div>");
                        sb.Append($"<div class='{titleClass}'>{task.Title}</div>");
                        sb.Append($"<div class='desc'>{task.Description}</div>");
                        break;
                }

                // Separator line before next date or task, for better readability on smartphones
                sb.Append("<div class='separator'></div>");
            }

            sb.Append("</body></html>");

            try
            {
                // Writes the generated HTML content to the selected file path
                File.WriteAllText(saveFileDialog1.FileName, sb.ToString());

                MessageBox.Show(LocalizationManager.GetString("exportSqlSuccess"),
                    LocalizationManager.GetString("success"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }

        private void cmdExportToHtml_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cmdExportToHtml_MouseLeave(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Sets the date to the next day when the user clicks on the right arrow button
        /// </summary>
        private void cmdNextDay_Click(object sender, EventArgs e)
        {
            calMonth.SetDate(calMonth.SelectionStart.AddDays(1));
        }

        /// <summary>
        /// Shows the tasksFound for the next topic, from the drop-down list
        /// </summary>
        private void cmdNextTopic_Click(object sender, EventArgs e)
        {
            if (cboTopics.SelectedIndex < cboTopics.Items.Count - 1)
            {
                cboTopics.SelectedIndex += 1;
            }
            else
            {
                cboTopics.SelectedIndex = 0;
            }


            ApplyTopicHeaderResponsive();
        }

        /// <summary>
        /// Sets the date to the previous day when the user clicks on the left arrow button
        /// </summary>
        private void cmdPreviousDay_Click(object sender, EventArgs e)
        {
            calMonth.SetDate(calMonth.SelectionStart.AddDays(-1));
        }

        private void cmdNextDay_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cmdNextDay_MouseLeave(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Shows the tasksFound for the previous topic, from the drop-down list
        /// </summary>
        private void cmdPreviousTopic_Click(object sender, EventArgs e)
        {
            if (cboTopics.SelectedIndex > 0)
            {
                cboTopics.SelectedIndex -= 1;
            }
            else
            {
                cboTopics.SelectedIndex = cboTopics.Items.Count - 1;
            }

            ApplyTopicHeaderResponsive();
        }

        private void cmdPreviousDay_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cmdPreviousDay_MouseLeave(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Displays the search popup. 
        /// If the user holds the CTRL key while clicking, runs the testrunner instead
        /// and displays the results in the console.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSearchByKeywords_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                TestRunner.RunAll(new SmartSearch());
                return;
            }

            ShowSearchPopup();
        }

        /// <summary>
        /// Sets the date to today when the user clicks on the calendar button.
        /// </summary>
        private void cmdToday_Click(object sender, EventArgs e)
        {
            // Detects if we are currently in search mode
            bool isSearchLayout = lblToday.Text == LocalizationManager.GetString("SearchResults");

            if (isSearchLayout)
            {
                // If Today is already selected, force a temporary date change
                // so the calendar triggers DateChanged again
                if (calMonth.SelectionStart == DateTime.Today)
                {
                    calMonth.SetDate(DateTime.Today.AddDays(1));
                }

                // Sets the date to Today, which triggers the DateChanged event,
                // reloads today's tasks and restores the Today label
                calMonth.SetDate(DateTime.Today);
                return;
            }

            // Normal behavior when not in search mode:
            // - If Today is not selected, selects it
            // - If Today is already selected, does nothing
            if (calMonth.SelectionStart != DateTime.Today)
            {
                calMonth.SetDate(DateTime.Today);
            }
        }

        private void cmdToday_MouseEnter(object sender, EventArgs e)
        {

        }

        private void cmdToday_MouseLeave(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Creates a color‑tinted icon by applying a color matrix to a grayscale mask.
        /// White areas of the mask become tinted; black areas remain transparent.
        /// </summary>
        /// <param name="maskBitmap">The grayscale mask used as the base for tinting.</param>
        /// <param name="targetColor">The color applied to the mask.</param>
        /// <returns>A new bitmap containing the tinted icon.</returns>
        internal Bitmap CreateTintedIcon(Bitmap maskBitmap, Color targetColor)
        {
            Bitmap tintedBitmap = new Bitmap(maskBitmap.Width, maskBitmap.Height);

            float redFactor = targetColor.R / 255f;
            float greenFactor = targetColor.G / 255f;
            float blueFactor = targetColor.B / 255f;

            ColorMatrix tintMatrix = new ColorMatrix(new float[][]
            {
                new float[] { redFactor, 0, 0, 0, 0 },
                new float[] { 0, greenFactor, 0, 0, 0 },
                new float[] { 0, 0, blueFactor, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            });

            ImageAttributes imgAttributes = new ImageAttributes();
            imgAttributes.SetColorMatrix(tintMatrix);

            using (Graphics g = Graphics.FromImage(tintedBitmap))
            {
                // Syntax: DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, GraphicsUnit, ImageAttributes)
                g.DrawImage(maskBitmap, new Rectangle(0, 0, maskBitmap.Width, maskBitmap.Height),
                    0, 0, maskBitmap.Width, maskBitmap.Height, GraphicsUnit.Pixel, imgAttributes);
            }

            return tintedBitmap;
        }

        /// <summary>
        /// Asks the user for confirmation before deleting the currently selected task.
        /// If confirmed, the task is removed from the database and the UI is refreshed.
        /// </summary>
        public void DeleteTask_Click(object? sender, EventArgs e)
        {
            var selectedRow = GetSelectedTaskRow();

            if (selectedRow == null || dbConn == null)
            {
                return;
            }

            // Reloads the full task object
            Tasks? taskToDelete = dbConn.ReadTaskById(selectedRow.TaskId);

            if (taskToDelete == null)
            {
                return;
            }

            DialogResult userChoice = MessageBox.Show(LocalizationManager.GetString("areYouSureDeleteTheTask"),
                LocalizationManager.GetString("confirmDeletion"), MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (userChoice == DialogResult.OK)
            {
                dbConn.DeleteTask(taskToDelete.Id);
                LoadTasks();
            }
        }

        /// <summary>
        /// Opens the edit dialog for the currently selected task. 
        /// The task is reloaded from the database to ensure the dialog
        /// receives the latest data.
        /// </summary>
        public void EditTask_Click(object? sender, EventArgs e)
        {
            var selectedRow = GetSelectedTaskRow();

            if (selectedRow == null || dbConn == null)
            {
                return;
            }

            // Reloads the full task object
            Tasks? taskToEdit = dbConn.ReadTaskById(selectedRow.TaskId);
            if (taskToEdit == null)
            {
                return;
            }

            new frmEditTask(this, taskToEdit).ShowDialog();
        }

        /// <summary>
        /// Calculates which export mode to store in application settings
        /// based on the state of the checkboxes.
        /// </summary>
        private void ExportCheckboxesResult()
        {
            if (chkDescriptions.Checked == true && chkTopics.Checked == true)
            {
                Properties.Settings.Default.exportMode = 3;
            }
            else if (chkDescriptions.Checked == true && chkTopics.Checked == false)
            {
                Properties.Settings.Default.exportMode = 1;
            }
            else if (chkDescriptions.Checked == false && chkTopics.Checked == true)
            {
                Properties.Settings.Default.exportMode = 2;
            }
            else
            {
                // chkDescriptions and chkTopics are both false
                Properties.Settings.Default.exportMode = 0;
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles each timer tick during the fade‑in animation by gradually
        /// increasing the form's opacity until it becomes fully visible.
        /// </summary>
        private void FadeInTimer_Tick(object? sender, EventArgs e)
        {
            if (this.Opacity < 1)
            {
                this.Opacity += 0.06;
            }
            else
            {
                fadeInTimer.Stop();
            }
        }

        /// <summary>
        /// Keyboard shortcuts (global and task navigation)
        /// </summary>
        private void frmMain_KeyDown(object? sender, KeyEventArgs e)
        {
            // -------------------------
            // Global keyboard shortcuts
            // -------------------------

            // Ctrl+A to add a new task
            if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                cmdAddTask.PerformClick();
                return;
            }

            // Ctrl+B to open the birthday calendar
            if (e.KeyCode == Keys.B && e.Modifiers == Keys.Control)
            {
                cmdBirthdayCalendar.PerformClick();
                return;
            }

            // Ctrl+E to export all tasks to an HTML file
            if (e.KeyCode == Keys.E && e.Modifiers == Keys.Control)
            {
                cmdExportToHtml.PerformClick();
                return;
            }

            // Ctrl+F, Ctrl+K or Ctrl+S to open the search popup
            if ((e.KeyCode == Keys.F || e.KeyCode == Keys.K || e.KeyCode == Keys.S) && e.Control)
            {
                ShowSearchPopup();
                return;
            }

            // Ctrl+N or Alt+Right: move the calendar to next day
            if ((e.KeyCode == Keys.N && e.Modifiers == Keys.Control) ||
                (e.KeyCode == Keys.Right && e.Modifiers == Keys.Alt))
            {
                cmdNextDay.PerformClick();
                return;
            }

            // Ctrl+P or Alt+Left: move the calendar to previous day
            if ((e.KeyCode == Keys.P && e.Modifiers == Keys.Control) ||
                (e.KeyCode == Keys.Left && e.Modifiers == Keys.Alt))
            {
                cmdPreviousDay.PerformClick();
                return;
            }

            // Ctrl+T or Ctrl+D: move the calendar to today
            if ((e.KeyCode == Keys.T || e.KeyCode == Keys.D) && e.Modifiers == Keys.Control)
            {
                cmdToday.PerformClick();
                return;
            }

            // Shift+W or Alt+Up: move the calendar of 7 days backward
            if ((e.KeyCode == Keys.W && e.Modifiers == Keys.Shift) ||
                (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt))
            {
                calMonth.SetDate(calMonth.SelectionStart.AddDays(-7));
                return;
            }

            // Ctrl+W or Alt+Down: move the calendar of 7 days forward
            if ((e.KeyCode == Keys.W && e.Modifiers == Keys.Control) ||
                (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt))
            {
                calMonth.SetDate(calMonth.SelectionStart.AddDays(+7));
                return;
            }

            // Shift+Del: delete all finished tasks
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Shift)
            {
                if (tabMain.SelectedTab == tabFinished && cmdDeleteFinishedTasks.Visible)
                {
                    cmdDeleteFinishedTasks.PerformClick();
                }
                return;
            }

            // Task navigation and task-level actions
            HandleTaskNavigationKey(e);
        }

        /// <summary>
        /// When the form is shown, loads all the topics and tasksFound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Shown(object sender, EventArgs e)
        {
            LoadTopics();
            UpdateAddTaskButtonVisibility();
            LoadTasks();
            ApplyResponsiveLayout();
        }

        /// <summary>
        /// Resizes the tasksFound gradientWidth in the different panels when the form is resized 
        /// by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_SizeChanged(object? sender, EventArgs e)
        {
            // Ignores resize events fired before the window handle exists (startup phase)
            if (!this.IsHandleCreated)
            {
                return;
            }

            // Always resize the tasksFound layout
            ApplyResponsiveLayout();

            // Responsive layout for Settings
            if (tabMain.SelectedTab == tabSettings)
            {
                ApplySettingsLayout();
            }

            // Saves window gradientWidth
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Gets a list of all unique words contained in the title and description of all tasksFound.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllWordsFromTasks()
        {
            // Retrieves all tasks directly from the database
            List<Tasks> allTasks = dbConn.ReadTask("");

            HashSet<string> words = new HashSet<string>();

            foreach (var task in allTasks)
            {
                string text = (task.Title + " " + task.Description).ToLowerInvariant();

                foreach (string word in text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    words.Add(word);
                }
            }

            return words.ToList();
        }

        /// <summary>
        /// Returns the localized currentDateLabel for the currently selected date
        /// (today, yesterday, tomorrow, etc.).
        /// </summary>
        private string? GetCurrentDateLabel()
        {
            DateTime selectedDate = calMonth.SelectionStart;
            DateTime today = DateTime.Today;

            if (selectedDate == today.AddDays(-2))
            {
                return LocalizationManager.GetString("twoDaysAgo");
            }

            if (selectedDate == today.AddDays(-1))
            {
                return LocalizationManager.GetString("yesterday");
            }

            if (selectedDate == today)
            {
                return LocalizationManager.GetString("today");
            }

            if (selectedDate == today.AddDays(1))
            {
                return LocalizationManager.GetString("tomorrow");
            }

            if (selectedDate == today.AddDays(2))
            {
                return LocalizationManager.GetString("dayAfterTomorrow");
            }

            // For dates beyond ±2 days: returns null to indicate “just show the date”
            return null;
        }

        /// <summary>
        /// Gets the panel that contains the task row with the specified TaskId.
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private Panel? GetPanelForTask(int taskId)
        {
            foreach (var keyValuePair in selectionByPanel)
            {
                Panel candidatePanel = keyValuePair.Key;
                var rows = keyValuePair.Value;

                foreach (var row in rows)
                {
                    if (row.TaskId == taskId)
                    {
                        return candidatePanel;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Scans the selection structure for the row whose TaskId matches the stored selection.
        /// </summary>
        /// <returns> The SelectableTaskRow corresponding to the currently selected task,
        /// or null if no task is selected. </returns>
        public SelectableTaskRow? GetSelectedTaskRow()
        {
            // No task is currently selected
            if (selectedTaskId < 0)
            {
                return null;
            }

            // Searches through all panels and their registered rows
            foreach (var panelEntry in selectionByPanel)
            {
                foreach (var taskRow in panelEntry.Value)
                {
                    if (taskRow.TaskId == selectedTaskId)
                    {
                        return taskRow;
                    }
                }
            }

            // No matching row found
            return null;
        }

        /// <summary>
        /// Handles keyboard navigation (Up/Down) and task actions (Enter, Space, Delete)
        /// for the currently selected task inside its owning panel.
        /// </summary>
        private void HandleTaskNavigationKey(KeyEventArgs e)
        {
            // No task selected: nothing to navigate
            if (selectedTaskId == -1)
            {
                return;
            }

            // Always use the panel where the last selection was made
            Panel? activePanel = GetPanelForTask(selectedTaskId);

            if (activePanel == null)
            {
                return;
            }

            // Ensures the active panel is valid and contains at least one row.
            // If the panel is missing or empty, keyboard navigation cannot continue.
            if (!selectionByPanel.ContainsKey(activePanel) ||
                selectionByPanel[activePanel].Count == 0)
            {
                return;
            }

            var selectionList = selectionByPanel[activePanel];

            if (selectionList == null || selectionList.Count == 0)
            {
                return;
            }

            // If no task is still selected or the ID is no longer found,
            // the first line of the active panel is properly selected.
            int indexSelectedTask = selectionList.FindIndex(row => row.TaskId == selectedTaskId);

            if (selectedTaskId == -1 || indexSelectedTask == -1)
            {
                selectedTaskId = selectionList[0].TaskId;
                ToggleSelection(selectedTaskId);
                return;
            }

            // Arrow Up : selects previous task (circular)
            if (e.KeyCode == Keys.Up && e.Modifiers == Keys.None)
            {
                if (indexSelectedTask > 0)
                {
                    selectedTaskId = selectionList[indexSelectedTask - 1].TaskId;
                }
                else
                {
                    // If first one : wraps to last
                    selectedTaskId = selectionList[selectionList.Count - 1].TaskId;
                }

                ToggleSelection(selectedTaskId);
                return;
            }

            // Arrow Down : selects next task (circular)
            if (e.KeyCode == Keys.Down && e.Modifiers == Keys.None)
            {
                if (indexSelectedTask < selectionList.Count - 1)
                {
                    selectedTaskId = selectionList[indexSelectedTask + 1].TaskId;
                }
                else
                {
                    // If last one : wraps to first
                    selectedTaskId = selectionList[0].TaskId;
                }

                ToggleSelection(selectedTaskId);
                return;
            }

            // Enter or V to approve a selected task
            if ((e.KeyCode == Keys.Enter && e.Modifiers == Keys.None) ||
                (e.KeyCode == Keys.V && e.Modifiers == Keys.None))
            {
                string validationDate = DateTime.Today.ToString("yyyy-MM-dd");
                dbConn.ApproveTask(selectedTaskId, validationDate);
                LoadTasks();
                return;
            }

            // Space, E or M to edit a selected task
            if ((e.KeyCode == Keys.Space || e.KeyCode == Keys.E || e.KeyCode == Keys.M) && e.Modifiers == Keys.None)
            {
                if (dbConn == null)
                {
                    return;
                }

                Tasks? taskToEdit = dbConn.ReadTaskById(selectedTaskId);

                if (taskToEdit != null)
                {
                    new frmEditTask(this, taskToEdit).ShowDialog();
                }

                return;
            }

            // Delete key to delete a selected task
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.None)
            {
                DialogResult result = MessageBox.Show(LocalizationManager.GetString("areYouSureDeleteTheTask"),
                    LocalizationManager.GetString("confirmDeletion"), MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    dbConn.DeleteTask(selectedTaskId);
                    LoadTasks();
                }

                return;
            }
        }

        public void HideTaskDescription()
        {
            lblTaskDescription.Visible = false;
        }

        /// <summary>
        /// Configures the fade‑in animation used when the form is shown.
        /// </summary>
        private void InitializeFadeInAnimation()
        {
            // Only start transparent if fade‑in is enabled
            if (_enableFadeIn)
            {
                this.Opacity = 0;
            }

            // Timer used to perform the fade‑in animation when the form is shown.
            fadeInTimer.Interval = 8;
            fadeInTimer.Tick += FadeInTimer_Tick;
        }

        private void lnkAppInLanguage_LinkClicked(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
        }

        /// <summary>
        /// Opens the form to customize application colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkChangeAppColors_LinkClicked(object sender, EventArgs e)
        {
            using (var _frmCustomizeAppColors = new frmCustomizeAppColors(this))
            {
                _frmCustomizeAppColors.ShowDialog(this);
            }
        }

        /// <summary>
        /// Exports all lists and tasks into a .sql file
        /// </summary>
        private void lnkExportTasksToSql_LinkClicked(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = LocalizationManager.GetString("exportSqlFilter"),
                Title = LocalizationManager.GetString("exportSqlTitle"),
                FileName = "LPM-tasks.sql"
            };

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            List<Lists> lstTopics = dbConn.ReadTopics();
            List<Tasks> lstTasks = dbConn.ReadTask("");

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("BEGIN TRANSACTION;");
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("-- Lists");
            foreach (var topic in lstTopics)
            {
                string taskTitle = (topic.Title ?? "").Replace("'", "''");
                stringBuilder.AppendLine($"INSERT INTO Lists (id, title) VALUES ({topic.Id}, '{taskTitle}');");
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("-- Tasks");

            foreach (var task in lstTasks)
            {
                string taskTitle = (task.Title ?? "").Replace("'", "''");
                string taskDescription = (task.Description ?? "").Replace("'", "''");

                string taskDeadline = string.IsNullOrWhiteSpace(task.Deadline)
                    ? "NULL" : $"'{task.Deadline}'";

                string taskValidationDate = string.IsNullOrWhiteSpace(task.ValidationDate)
                    ? "NULL" : $"'{task.ValidationDate}'";

                stringBuilder.AppendLine(
                    "INSERT INTO Tasks VALUES (" +
                    $"{task.Id}, " +
                    $"'{taskTitle}', " +
                    $"'{taskDescription}', " +
                    $"{taskDeadline}, " +
                    $"{taskValidationDate}, " +
                    $"{task.Priorities_id}, " +
                    $"{task.Lists_id}, " +
                    $"{task.Status_id}" +
                    ");"
                );
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("COMMIT;");

            try
            {
                File.WriteAllText(saveFileDialog1.FileName, stringBuilder.ToString());

                MessageBox.Show(LocalizationManager.GetString("exportSqlSuccess"),
                    LocalizationManager.GetString("success"), MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(LocalizationManager.GetString("exportSqlError"), ex.Message),
                    LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Opens a dialog to insert tasks from a SQL script file.
        /// Executes the entire script in one block.
        /// </summary>
        private void lnkInsertTasksFromSql_LinkClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = LocalizationManager.GetString("importSqlFilter");
                dialog.Title = LocalizationManager.GetString("importSql");

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string sqlContent = File.ReadAllText(dialog.FileName);

                try
                {
                    dbConn.ExecuteRawSql(sqlContent);

                    MessageBox.Show(LocalizationManager.GetString("importSqlSuccess"), LocalizationManager.GetString("success"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadTasks();
                    LoadTopics();

                    if (cboTopics.Items.Count > 0)
                    {
                        cboTopics.SelectedIndex = 0;
                        tabMain.SelectTab(tabDates);
                        UpdateAddTaskButtonVisibility();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LocalizationManager.GetString("importSqlError") + "\n\n" + ex.Message,
                        LocalizationManager.GetString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Loads all the tasksFound in the finished tab
        /// </summary>
        public void LoadDoneTasks()
        {
            // Updates tasksFound that are done
            List<Tasks> tasksList = dbConn.ReadApprovedTask();
            layoutBuilder.CreateTasksLayout(tasksList, LayoutBuilder.LayoutType.Finished);

            cmdDeleteFinishedTasks.Visible = (pnlFinished.Controls.Count > 0);
        }

        /// <summary>
        /// Loads all the localized strings for the UI elements based on the current language setting.
        /// </summary>
        public void LoadLocalizedStrings()
        {
            // --- Tabs ---
            tabDates.Text = LocalizationManager.GetString("tabDatesText");
            tabTopics.Text = LocalizationManager.GetString("tabTopicsText");
            tabFinished.Text = LocalizationManager.GetString("tabFinishedText");
            tabSettings.Text = LocalizationManager.GetString("tabSettingsText");

            // --- Labels ---
            string? label = GetCurrentDateLabel();
            DateTime selectedDate = calMonth.SelectionStart;

            if (label == null)
            {
                // Beyond ±2 days → show only the date
                lblToday.Text = selectedDate.ToString("d", CultureInfo.CurrentUICulture);
            }
            else
            {
                lblToday.Text = $"{label} ({selectedDate.ToString("d", CultureInfo.CurrentUICulture)})";
            }

            lblWeek.Text = LocalizationManager.GetString("nextDays");
            lblTopic.Text = LocalizationManager.GetString("topic");
            lblExportDeadlineAndTitle.Text = LocalizationManager.GetString("exportDeadlineAndTitle");
            lblTaskDescriptionFontSize.Text = LocalizationManager.GetString("taskDescriptionFontSizeText");

            // -- Links ---
            lnkChangeAppColors.Text = LocalizationManager.GetString("lnkChangeAppColorsText");
            lnkInsertTasksFromSql.Text = LocalizationManager.GetString("lnkInsertTasksFromSqlText");
            lnkExportTasksToSql.Text = LocalizationManager.GetString("lnkExportTasksToSqlText");
            lnkAppInLanguage.Text = LocalizationManager.GetString("appInLanguage");

            // --- Checkboxes ---
            chkTopics.Text = LocalizationManager.GetString("chkTopicsText");
            chkDescriptions.Text = LocalizationManager.GetString("chkDescriptionsText");
            chkRunAtWindowsStartup.Text = LocalizationManager.GetString("chkRunAtWindowsStartupText");

            // --- ComboBox: language selection ---
            ApplyLanguageComboBoxItems();

            // --- Context menu: tasks options ---
            ValidateTask.Text = LocalizationManager.GetString("ValidateTask");
            EditTask.Text = LocalizationManager.GetString("EditTask");
            DeleteTask.Text = LocalizationManager.GetString("DeleteTask");
            ReassignTask.Text = LocalizationManager.GetString("ReassignTask");
        }

        /// <summary>
        /// Reloads all task lists for every tab (Today, Next 7 Days, Topics, Done)
        /// and refreshes the calendar bold dates and task counters.
        /// </summary>
        public void LoadTasks()
        {
            // Resets selected task and clears any visual selection
            selectedTaskId = -1;
            lblTaskDescription.Visible = false;

            // Hides description panel
            lblTaskDescription.Visible = false;

            // Reloads each layout based on the current reference date
            LoadTasksForDate();
            LoadTasksForTodayPlusSeven();
            LoadTasksInTopic();
            LoadDoneTasks();

            // Refreshes bolded dates in the calendar
            calMonth.RemoveAllBoldedDates();
            SetDatesInBold();

            // Updates total tasksFound counter
            nbTasksToComplete = dbConn.CountTotalTasksToComplete();

            ttpTotalTasksToComplete.SetToolTip(cmdExportToHtml,
                LocalizationManager.GetString("totalTasksToComplete") + " " + nbTasksToComplete.ToString()
            );
        }

        /// <summary>
        /// Loads all the tasksFound for today in the dates tab
        /// </summary>
        public void LoadTasksForDate()
        {
            List<Tasks> tasksList = dbConn.ReadTaskForDate(selectedDateString);
            layoutBuilder.CreateTasksLayout(tasksList, LayoutType.Today);

            // Resets scroll state
            pnlToday.AutoScrollMinSize = Size.Empty;
            pnlToday.VerticalScroll.Value = 0;

            pnlToday.PerformLayout();
            pnlToday.Invalidate();
        }

        /// <summary>
        /// Loads all the tasksFound for the next 7 days in the dates tab
        /// </summary>
        public void LoadTasksForTodayPlusSeven()
        {
            // Updates tasksFound for the next seven days
            List<Tasks> tasksList = dbConn.ReadTaskForDatePlusSeven(plusSevenDays);
            layoutBuilder.CreateTasksLayout(tasksList, LayoutType.Week);
        }

        /// <summary>
        /// Loads all the tasksFound in the topics tab
        /// </summary>
        public void LoadTasksInTopic()
        {
            // Gets the selected topic
            Lists? currentTopic = cboTopics.SelectedItem as Lists;

            // If a topic has been selected in the topic combobox
            if (currentTopic != null)
            {
                // Updates the currentDateLabel
                lblTopic.Text = currentTopic.Title;

                // Updates the tasksFound for the current topic
                List<Tasks> tasksList = dbConn.ReadTaskForTopic(currentTopic.Id);
                layoutBuilder.CreateTasksLayout(tasksList, LayoutType.Topics);
            }
        }

        /// <summary>
        /// Loads all the topics in the drop-down list on the right panel
        /// </summary>
        public void LoadTopics()
        {
            cboTopics.Items.Clear();

            foreach (Lists topic in dbConn.ReadTopics())
            {
                cboTopics.Items.Add(topic);
            }

            CheckIfPreviousNextTopicArrowButtonsUseful();

            cboTopics.Text = LocalizationManager.GetString("displayByTopic");
        }

        /// <summary>
        /// Gets the new font size for the task description from the numeric up-down control 
        /// and applies it immediately to the label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudTaskDescriptionFontSize_ValueChanged(object? sender, EventArgs e)
        {
            int newFontSize = (int)nudTaskDescriptionFontSize.Value;
            lblTaskDescription.Font = new Font(lblTaskDescription.Font.FontFamily, newFontSize);

            Properties.Settings.Default.taskDescriptionFontSize = newFontSize;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Overrides the form's lifecycle to start the fade‑in animation as soon as
        /// the window becomes visible, when explicitly enabled.
        /// This provides a smooth transition ffect without blocking the UI thread.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (_enableFadeIn)
            {
                fadeInTimer.Start();
            }
        }

        /// <summary>
        /// Applies responsive layout adjustments to the topic header, so that it
        /// aligns properly and remains user-friendly even when the window is resized to smaller widths.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlTopics_Resize(object? sender, EventArgs e)
        {
            ApplyTopicHeaderResponsive();
        }

        /// <summary>
        /// Removes the validation date from the currently selected task, 
        /// moving it back to the active task lists.
        /// </summary>
        public void ReassignTask_Click(object? sender, EventArgs e)
        {
            var selectedRow = GetSelectedTaskRow();

            if (selectedRow == null || dbConn == null)
            {
                return;
            }

            // Reloads the full task object from the database.
            Tasks? taskToReassign = dbConn.ReadTaskById(selectedRow.TaskId);

            if (taskToReassign == null)
            {
                return;
            }

            dbConn.UnapproveTask(taskToReassign.Id);
            LoadTasks();
        }

        /// <summary>
        /// Applies theme colors to all dynamically created task rows.
        /// </summary>
        private void RecolorDynamicTaskRows()
        {
            Color titleColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText);
            Color panelColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfPanelsBackground);

            RecolorPanelRows(pnlToday, titleColor, panelColor);
            RecolorPanelRows(pnlWeek, titleColor, panelColor);
            RecolorPanelRows(pnlTopics, titleColor, panelColor);
            RecolorPanelRows(pnlFinished, titleColor, panelColor);
        }

        /// <summary>
        /// Recolors all rows in a given panel, applying the specified title and background colors.
        /// </summary>
        /// <param name="parentControl"></param>
        /// <param name="titleColor"></param>
        /// <param name="panelBackColor"></param>
        private void RecolorPanelRows(Panel parentControl, Color titleColor, Color panelBackColor)
        {
            foreach (Control ctrl in parentControl.Controls)
            {
                if (ctrl is Panel rowPanel)
                {
                    rowPanel.BackColor = Color.Transparent; // tu veux garder transparent
                    foreach (Control sub in rowPanel.Controls)
                    {
                        if (sub is Label lbl)
                            lbl.ForeColor = titleColor;
                    }
                }
            }
        }

        /// <summary>
        /// Reapplies the themed Validate icon to all Validate buttons
        /// currently displayed in the four task panels.
        /// </summary>
        internal void RefreshAllValidateButtons()
        {
            RefreshValidateButtonsInPanel(pnlToday);
            RefreshValidateButtonsInPanel(pnlWeek);
            RefreshValidateButtonsInPanel(pnlTopics);
            RefreshValidateButtonsInPanel(pnlFinished);
        }

        /// <summary>
        /// Reapplies the themed Validate icon to all Validate buttons
        /// inside a specific task panel.
        /// </summary>
        private void RefreshValidateButtonsInPanel(Panel taskPanel)
        {
            foreach (Control row in taskPanel.Controls)
            {
                if (row is Panel taskRowPanel)
                {
                    foreach (Control ctrl in taskRowPanel.Controls)
                    {
                        // Identifies Validate buttons
                        if (ctrl is Button btnValidate && btnValidate.Dock == DockStyle.Right &&
                            btnValidate.Width == LayoutBuilder.BUTTON_SIZE && btnValidate.Height == LayoutBuilder.BUTTON_SIZE)
                        {
                            // Reapplies the dynamic color tint
                            ApplyValidateButtonColor(btnValidate);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reloads only the UI elements affected by the updated color setting.
        /// This avoids unnecessary full-window redraws and keeps the UI responsive.
        /// </summary>
        internal void ReloadUI(string settingKey)
        {
            if (_isInitializing)
            {
                return;
            }

            // Main window elements
            if (settingKey == "colorOfMainWindowBackground" ||
                settingKey == "colorOfRightPanelBackground" ||
                settingKey == "colorOfInputFieldsBackground" ||
                settingKey == "colorOfInputFieldsText" ||
                settingKey == "colorOfLblTaskDescriptionBackground" ||
                settingKey == "colorOfLblTaskDescriptionText" ||
                settingKey == "colorOfTaskTitlesBackground" ||
                settingKey == "colorOfTaskTitlesText" ||
                settingKey == "colorOfValidateButton")
            {
                ApplyStoredColors();
                LoadTasks();
                return;
            }

            // Birthday calendar window
            if (settingKey == "colorOfBirthdayCalendarBackground" ||
                settingKey == "colorOfBirthdayCalendarSquaresPrimary" ||
                settingKey == "colorOfBirthdayCalendarSquaresSecondary" ||
                settingKey == "colorOfBirthdayCalendarSquaresTertiary" ||
                settingKey == "colorOfBirthdayCalendarText")
            {
                Form? birthdayCalendarForm = Application.OpenForms["frmBirthdayCalendar"];

                if (birthdayCalendarForm != null)
                {
                    // Forces the calendar to redraw using the updated colors
                    birthdayCalendarForm.Invalidate();
                }
                return;
            }

            // Secondary windows (Add task, Edit task, Topics, Customize colors)
            if (settingKey == "colorOfSecondaryWindowsBackground")
            {
                ApplyColorsToSecondaryWindows();
                return;
            }
        }

        /// <summary>
        /// Clears the current selection and hides the task description.
        /// Used when reloading layouts or resetting panels.
        /// </summary>
        internal void ResetSelection()
        {
            selectedTaskId = -1;
            lblTaskDescription.Visible = false;
        }

        /// <summary>
        /// Repositions all task-related buttons and resizes the task currentDateLabel 
        /// on each row so that buttons always remain visible inside the panel, 
        /// even when the window shrinks.
        /// The task currentDateLabel expands to fill all remaining horizontal space when the window grows.
        /// </summary>
        /// <param name="panel">Panel containing dynamically created task controls.</param>
        /// <summary>
        private void ResizeTasksInPanel(Panel panel)
        {
            // Total horizontal space available inside this task panel
            int availablePanelWidth = panel.ClientSize.Width;

            // Space to keep between the rightmost button and the panel border
            int rightSidePadding = 20;

            // Horizontal spacing between adjacent buttons
            int buttonHorizontalSpacing = 10;

            // Groups all controls by their vertical position.
            // Each unique "Top" value corresponds to one logical task row.
            var taskRows = panel.Controls
                                .Cast<Control>()
                                .GroupBy(control => control.Top)
                                .ToList();

            foreach (var row in taskRows)
            {
                // Identifies the main task currentDateLabel for this row.
                Label? taskLabel = row.OfType<Label>()
                    .Where(label => label.Height == 25 && label.Left > 40)
                    .FirstOrDefault();

                // If no task currentDateLabel is found, this row is not a task row, so we skip it.
                if (taskLabel == null)
                {
                    continue;
                }

                // Collects all buttons on this row sorting by their original Left
                // ensures a predictable right-to-left order.
                var taskRowButtons = row
                    .OfType<Button>()
                    .OrderByDescending(button => button.Left)
                    .ToList();

                // Starting X coordinate for the rightmost button.
                // Buttons will be placed from right to left.
                int nextButtonRightEdge = availablePanelWidth - rightSidePadding;

                // Repositions each button from right to left.
                foreach (Button button in taskRowButtons)
                {
                    // Aligns vertically with the task currentDateLabel (same row)
                    button.Top = taskLabel.Top;

                    // Positions the button so its right edge aligns with nextButtonRightEdge
                    button.Left = nextButtonRightEdge - button.Width;

                    // Moves the cursor left for the next button
                    nextButtonRightEdge = button.Left - buttonHorizontalSpacing;
                }

                // Now resizes the task currentDateLabel so it fills all remaining space
                // between its original Left and the first button.
                int maximumLabelRightEdge = nextButtonRightEdge;

                // Prevents the currentDateLabel from collapsing too much
                int minimumLabelWidth = 50;

                // Computes the new gradientWidth for the task currentDateLabel
                int computedLabelWidth = maximumLabelRightEdge - taskLabel.Left;

                if (computedLabelWidth < minimumLabelWidth)
                {
                    computedLabelWidth = minimumLabelWidth;
                }

                taskLabel.Width = computedLabelWidth;
            }
        }

        /// <summary>
        /// Restores the gradientWidth of the window from the settings when the application is launched.
        /// </summary>
        private void RestoreWindowWidthFromSettings()
        {
            int savedWidth = Properties.Settings.Default.WindowWidth;

            if (savedWidth > 0)
            {
                this.Width = savedWidth;
            }
        }

        /// <summary>
        /// Sets the dates of the calendar in bold when there's one or more deadline for a task on a given day
        /// </summary>
        private void SetDatesInBold()
        {
            var deadlinesList = dbConn.ReadDataForDeadlines();

            foreach (var item in deadlinesList)
            {
                if (DateTime.TryParseExact(item, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var date))
                {
                    calMonth.AddBoldedDate(date);
                }
            }

            calMonth.UpdateBoldedDates();
        }

        /// <summary>
        /// Displays a lightweight popup search box using ToolStripDropDown.
        /// Auto-closes when clicking outside and triggers SmartSearch() on Enter.
        /// </summary>
        private void ShowSearchPopup()
        {
            // Textbox inside popup
            TextBox txtKeywords = new TextBox
            {
                Width = 190,
                BackColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsBackground),
                ForeColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfInputFieldsText),
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 150,
                Padding = new Padding(8, 0, 8, 0),
                Margin = new Padding(4, 4, 4, 4),
                Font = new Font("Segoe UI", 10),
            };

            // Button inside popup
            Button cmdPopupSearch = new Button
            {
                Text = LocalizationManager.GetString("Search"),
                BackColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfPanelsBackground),
                ForeColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfTaskTitlesText),
                AutoSize = true,
                Cursor = Cursors.Hand,
                Margin = new Padding(4, 4, 0, 0)
            };

            // Popup container with the same background as the main window
            ToolStripDropDown tlstrpDropDown = new ToolStripDropDown
            {
                Width = txtKeywords.Width + 30,
                Padding = new Padding(2, 1, 2, 2),
                BackColor = ColorTranslator.FromHtml(Properties.Settings.Default.colorOfMainWindowBackground)
            };

            // Enter triggers the button click
            txtKeywords.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                {
                    ev.Handled = true;
                    ev.SuppressKeyPress = true;

                    cmdPopupSearch.PerformClick();
                }
            };

            // Prevents popup from closing when typing AltGr (for ñ, accents, etc.)
            txtKeywords.PreviewKeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Alt ||
                    ev.KeyCode == Keys.Menu ||
                    ev.KeyCode == Keys.ControlKey)
                {
                    ev.IsInputKey = true;
                }
            };

            // Button click triggers the search and updates the UI with results
            cmdPopupSearch.Click += (s, ev) =>
            {
                List<Tasks> lstTasksFound = smartSearch.Search(txtKeywords.Text);

                tlstrpDropDown.Close();
                tabMain.SelectedTab = tabDates;

                lblToday.Text = LocalizationManager.GetString("SearchResults");

                // If no results are found, creates a dummy task with a "No results found"
                // message to display in the UI
                if (lstTasksFound == null || lstTasksFound.Count == 0)
                {
                    Tasks noResultTask = new Tasks();
                    noResultTask.Id = -1;
                    noResultTask.Title = LocalizationManager.GetString("NoResultsFound");
                    noResultTask.Description = string.Empty;
                    noResultTask.Deadline = DateTime.Today.ToString("yyyy-MM-dd");

                    lstTasksFound = new List<Tasks>();
                    lstTasksFound.Add(noResultTask);
                }

                layoutBuilder.CreateTasksLayout(lstTasksFound, LayoutType.Search);
            };

            // Host controls
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                BorderStyle = BorderStyle.None,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            // Adds controls to panel
            panel.Controls.Add(txtKeywords);
            panel.Controls.Add(cmdPopupSearch);

            ToolStripControlHost host = new ToolStripControlHost(panel)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            // Adds host to dropdown control
            tlstrpDropDown.Items.Add(host);

            tlstrpDropDown.AutoSize = true;
            tlstrpDropDown.PerformLayout();
            tlstrpDropDown.Height = host.Size.Height;

            // Button screen position
            Point buttonScreenPos = cmdSearch.PointToScreen(Point.Empty);

            // Centers horizontally under the button
            int centeredPosX = buttonScreenPos.X + (cmdSearch.Width / 2) - (tlstrpDropDown.Width / 2) - 55;

            // Vertical position just below the button
            int y = buttonScreenPos.Y + cmdSearch.Height + 18;

            // Final popup position
            Point finalPos = new Point(centeredPosX, y);

            // Shows popup at corrected position
            tlstrpDropDown.Show(finalPos);

            txtKeywords.Focus();
        }

        /// <summary>
        /// Handles the event when the user selects a tab
        /// </summary>
        private void tabMain_Selected(object sender, EventArgs e)
        {
            selectedTaskId = -1;
            lblTaskDescription.Visible = false;
            lblTaskDescription.Text = "";

            if (tabMain.SelectedTab == tabTopics)
            {
                if (cboTopics.Items.Count == 0)
                {
                    cmdAddTopic.PerformClick();
                    tabMain.SelectTab(tabDates);
                    return;
                }

                if (cboTopics.SelectedIndex == -1)
                {
                    // Clears the placeholder text so the ComboBox can accept a real selection
                    cboTopics.Text = string.Empty;

                    // Selects the first topic, which will trigger SelectedIndexChanged
                    cboTopics.SelectedIndex = 0;
                }

                CheckIfPreviousNextTopicArrowButtonsUseful();
            }

            // When switching to the finished tab, shows the "Delete finished tasks" button only if there are finished tasks to delete.
            else if (tabMain.SelectedTab == tabFinished)
            {
                if (dbConn.ReadApprovedTask().Count > 0)
                {
                    cmdDeleteFinishedTasks.Visible = true;
                }
            }

            // When switching to the settings tab, applies the settings layout to ensure controls are properly positioned.
            else if (tabMain.SelectedTab == tabSettings)
            {
                ApplySettingsLayout();
            }
        }

        /// <summary>
        /// Ensures that the topic header and its buttons are properly aligned and responsive 
        /// when the topics tab is selected or when the form is resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabTopics_Layout(object sender, LayoutEventArgs e)
        {
            ApplyTopicHeaderResponsive();
        }

        /// <summary>
        /// Ensures that the settings controls are properly positioned and resized when the
        /// settings tab is selected or when the form is resized while the settings tab is active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabSettings_Layout(object sender, LayoutEventArgs e)
        {
            ApplySettingsLayout();
        }

        /// <summary>
        /// Updates the visual selection state across all panels.
        /// Highlights the selected task in the active panel, clears all others,
        /// updates the task description area, and ensures the selected row
        /// is scrolled into view.
        /// This method is the single source of truth for selection state.
        /// </summary>
        /// <param name="selectedTaskId">The ID of the task to select.</param>
        internal void ToggleSelection(int selectedTaskId)
        {
            // Determines the active panel dynamically
            Panel? activePanel = GetPanelForTask(selectedTaskId);

            // Updates global selection state
            this.selectedTaskId = selectedTaskId;

            // Iterates through all panels registered in the selection dictionary
            foreach (var keyValuePair in selectionByPanel)
            {
                Panel selDictPanel = keyValuePair.Key;
                var selDictRows = keyValuePair.Value;

                // Iterates through all rows of the current panel
                foreach (var selDictRow in selDictRows)
                {
                    bool isSelected = (selDictRow.TaskId == selectedTaskId);

                    if (isSelected && selDictPanel == activePanel)
                    {
                        // Safety guard to silence nullability warnings
                        if (selDictRow?.TitleLabel == null)
                        {
                            continue;
                        }

                        // Applies highlight to the selected row (only in the active panel)
                        selDictRow.TitleLabel.BackColor = Color.FromArgb(168, 208, 230);
                        selDictRow.TitleLabel.ForeColor = Color.Black;

                        // Birthday tasks hide the description to avoid exposing the birth year
                        if (selDictRow.Priority != 4 && !string.IsNullOrEmpty(selDictRow.Description))
                        {
                            lblTaskDescription.Text = selDictRow.Description;
                            lblTaskDescription.Visible = true;
                        }
                        else
                        {
                            lblTaskDescription.Visible = false;
                        }

                        // Find the actual row container inside the active panel,
                        // regardless of how many nested panels exist in Topics.
                        Control? rowPanel = selDictRow.TitleLabel;

                        while (rowPanel != null && rowPanel.Parent != activePanel)
                        {
                            rowPanel = rowPanel.Parent;
                        }

                        if (rowPanel != null)
                        {
                            activePanel.ScrollControlIntoView(rowPanel);
                        }
                    }
                    else
                    {
                        if (selDictRow?.TitleLabel != null)
                        {
                            // Clears highlight for all non-selected rows (all panels)
                            selDictRow.TitleLabel.BackColor = Color.Transparent;
                            selDictRow.TitleLabel.ForeColor = Color.Black;
                        }
                    }
                }
            }

            // If no task is selected, hides the description area
            if (selectedTaskId == -1)
            {
                lblTaskDescription.Visible = false;
            }
        }

        public void TriggerTodayClick()
        {
            cmdToday.PerformClick();
        }

        /// <summary>
        /// Updates the visibility of the "Add Task" button based on whether at least one topic exists.
        /// The button is hidden when no topics are available, preventing users from attempting to
        /// create tasks before defining a topic.
        /// </summary>
        public void UpdateAddTaskButtonVisibility()
        {
            cmdAddTask.Visible = cboTopics.Items.Count > 0;
        }

        /// <summary>
        /// Validates the currently selected task by assigning today's date as the
        /// validation date, saving the change to the database, refreshing the UI,
        /// and triggering the copy suggestion for repeatable tasks.
        /// </summary>
        public void ValidateTask_Click(object? sender, EventArgs e)
        {
            // Retrieves the UI row corresponding to the selected task.
            var selectedRow = GetSelectedTaskRow();

            if (selectedRow == null || dbConn == null)
            {
                return;
            }

            // Reloads the full task object from the database.
            Tasks? selectedTask = dbConn.ReadTaskById(selectedRow.TaskId);

            if (selectedTask == null)
            {
                return;
            }

            string validationDate = DateTime.Today.ToString("yyyy-MM-dd");

            dbConn.ApproveTask(selectedTask.Id, validationDate);
            LoadTasks();

            // Repeatable tasks trigger the copy suggestion.
            if (selectedTask.Priorities_id >= 2)
            {
                AskForCopyingTask(selectedTask);
            }
        }
    }
}
