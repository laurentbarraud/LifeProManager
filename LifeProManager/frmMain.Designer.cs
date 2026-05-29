namespace LifeProManager
{
    partial class frmMain
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.ilsTabs = new System.Windows.Forms.ImageList(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ttpTotalTasksToComplete = new System.Windows.Forms.ToolTip(this.components);
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabDates = new System.Windows.Forms.TabPage();
            this.pnlWeek = new System.Windows.Forms.Panel();
            this.pnlToday = new System.Windows.Forms.Panel();
            this.lblWeek = new System.Windows.Forms.Label();
            this.lblToday = new System.Windows.Forms.Label();
            this.tabTopics = new System.Windows.Forms.TabPage();
            this.cmdNextTopic = new System.Windows.Forms.Button();
            this.cmdPreviousTopic = new System.Windows.Forms.Button();
            this.pnlTopics = new System.Windows.Forms.Panel();
            this.lblTopic = new System.Windows.Forms.Label();
            this.cmdDeleteTopic = new System.Windows.Forms.Button();
            this.tabFinished = new System.Windows.Forms.TabPage();
            this.cmdDeleteFinishedTasks = new System.Windows.Forms.Button();
            this.pnlFinished = new System.Windows.Forms.Panel();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.lnkAppInLanguage = new System.Windows.Forms.LinkLabel();
            this.lnkInsertTasksFromSql = new System.Windows.Forms.LinkLabel();
            this.nudTaskDescriptionFontSize = new System.Windows.Forms.NumericUpDown();
            this.lblTaskDescriptionFontSize = new System.Windows.Forms.Label();
            this.chkRunAtWindowsStartup = new System.Windows.Forms.CheckBox();
            this.lblExportDeadlineAndTitle = new System.Windows.Forms.Label();
            this.cboAppLanguage = new System.Windows.Forms.ComboBox();
            this.chkTopics = new System.Windows.Forms.CheckBox();
            this.chkDescriptions = new System.Windows.Forms.CheckBox();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.cmdAddTask = new System.Windows.Forms.Button();
            this.cmdSearch = new System.Windows.Forms.Button();
            this.cboTopics = new System.Windows.Forms.ComboBox();
            this.cmdAddTopic = new System.Windows.Forms.Button();
            this.cmdBirthdayCalendar = new System.Windows.Forms.Button();
            this.cmdExportToHtml = new System.Windows.Forms.Button();
            this.cmdNextDay = new System.Windows.Forms.Button();
            this.cmdToday = new System.Windows.Forms.Button();
            this.cmdPreviousDay = new System.Windows.Forms.Button();
            this.lblTaskDescription = new System.Windows.Forms.Label();
            this.calMonth = new System.Windows.Forms.MonthCalendar();
            this.cmsTasksOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ValidateTask = new System.Windows.Forms.ToolStripMenuItem();
            this.EditTask = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteTask = new System.Windows.Forms.ToolStripMenuItem();
            this.ReassignTask = new System.Windows.Forms.ToolStripMenuItem();
            this.lnkExportTasksToSql = new System.Windows.Forms.LinkLabel();
            this.pnlContainer.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabDates.SuspendLayout();
            this.tabTopics.SuspendLayout();
            this.tabFinished.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.pnlSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTaskDescriptionFontSize)).BeginInit();
            this.pnlRight.SuspendLayout();
            this.cmsTasksOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilsTabs
            // 
            this.ilsTabs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilsTabs.ImageStream")));
            this.ilsTabs.TransparentColor = System.Drawing.Color.Transparent;
            this.ilsTabs.Images.SetKeyName(0, "calendar.png");
            this.ilsTabs.Images.SetKeyName(1, "topic.png");
            this.ilsTabs.Images.SetKeyName(2, "validated-tasks.png");
            this.ilsTabs.Images.SetKeyName(3, "settings.png");
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.tabMain);
            this.pnlContainer.Controls.Add(this.pnlRight);
            resources.ApplyResources(this.pnlContainer, "pnlContainer");
            this.pnlContainer.Name = "pnlContainer";
            // 
            // tabMain
            // 
            resources.ApplyResources(this.tabMain, "tabMain");
            this.tabMain.Controls.Add(this.tabDates);
            this.tabMain.Controls.Add(this.tabTopics);
            this.tabMain.Controls.Add(this.tabFinished);
            this.tabMain.Controls.Add(this.tabSettings);
            this.tabMain.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabMain.HotTrack = true;
            this.tabMain.ImageList = this.ilsTabs;
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabMain_Selected);
            // 
            // tabDates
            // 
            this.tabDates.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.tabDates.Controls.Add(this.pnlWeek);
            this.tabDates.Controls.Add(this.pnlToday);
            this.tabDates.Controls.Add(this.lblWeek);
            this.tabDates.Controls.Add(this.lblToday);
            resources.ApplyResources(this.tabDates, "tabDates");
            this.tabDates.Name = "tabDates";
            // 
            // pnlWeek
            // 
            resources.ApplyResources(this.pnlWeek, "pnlWeek");
            this.pnlWeek.BackColor = System.Drawing.Color.White;
            this.pnlWeek.Name = "pnlWeek";
            // 
            // pnlToday
            // 
            resources.ApplyResources(this.pnlToday, "pnlToday");
            this.pnlToday.BackColor = System.Drawing.Color.White;
            this.pnlToday.Name = "pnlToday";
            // 
            // lblWeek
            // 
            resources.ApplyResources(this.lblWeek, "lblWeek");
            this.lblWeek.ForeColor = System.Drawing.Color.Black;
            this.lblWeek.Name = "lblWeek";
            // 
            // lblToday
            // 
            resources.ApplyResources(this.lblToday, "lblToday");
            this.lblToday.ForeColor = System.Drawing.Color.Black;
            this.lblToday.Name = "lblToday";
            // 
            // tabTopics
            // 
            this.tabTopics.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.tabTopics.Controls.Add(this.cmdNextTopic);
            this.tabTopics.Controls.Add(this.cmdPreviousTopic);
            this.tabTopics.Controls.Add(this.pnlTopics);
            this.tabTopics.Controls.Add(this.lblTopic);
            this.tabTopics.Controls.Add(this.cmdDeleteTopic);
            resources.ApplyResources(this.tabTopics, "tabTopics");
            this.tabTopics.Name = "tabTopics";
            this.tabTopics.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabTopics_Layout);
            // 
            // cmdNextTopic
            // 
            this.cmdNextTopic.BackColor = System.Drawing.Color.Transparent;
            this.cmdNextTopic.BackgroundImage = global::LifeProManager.Properties.Resources.right_chevron;
            resources.ApplyResources(this.cmdNextTopic, "cmdNextTopic");
            this.cmdNextTopic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdNextTopic.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.cmdNextTopic.FlatAppearance.BorderSize = 0;
            this.cmdNextTopic.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdNextTopic.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdNextTopic.Name = "cmdNextTopic";
            this.cmdNextTopic.UseVisualStyleBackColor = false;
            this.cmdNextTopic.Click += new System.EventHandler(this.cmdNextTopic_Click);
            // 
            // cmdPreviousTopic
            // 
            resources.ApplyResources(this.cmdPreviousTopic, "cmdPreviousTopic");
            this.cmdPreviousTopic.BackColor = System.Drawing.Color.Transparent;
            this.cmdPreviousTopic.BackgroundImage = global::LifeProManager.Properties.Resources.left_chevron;
            this.cmdPreviousTopic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdPreviousTopic.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.cmdPreviousTopic.FlatAppearance.BorderSize = 0;
            this.cmdPreviousTopic.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdPreviousTopic.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdPreviousTopic.Name = "cmdPreviousTopic";
            this.cmdPreviousTopic.UseVisualStyleBackColor = false;
            this.cmdPreviousTopic.Click += new System.EventHandler(this.cmdPreviousTopic_Click);
            // 
            // pnlTopics
            // 
            resources.ApplyResources(this.pnlTopics, "pnlTopics");
            this.pnlTopics.BackColor = System.Drawing.Color.White;
            this.pnlTopics.Name = "pnlTopics";
            this.pnlTopics.Resize += new System.EventHandler(this.pnlTopics_Resize);
            // 
            // lblTopic
            // 
            resources.ApplyResources(this.lblTopic, "lblTopic");
            this.lblTopic.ForeColor = System.Drawing.Color.Black;
            this.lblTopic.Name = "lblTopic";
            // 
            // cmdDeleteTopic
            // 
            resources.ApplyResources(this.cmdDeleteTopic, "cmdDeleteTopic");
            this.cmdDeleteTopic.BackColor = System.Drawing.Color.Transparent;
            this.cmdDeleteTopic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdDeleteTopic.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.cmdDeleteTopic.FlatAppearance.BorderSize = 0;
            this.cmdDeleteTopic.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdDeleteTopic.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdDeleteTopic.Name = "cmdDeleteTopic";
            this.cmdDeleteTopic.UseVisualStyleBackColor = false;
            this.cmdDeleteTopic.Click += new System.EventHandler(this.cmdDeleteTopic_Click);
            // 
            // tabFinished
            // 
            resources.ApplyResources(this.tabFinished, "tabFinished");
            this.tabFinished.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.tabFinished.Controls.Add(this.cmdDeleteFinishedTasks);
            this.tabFinished.Controls.Add(this.pnlFinished);
            this.tabFinished.Name = "tabFinished";
            // 
            // cmdDeleteFinishedTasks
            // 
            resources.ApplyResources(this.cmdDeleteFinishedTasks, "cmdDeleteFinishedTasks");
            this.cmdDeleteFinishedTasks.BackColor = System.Drawing.Color.Transparent;
            this.cmdDeleteFinishedTasks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdDeleteFinishedTasks.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.cmdDeleteFinishedTasks.FlatAppearance.BorderSize = 0;
            this.cmdDeleteFinishedTasks.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdDeleteFinishedTasks.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdDeleteFinishedTasks.Name = "cmdDeleteFinishedTasks";
            this.cmdDeleteFinishedTasks.UseVisualStyleBackColor = false;
            this.cmdDeleteFinishedTasks.Click += new System.EventHandler(this.cmdDeleteFinishedTasks_Click);
            // 
            // pnlFinished
            // 
            resources.ApplyResources(this.pnlFinished, "pnlFinished");
            this.pnlFinished.BackColor = System.Drawing.Color.White;
            this.pnlFinished.Name = "pnlFinished";
            // 
            // tabSettings
            // 
            this.tabSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.tabSettings.Controls.Add(this.pnlSettings);
            resources.ApplyResources(this.tabSettings, "tabSettings");
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Layout += new System.Windows.Forms.LayoutEventHandler(this.tabSettings_Layout);
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.lnkExportTasksToSql);
            this.pnlSettings.Controls.Add(this.lnkAppInLanguage);
            this.pnlSettings.Controls.Add(this.lnkInsertTasksFromSql);
            this.pnlSettings.Controls.Add(this.nudTaskDescriptionFontSize);
            this.pnlSettings.Controls.Add(this.lblTaskDescriptionFontSize);
            this.pnlSettings.Controls.Add(this.chkRunAtWindowsStartup);
            this.pnlSettings.Controls.Add(this.lblExportDeadlineAndTitle);
            this.pnlSettings.Controls.Add(this.cboAppLanguage);
            this.pnlSettings.Controls.Add(this.chkTopics);
            this.pnlSettings.Controls.Add(this.chkDescriptions);
            resources.ApplyResources(this.pnlSettings, "pnlSettings");
            this.pnlSettings.Name = "pnlSettings";
            // 
            // lnkAppInLanguage
            // 
            this.lnkAppInLanguage.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(83)))), ((int)(((byte)(116)))));
            resources.ApplyResources(this.lnkAppInLanguage, "lnkAppInLanguage");
            this.lnkAppInLanguage.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkAppInLanguage.LinkColor = System.Drawing.Color.Black;
            this.lnkAppInLanguage.Name = "lnkAppInLanguage";
            this.lnkAppInLanguage.TabStop = true;
            this.lnkAppInLanguage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAppInLanguage_LinkClicked);
            // 
            // lnkInsertTasksFromSql
            // 
            this.lnkInsertTasksFromSql.ActiveLinkColor = System.Drawing.Color.Purple;
            resources.ApplyResources(this.lnkInsertTasksFromSql, "lnkInsertTasksFromSql");
            this.lnkInsertTasksFromSql.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkInsertTasksFromSql.DisabledLinkColor = System.Drawing.Color.Black;
            this.lnkInsertTasksFromSql.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkInsertTasksFromSql.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(37)))), ((int)(((byte)(52)))));
            this.lnkInsertTasksFromSql.Name = "lnkInsertTasksFromSql";
            this.lnkInsertTasksFromSql.TabStop = true;
            this.lnkInsertTasksFromSql.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkInsertTasksFromSql_LinkClicked);
            // 
            // nudTaskDescriptionFontSize
            // 
            this.nudTaskDescriptionFontSize.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.nudTaskDescriptionFontSize, "nudTaskDescriptionFontSize");
            this.nudTaskDescriptionFontSize.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudTaskDescriptionFontSize.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudTaskDescriptionFontSize.Name = "nudTaskDescriptionFontSize";
            this.nudTaskDescriptionFontSize.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudTaskDescriptionFontSize.ValueChanged += new System.EventHandler(this.nudTaskDescriptionFontSize_ValueChanged);
            // 
            // lblTaskDescriptionFontSize
            // 
            resources.ApplyResources(this.lblTaskDescriptionFontSize, "lblTaskDescriptionFontSize");
            this.lblTaskDescriptionFontSize.ForeColor = System.Drawing.Color.Black;
            this.lblTaskDescriptionFontSize.Name = "lblTaskDescriptionFontSize";
            // 
            // chkRunAtWindowsStartup
            // 
            resources.ApplyResources(this.chkRunAtWindowsStartup, "chkRunAtWindowsStartup");
            this.chkRunAtWindowsStartup.ForeColor = System.Drawing.Color.Black;
            this.chkRunAtWindowsStartup.Name = "chkRunAtWindowsStartup";
            this.chkRunAtWindowsStartup.UseVisualStyleBackColor = true;
            this.chkRunAtWindowsStartup.CheckedChanged += new System.EventHandler(this.chkRunAtWindowsStartup_CheckedChanged);
            // 
            // lblExportDeadlineAndTitle
            // 
            resources.ApplyResources(this.lblExportDeadlineAndTitle, "lblExportDeadlineAndTitle");
            this.lblExportDeadlineAndTitle.ForeColor = System.Drawing.Color.Black;
            this.lblExportDeadlineAndTitle.Name = "lblExportDeadlineAndTitle";
            // 
            // cboAppLanguage
            // 
            resources.ApplyResources(this.cboAppLanguage, "cboAppLanguage");
            this.cboAppLanguage.FormattingEnabled = true;
            this.cboAppLanguage.Items.AddRange(new object[] {
            resources.GetString("cboAppLanguage.Items"),
            resources.GetString("cboAppLanguage.Items1")});
            this.cboAppLanguage.Name = "cboAppLanguage";
            this.cboAppLanguage.SelectedIndexChanged += new System.EventHandler(this.cboAppLanguage_SelectedIndexChanged);
            // 
            // chkTopics
            // 
            resources.ApplyResources(this.chkTopics, "chkTopics");
            this.chkTopics.ForeColor = System.Drawing.Color.Black;
            this.chkTopics.Name = "chkTopics";
            this.chkTopics.UseVisualStyleBackColor = true;
            this.chkTopics.CheckedChanged += new System.EventHandler(this.chkTopics_CheckedChanged);
            // 
            // chkDescriptions
            // 
            resources.ApplyResources(this.chkDescriptions, "chkDescriptions");
            this.chkDescriptions.ForeColor = System.Drawing.Color.Black;
            this.chkDescriptions.Name = "chkDescriptions";
            this.chkDescriptions.UseVisualStyleBackColor = true;
            this.chkDescriptions.CheckedChanged += new System.EventHandler(this.chkDescriptions_CheckedChanged);
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlRight.Controls.Add(this.cmdAddTask);
            this.pnlRight.Controls.Add(this.cmdSearch);
            this.pnlRight.Controls.Add(this.cboTopics);
            this.pnlRight.Controls.Add(this.cmdAddTopic);
            this.pnlRight.Controls.Add(this.cmdBirthdayCalendar);
            this.pnlRight.Controls.Add(this.cmdExportToHtml);
            this.pnlRight.Controls.Add(this.cmdNextDay);
            this.pnlRight.Controls.Add(this.cmdToday);
            this.pnlRight.Controls.Add(this.cmdPreviousDay);
            this.pnlRight.Controls.Add(this.lblTaskDescription);
            this.pnlRight.Controls.Add(this.calMonth);
            resources.ApplyResources(this.pnlRight, "pnlRight");
            this.pnlRight.Name = "pnlRight";
            // 
            // cmdAddTask
            // 
            resources.ApplyResources(this.cmdAddTask, "cmdAddTask");
            this.cmdAddTask.BackColor = System.Drawing.Color.Transparent;
            this.cmdAddTask.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdAddTask.FlatAppearance.BorderSize = 0;
            this.cmdAddTask.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdAddTask.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdAddTask.Name = "cmdAddTask";
            this.cmdAddTask.UseVisualStyleBackColor = false;
            this.cmdAddTask.Click += new System.EventHandler(this.cmdAddTask_Click);
            // 
            // cmdSearch
            // 
            resources.ApplyResources(this.cmdSearch, "cmdSearch");
            this.cmdSearch.BackColor = System.Drawing.Color.Transparent;
            this.cmdSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdSearch.FlatAppearance.BorderSize = 0;
            this.cmdSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.UseVisualStyleBackColor = false;
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearchByKeywords_Click);
            // 
            // cboTopics
            // 
            resources.ApplyResources(this.cboTopics, "cboTopics");
            this.cboTopics.FormattingEnabled = true;
            this.cboTopics.Name = "cboTopics";
            this.cboTopics.SelectedIndexChanged += new System.EventHandler(this.cboTopics_SelectedIndexChanged);
            // 
            // cmdAddTopic
            // 
            resources.ApplyResources(this.cmdAddTopic, "cmdAddTopic");
            this.cmdAddTopic.BackColor = System.Drawing.Color.Transparent;
            this.cmdAddTopic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdAddTopic.FlatAppearance.BorderSize = 0;
            this.cmdAddTopic.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdAddTopic.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdAddTopic.Name = "cmdAddTopic";
            this.cmdAddTopic.UseVisualStyleBackColor = false;
            this.cmdAddTopic.Click += new System.EventHandler(this.cmdAddTopic_Click);
            this.cmdAddTopic.MouseEnter += new System.EventHandler(this.cmdAddTopic_MouseEnter);
            this.cmdAddTopic.MouseLeave += new System.EventHandler(this.cmdAddTopic_MouseLeave);
            // 
            // cmdBirthdayCalendar
            // 
            resources.ApplyResources(this.cmdBirthdayCalendar, "cmdBirthdayCalendar");
            this.cmdBirthdayCalendar.BackColor = System.Drawing.Color.Transparent;
            this.cmdBirthdayCalendar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdBirthdayCalendar.FlatAppearance.BorderSize = 0;
            this.cmdBirthdayCalendar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdBirthdayCalendar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdBirthdayCalendar.Name = "cmdBirthdayCalendar";
            this.cmdBirthdayCalendar.UseVisualStyleBackColor = false;
            this.cmdBirthdayCalendar.Click += new System.EventHandler(this.cmdBirthdayCalendar_Click);
            this.cmdBirthdayCalendar.MouseEnter += new System.EventHandler(this.cmdBirthdayCalendar_MouseEnter);
            this.cmdBirthdayCalendar.MouseLeave += new System.EventHandler(this.cmdBirthdayCalendar_MouseLeave);
            // 
            // cmdExportToHtml
            // 
            resources.ApplyResources(this.cmdExportToHtml, "cmdExportToHtml");
            this.cmdExportToHtml.BackColor = System.Drawing.Color.Transparent;
            this.cmdExportToHtml.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdExportToHtml.FlatAppearance.BorderSize = 0;
            this.cmdExportToHtml.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdExportToHtml.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdExportToHtml.Name = "cmdExportToHtml";
            this.cmdExportToHtml.UseVisualStyleBackColor = false;
            this.cmdExportToHtml.Click += new System.EventHandler(this.cmdExportToHtml_Click);
            this.cmdExportToHtml.MouseEnter += new System.EventHandler(this.cmdExportToHtml_MouseEnter);
            this.cmdExportToHtml.MouseLeave += new System.EventHandler(this.cmdExportToHtml_MouseLeave);
            // 
            // cmdNextDay
            // 
            resources.ApplyResources(this.cmdNextDay, "cmdNextDay");
            this.cmdNextDay.BackColor = System.Drawing.Color.Transparent;
            this.cmdNextDay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdNextDay.FlatAppearance.BorderColor = System.Drawing.Color.LightSteelBlue;
            this.cmdNextDay.FlatAppearance.BorderSize = 0;
            this.cmdNextDay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdNextDay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdNextDay.Name = "cmdNextDay";
            this.cmdNextDay.UseVisualStyleBackColor = false;
            this.cmdNextDay.Click += new System.EventHandler(this.cmdNextDay_Click);
            this.cmdNextDay.MouseEnter += new System.EventHandler(this.cmdNextDay_MouseEnter);
            this.cmdNextDay.MouseLeave += new System.EventHandler(this.cmdNextDay_MouseLeave);
            // 
            // cmdToday
            // 
            resources.ApplyResources(this.cmdToday, "cmdToday");
            this.cmdToday.BackColor = System.Drawing.Color.Transparent;
            this.cmdToday.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdToday.FlatAppearance.BorderSize = 0;
            this.cmdToday.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdToday.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdToday.Name = "cmdToday";
            this.cmdToday.UseVisualStyleBackColor = false;
            this.cmdToday.Click += new System.EventHandler(this.cmdToday_Click);
            this.cmdToday.MouseEnter += new System.EventHandler(this.cmdToday_MouseEnter);
            this.cmdToday.MouseLeave += new System.EventHandler(this.cmdToday_MouseLeave);
            // 
            // cmdPreviousDay
            // 
            resources.ApplyResources(this.cmdPreviousDay, "cmdPreviousDay");
            this.cmdPreviousDay.BackColor = System.Drawing.Color.Transparent;
            this.cmdPreviousDay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdPreviousDay.FlatAppearance.BorderSize = 0;
            this.cmdPreviousDay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdPreviousDay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdPreviousDay.Name = "cmdPreviousDay";
            this.cmdPreviousDay.UseVisualStyleBackColor = false;
            this.cmdPreviousDay.Click += new System.EventHandler(this.cmdPreviousDay_Click);
            this.cmdPreviousDay.MouseEnter += new System.EventHandler(this.cmdPreviousDay_MouseEnter);
            this.cmdPreviousDay.MouseLeave += new System.EventHandler(this.cmdPreviousDay_MouseLeave);
            // 
            // lblTaskDescription
            // 
            this.lblTaskDescription.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lblTaskDescription, "lblTaskDescription");
            this.lblTaskDescription.ForeColor = System.Drawing.Color.Black;
            this.lblTaskDescription.Name = "lblTaskDescription";
            // 
            // calMonth
            // 
            this.calMonth.BackColor = System.Drawing.Color.Black;
            this.calMonth.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.calMonth, "calMonth");
            this.calMonth.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            this.calMonth.MaxSelectionCount = 1;
            this.calMonth.Name = "calMonth";
            this.calMonth.ShowToday = false;
            this.calMonth.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.calMonth_DateChanged);
            // 
            // cmsTasksOptions
            // 
            this.cmsTasksOptions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsTasksOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ValidateTask,
            this.EditTask,
            this.DeleteTask,
            this.ReassignTask});
            this.cmsTasksOptions.Name = "contextMenuStrip1";
            resources.ApplyResources(this.cmsTasksOptions, "cmsTasksOptions");
            // 
            // ValidateTask
            // 
            this.ValidateTask.Image = global::LifeProManager.Properties.Resources.validate_task;
            this.ValidateTask.Name = "ValidateTask";
            resources.ApplyResources(this.ValidateTask, "ValidateTask");
            // 
            // EditTask
            // 
            this.EditTask.Image = global::LifeProManager.Properties.Resources.edit_task;
            this.EditTask.Name = "EditTask";
            resources.ApplyResources(this.EditTask, "EditTask");
            // 
            // DeleteTask
            // 
            this.DeleteTask.Image = global::LifeProManager.Properties.Resources.delete_task;
            this.DeleteTask.Name = "DeleteTask";
            resources.ApplyResources(this.DeleteTask, "DeleteTask");
            // 
            // ReassignTask
            // 
            this.ReassignTask.Image = global::LifeProManager.Properties.Resources.unapprove_task;
            this.ReassignTask.Name = "ReassignTask";
            resources.ApplyResources(this.ReassignTask, "ReassignTask");
            // 
            // lnkExportTasksToSql
            // 
            this.lnkExportTasksToSql.ActiveLinkColor = System.Drawing.Color.Purple;
            resources.ApplyResources(this.lnkExportTasksToSql, "lnkExportTasksToSql");
            this.lnkExportTasksToSql.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkExportTasksToSql.DisabledLinkColor = System.Drawing.Color.Black;
            this.lnkExportTasksToSql.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkExportTasksToSql.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(37)))), ((int)(((byte)(52)))));
            this.lnkExportTasksToSql.Name = "lnkExportTasksToSql";
            this.lnkExportTasksToSql.TabStop = true;
            this.lnkExportTasksToSql.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkExportTasksToSql_LinkClicked);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlContainer);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.SizeChanged += new System.EventHandler(this.frmMain_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.pnlContainer.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabDates.ResumeLayout(false);
            this.tabDates.PerformLayout();
            this.tabTopics.ResumeLayout(false);
            this.tabTopics.PerformLayout();
            this.tabFinished.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTaskDescriptionFontSize)).EndInit();
            this.pnlRight.ResumeLayout(false);
            this.cmsTasksOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList ilsTabs;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolTip ttpTotalTasksToComplete;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabTopics;
        private System.Windows.Forms.Button cmdNextTopic;
        private System.Windows.Forms.Button cmdPreviousTopic;
        private System.Windows.Forms.Label lblTopic;
        private System.Windows.Forms.TabPage tabFinished;
        private System.Windows.Forms.Button cmdDeleteFinishedTasks;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabDates;
        private System.Windows.Forms.Label lblWeek;
        private System.Windows.Forms.Label lblToday;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Button cmdBirthdayCalendar;
        private System.Windows.Forms.Label lblTaskDescription;
        private System.Windows.Forms.Button cmdNextDay;
        private System.Windows.Forms.Button cmdPreviousDay;
        public System.Windows.Forms.ComboBox cboTopics;
        private System.Windows.Forms.Button cmdAddTopic;
        private System.Windows.Forms.Button cmdExportToHtml;
        private System.Windows.Forms.Button cmdToday;
        private System.Windows.Forms.MonthCalendar calMonth;
        private System.Windows.Forms.Button cmdDeleteTopic;
        private System.Windows.Forms.Button cmdSearch;
        internal System.Windows.Forms.Panel pnlTopics;
        internal System.Windows.Forms.Panel pnlFinished;
        internal System.Windows.Forms.Panel pnlWeek;
        internal System.Windows.Forms.Panel pnlToday;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.LinkLabel lnkAppInLanguage;
        private System.Windows.Forms.LinkLabel lnkInsertTasksFromSql;
        private System.Windows.Forms.NumericUpDown nudTaskDescriptionFontSize;
        private System.Windows.Forms.Label lblTaskDescriptionFontSize;
        private System.Windows.Forms.CheckBox chkRunAtWindowsStartup;
        private System.Windows.Forms.Label lblExportDeadlineAndTitle;
        private System.Windows.Forms.ComboBox cboAppLanguage;
        private System.Windows.Forms.CheckBox chkTopics;
        private System.Windows.Forms.CheckBox chkDescriptions;
        private System.Windows.Forms.Button cmdAddTask;
        internal System.Windows.Forms.ToolStripMenuItem ValidateTask;
        internal System.Windows.Forms.ToolStripMenuItem EditTask;
        internal System.Windows.Forms.ToolStripMenuItem DeleteTask;
        internal System.Windows.Forms.ToolStripMenuItem ReassignTask;
        internal System.Windows.Forms.ContextMenuStrip cmsTasksOptions;
        private System.Windows.Forms.LinkLabel lnkExportTasksToSql;
    }
}

