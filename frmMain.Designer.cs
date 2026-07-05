namespace LifeProManager
{
    partial class frmMain
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            ilsTabs = new ImageList(components);
            saveFileDialog1 = new SaveFileDialog();
            ttpTotalTasksToComplete = new ToolTip(components);
            pnlContainer = new Panel();
            tabMain = new TabControl();
            tabDates = new TabPage();
            pnlWeek = new Panel();
            pnlToday = new Panel();
            lblWeek = new Label();
            lblToday = new Label();
            tabTopics = new TabPage();
            pnlTopics = new Panel();
            cmdNextTopic = new Button();
            cmdPreviousTopic = new Button();
            lblTopic = new Label();
            cmdDeleteTopic = new Button();
            tabFinished = new TabPage();
            cmdDeleteFinishedTasks = new Button();
            pnlFinished = new Panel();
            tabSettings = new TabPage();
            lnkChangeAppColors = new LinkLabel();
            lnkExportTasksToSql = new LinkLabel();
            lnkAppInLanguage = new LinkLabel();
            lnkInsertTasksFromSql = new LinkLabel();
            nudTaskDescriptionFontSize = new NumericUpDown();
            lblTaskDescriptionFontSize = new Label();
            chkRunAtWindowsStartup = new CheckBox();
            lblExportDeadlineAndTitle = new Label();
            cboAppLanguage = new ComboBox();
            chkTopics = new CheckBox();
            chkDescriptions = new CheckBox();
            pnlRight = new Panel();
            cmdAddTask = new Button();
            cmdSearch = new Button();
            cboTopics = new ComboBox();
            cmdAddTopic = new Button();
            cmdBirthdayCalendar = new Button();
            cmdExportToHtml = new Button();
            cmdNextDay = new Button();
            cmdToday = new Button();
            cmdPreviousDay = new Button();
            lblTaskDescription = new Label();
            calMonth = new MonthCalendar();
            cmsTasksOptions = new ContextMenuStrip(components);
            ValidateTask = new ToolStripMenuItem();
            EditTask = new ToolStripMenuItem();
            DeleteTask = new ToolStripMenuItem();
            ReassignTask = new ToolStripMenuItem();
            pnlContainer.SuspendLayout();
            tabMain.SuspendLayout();
            tabDates.SuspendLayout();
            tabTopics.SuspendLayout();
            tabFinished.SuspendLayout();
            tabSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudTaskDescriptionFontSize).BeginInit();
            pnlRight.SuspendLayout();
            cmsTasksOptions.SuspendLayout();
            SuspendLayout();
            // 
            // ilsTabs
            // 
            ilsTabs.ColorDepth = ColorDepth.Depth32Bit;
            ilsTabs.ImageStream = (ImageListStreamer)resources.GetObject("ilsTabs.ImageStream");
            ilsTabs.TransparentColor = Color.Transparent;
            ilsTabs.Images.SetKeyName(0, "calendar.png");
            ilsTabs.Images.SetKeyName(1, "topic.png");
            ilsTabs.Images.SetKeyName(2, "validated-tasks.png");
            ilsTabs.Images.SetKeyName(3, "settings.png");
            // 
            // pnlContainer
            // 
            pnlContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlContainer.Controls.Add(tabMain);
            pnlContainer.Controls.Add(pnlRight);
            pnlContainer.Location = new Point(13, 12);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(961, 670);
            pnlContainer.TabIndex = 1;
            // 
            // tabMain
            // 
            tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabMain.Controls.Add(tabDates);
            tabMain.Controls.Add(tabTopics);
            tabMain.Controls.Add(tabFinished);
            tabMain.Controls.Add(tabSettings);
            tabMain.Font = new Font("Segoe UI", 12F);
            tabMain.HotTrack = true;
            tabMain.ImageList = ilsTabs;
            tabMain.ItemSize = new Size(81, 30);
            tabMain.Location = new Point(0, 0);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(684, 658);
            tabMain.TabIndex = 0;
            tabMain.SelectedIndexChanged += tabMain_Selected;
            // 
            // tabDates
            // 
            tabDates.BackColor = Color.FromArgb(245, 247, 250);
            tabDates.Controls.Add(pnlWeek);
            tabDates.Controls.Add(pnlToday);
            tabDates.Controls.Add(lblWeek);
            tabDates.Controls.Add(lblToday);
            tabDates.ImageKey = "calendar.png";
            tabDates.Location = new Point(4, 34);
            tabDates.Name = "tabDates";
            tabDates.Padding = new Padding(4);
            tabDates.Size = new Size(676, 620);
            tabDates.TabIndex = 0;
            tabDates.Text = "Dates";
            // 
            // pnlWeek
            // 
            pnlWeek.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlWeek.AutoScroll = true;
            pnlWeek.BackColor = Color.White;
            pnlWeek.Location = new Point(23, 377);
            pnlWeek.Name = "pnlWeek";
            pnlWeek.Size = new Size(631, 225);
            pnlWeek.TabIndex = 4;
            // 
            // pnlToday
            // 
            pnlToday.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlToday.AutoScroll = true;
            pnlToday.BackColor = Color.White;
            pnlToday.Location = new Point(23, 80);
            pnlToday.Name = "pnlToday";
            pnlToday.Size = new Size(631, 234);
            pnlToday.TabIndex = 0;
            // 
            // lblWeek
            // 
            lblWeek.AutoSize = true;
            lblWeek.Font = new Font("Segoe UI Light", 10.8F);
            lblWeek.ForeColor = Color.Black;
            lblWeek.Location = new Point(65, 336);
            lblWeek.Name = "lblWeek";
            lblWeek.Size = new Size(128, 25);
            lblWeek.TabIndex = 2;
            lblWeek.Text = "Prochains jours";
            // 
            // lblToday
            // 
            lblToday.AutoSize = true;
            lblToday.Font = new Font("Segoe UI Light", 10.8F);
            lblToday.ForeColor = Color.Black;
            lblToday.Location = new Point(64, 40);
            lblToday.Name = "lblToday";
            lblToday.Size = new Size(101, 25);
            lblToday.TabIndex = 3;
            lblToday.Text = "Aujourd'hui";
            // 
            // tabTopics
            // 
            tabTopics.BackColor = Color.FromArgb(245, 247, 250);
            tabTopics.Controls.Add(pnlTopics);
            tabTopics.Controls.Add(cmdNextTopic);
            tabTopics.Controls.Add(cmdPreviousTopic);
            tabTopics.Controls.Add(lblTopic);
            tabTopics.Controls.Add(cmdDeleteTopic);
            tabTopics.ImageKey = "topic.png";
            tabTopics.Location = new Point(4, 34);
            tabTopics.Name = "tabTopics";
            tabTopics.Padding = new Padding(4);
            tabTopics.Size = new Size(676, 620);
            tabTopics.TabIndex = 1;
            tabTopics.Text = "Thèmes";
            tabTopics.Layout += tabTopics_Layout;
            // 
            // pnlTopics
            // 
            pnlTopics.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlTopics.AutoScroll = true;
            pnlTopics.BackColor = Color.White;
            pnlTopics.Location = new Point(27, 88);
            pnlTopics.Name = "pnlTopics";
            pnlTopics.Size = new Size(1090, 516);
            pnlTopics.TabIndex = 0;
            // 
            // cmdNextTopic
            // 
            cmdNextTopic.BackgroundImage = Properties.Resources.rightChevron;
            cmdNextTopic.BackgroundImageLayout = ImageLayout.Zoom;
            cmdNextTopic.Cursor = Cursors.Hand;
            cmdNextTopic.FlatAppearance.BorderSize = 0;
            cmdNextTopic.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdNextTopic.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdNextTopic.FlatStyle = FlatStyle.Flat;
            cmdNextTopic.Location = new Point(519, 36);
            cmdNextTopic.Name = "cmdNextTopic";
            cmdNextTopic.Size = new Size(28, 26);
            cmdNextTopic.TabIndex = 1;
            cmdNextTopic.Click += cmdNextTopic_Click;
            // 
            // cmdPreviousTopic
            // 
            cmdPreviousTopic.BackgroundImage = Properties.Resources.leftChevron;
            cmdPreviousTopic.BackgroundImageLayout = ImageLayout.Zoom;
            cmdPreviousTopic.Cursor = Cursors.Hand;
            cmdPreviousTopic.FlatAppearance.BorderSize = 0;
            cmdPreviousTopic.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdPreviousTopic.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdPreviousTopic.FlatStyle = FlatStyle.Flat;
            cmdPreviousTopic.Location = new Point(158, 36);
            cmdPreviousTopic.Name = "cmdPreviousTopic";
            cmdPreviousTopic.Size = new Size(28, 26);
            cmdPreviousTopic.TabIndex = 2;
            cmdPreviousTopic.Click += cmdPreviousTopic_Click;
            // 
            // lblTopic
            // 
            lblTopic.AutoSize = true;
            lblTopic.Font = new Font("Segoe UI Light", 12F);
            lblTopic.ForeColor = Color.Black;
            lblTopic.Location = new Point(322, 36);
            lblTopic.Name = "lblTopic";
            lblTopic.Size = new Size(69, 28);
            lblTopic.TabIndex = 3;
            lblTopic.Text = "Thème";
            lblTopic.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmdDeleteTopic
            // 
            cmdDeleteTopic.BackColor = Color.Transparent;
            cmdDeleteTopic.BackgroundImage = Properties.Resources.deleteTrash;
            cmdDeleteTopic.BackgroundImageLayout = ImageLayout.Zoom;
            cmdDeleteTopic.Cursor = Cursors.Hand;
            cmdDeleteTopic.FlatAppearance.BorderSize = 0;
            cmdDeleteTopic.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdDeleteTopic.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdDeleteTopic.FlatStyle = FlatStyle.Flat;
            cmdDeleteTopic.Location = new Point(595, 34);
            cmdDeleteTopic.Name = "cmdDeleteTopic";
            cmdDeleteTopic.Size = new Size(28, 30);
            cmdDeleteTopic.TabIndex = 4;
            cmdDeleteTopic.UseVisualStyleBackColor = false;
            cmdDeleteTopic.Click += cmdDeleteTopic_Click;
            // 
            // tabFinished
            // 
            tabFinished.BackColor = Color.FromArgb(245, 247, 250);
            tabFinished.Controls.Add(cmdDeleteFinishedTasks);
            tabFinished.Controls.Add(pnlFinished);
            tabFinished.ImageKey = "validated-tasks.png";
            tabFinished.Location = new Point(4, 34);
            tabFinished.Name = "tabFinished";
            tabFinished.Padding = new Padding(4);
            tabFinished.Size = new Size(676, 620);
            tabFinished.TabIndex = 2;
            tabFinished.Text = "Terminées";
            // 
            // cmdDeleteFinishedTasks
            // 
            cmdDeleteFinishedTasks.BackColor = Color.Transparent;
            cmdDeleteFinishedTasks.BackgroundImage = Properties.Resources.deleteTrash;
            cmdDeleteFinishedTasks.BackgroundImageLayout = ImageLayout.Zoom;
            cmdDeleteFinishedTasks.Cursor = Cursors.Hand;
            cmdDeleteFinishedTasks.FlatAppearance.BorderSize = 0;
            cmdDeleteFinishedTasks.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdDeleteFinishedTasks.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdDeleteFinishedTasks.FlatStyle = FlatStyle.Flat;
            cmdDeleteFinishedTasks.Location = new Point(595, 34);
            cmdDeleteFinishedTasks.Name = "cmdDeleteFinishedTasks";
            cmdDeleteFinishedTasks.Size = new Size(28, 30);
            cmdDeleteFinishedTasks.TabIndex = 0;
            cmdDeleteFinishedTasks.UseVisualStyleBackColor = false;
            cmdDeleteFinishedTasks.Visible = false;
            cmdDeleteFinishedTasks.Click += cmdDeleteFinishedTasks_Click;
            // 
            // pnlFinished
            // 
            pnlFinished.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlFinished.AutoScroll = true;
            pnlFinished.BackColor = Color.White;
            pnlFinished.Location = new Point(27, 88);
            pnlFinished.Name = "pnlFinished";
            pnlFinished.Size = new Size(1090, 516);
            pnlFinished.TabIndex = 1;
            // 
            // tabSettings
            // 
            tabSettings.BackColor = Color.FromArgb(248, 250, 252);
            tabSettings.Controls.Add(lnkChangeAppColors);
            tabSettings.Controls.Add(lnkExportTasksToSql);
            tabSettings.Controls.Add(lnkAppInLanguage);
            tabSettings.Controls.Add(lnkInsertTasksFromSql);
            tabSettings.Controls.Add(nudTaskDescriptionFontSize);
            tabSettings.Controls.Add(lblTaskDescriptionFontSize);
            tabSettings.Controls.Add(chkRunAtWindowsStartup);
            tabSettings.Controls.Add(lblExportDeadlineAndTitle);
            tabSettings.Controls.Add(cboAppLanguage);
            tabSettings.Controls.Add(chkTopics);
            tabSettings.Controls.Add(chkDescriptions);
            tabSettings.ImageKey = "settings.png";
            tabSettings.Location = new Point(4, 34);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new Padding(4);
            tabSettings.Size = new Size(676, 620);
            tabSettings.TabIndex = 3;
            tabSettings.Text = "Paramètres";
            tabSettings.Layout += tabSettings_Layout;
            // 
            // lnkChangeAppColors
            // 
            lnkChangeAppColors.ActiveLinkColor = Color.Purple;
            lnkChangeAppColors.AutoSize = true;
            lnkChangeAppColors.Cursor = Cursors.Hand;
            lnkChangeAppColors.Font = new Font("Segoe UI", 10.2F);
            lnkChangeAppColors.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkChangeAppColors.LinkColor = Color.FromArgb(22, 37, 52);
            lnkChangeAppColors.Location = new Point(134, 357);
            lnkChangeAppColors.Name = "lnkChangeAppColors";
            lnkChangeAppColors.Size = new Size(325, 23);
            lnkChangeAppColors.TabIndex = 11;
            lnkChangeAppColors.TabStop = true;
            lnkChangeAppColors.Text = "Personnaliser les couleurs de l’application";
            lnkChangeAppColors.Click += lnkChangeAppColors_LinkClicked;
            // 
            // lnkExportTasksToSql
            // 
            lnkExportTasksToSql.ActiveLinkColor = Color.Purple;
            lnkExportTasksToSql.AutoSize = true;
            lnkExportTasksToSql.Cursor = Cursors.Hand;
            lnkExportTasksToSql.Font = new Font("Segoe UI", 10.2F);
            lnkExportTasksToSql.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkExportTasksToSql.LinkColor = Color.FromArgb(22, 37, 52);
            lnkExportTasksToSql.Location = new Point(134, 461);
            lnkExportTasksToSql.Name = "lnkExportTasksToSql";
            lnkExportTasksToSql.Size = new Size(404, 23);
            lnkExportTasksToSql.TabIndex = 12;
            lnkExportTasksToSql.TabStop = true;
            lnkExportTasksToSql.Text = "Exporter toutes les tâches dans un fichier script SQL";
            lnkExportTasksToSql.Click += lnkExportTasksToSql_LinkClicked;
            // 
            // lnkAppInLanguage
            // 
            lnkAppInLanguage.ActiveLinkColor = Color.FromArgb(50, 83, 116);
            lnkAppInLanguage.AutoSize = true;
            lnkAppInLanguage.Font = new Font("Segoe UI", 10.2F);
            lnkAppInLanguage.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkAppInLanguage.LinkColor = Color.Black;
            lnkAppInLanguage.Location = new Point(140, 517);
            lnkAppInLanguage.Name = "lnkAppInLanguage";
            lnkAppInLanguage.Size = new Size(129, 23);
            lnkAppInLanguage.TabIndex = 13;
            lnkAppInLanguage.TabStop = true;
            lnkAppInLanguage.Text = "Application en :";
            lnkAppInLanguage.Click += lnkAppInLanguage_LinkClicked;
            // 
            // lnkInsertTasksFromSql
            // 
            lnkInsertTasksFromSql.ActiveLinkColor = Color.Purple;
            lnkInsertTasksFromSql.AutoSize = true;
            lnkInsertTasksFromSql.Cursor = Cursors.Hand;
            lnkInsertTasksFromSql.Font = new Font("Segoe UI", 10.2F);
            lnkInsertTasksFromSql.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkInsertTasksFromSql.LinkColor = Color.FromArgb(22, 37, 52);
            lnkInsertTasksFromSql.Location = new Point(134, 426);
            lnkInsertTasksFromSql.Name = "lnkInsertTasksFromSql";
            lnkInsertTasksFromSql.Size = new Size(254, 23);
            lnkInsertTasksFromSql.TabIndex = 14;
            lnkInsertTasksFromSql.TabStop = true;
            lnkInsertTasksFromSql.Text = "Insérer des tâches via script SQL";
            lnkInsertTasksFromSql.Click += lnkInsertTasksFromSql_LinkClicked;
            // 
            // nudTaskDescriptionFontSize
            // 
            nudTaskDescriptionFontSize.Font = new Font("Segoe UI", 10.2F);
            nudTaskDescriptionFontSize.Location = new Point(138, 248);
            nudTaskDescriptionFontSize.Maximum = new decimal(new int[] { 32, 0, 0, 0 });
            nudTaskDescriptionFontSize.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            nudTaskDescriptionFontSize.Name = "nudTaskDescriptionFontSize";
            nudTaskDescriptionFontSize.Size = new Size(46, 30);
            nudTaskDescriptionFontSize.TabIndex = 15;
            nudTaskDescriptionFontSize.Value = new decimal(new int[] { 8, 0, 0, 0 });
            nudTaskDescriptionFontSize.ValueChanged += nudTaskDescriptionFontSize_ValueChanged;
            // 
            // lblTaskDescriptionFontSize
            // 
            lblTaskDescriptionFontSize.AutoSize = true;
            lblTaskDescriptionFontSize.Font = new Font("Segoe UI", 10.2F);
            lblTaskDescriptionFontSize.ForeColor = Color.Black;
            lblTaskDescriptionFontSize.Location = new Point(134, 208);
            lblTaskDescriptionFontSize.Name = "lblTaskDescriptionFontSize";
            lblTaskDescriptionFontSize.Size = new Size(251, 23);
            lblTaskDescriptionFontSize.TabIndex = 16;
            lblTaskDescriptionFontSize.Text = "Taille du texte des descriptions :";
            // 
            // chkRunAtWindowsStartup
            // 
            chkRunAtWindowsStartup.AutoSize = true;
            chkRunAtWindowsStartup.Font = new Font("Segoe UI", 10.2F);
            chkRunAtWindowsStartup.ForeColor = Color.Black;
            chkRunAtWindowsStartup.Location = new Point(134, 304);
            chkRunAtWindowsStartup.Name = "chkRunAtWindowsStartup";
            chkRunAtWindowsStartup.Size = new Size(292, 27);
            chkRunAtWindowsStartup.TabIndex = 17;
            chkRunAtWindowsStartup.Text = "Lancer au démarrage de Windows";
            chkRunAtWindowsStartup.CheckedChanged += chkRunAtWindowsStartup_CheckedChanged;
            // 
            // lblExportDeadlineAndTitle
            // 
            lblExportDeadlineAndTitle.AutoSize = true;
            lblExportDeadlineAndTitle.Font = new Font("Segoe UI", 10.2F);
            lblExportDeadlineAndTitle.ForeColor = Color.Black;
            lblExportDeadlineAndTitle.Location = new Point(134, 73);
            lblExportDeadlineAndTitle.Name = "lblExportDeadlineAndTitle";
            lblExportDeadlineAndTitle.Size = new Size(325, 23);
            lblExportDeadlineAndTitle.TabIndex = 18;
            lblExportDeadlineAndTitle.Text = "Exporter les tâches avec échéance et titre";
            // 
            // cboAppLanguage
            // 
            cboAppLanguage.Font = new Font("Segoe UI Semilight", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cboAppLanguage.Items.AddRange(new object[] { "Français", "English" });
            cboAppLanguage.Location = new Point(143, 550);
            cboAppLanguage.Name = "cboAppLanguage";
            cboAppLanguage.Size = new Size(121, 33);
            cboAppLanguage.TabIndex = 19;
            cboAppLanguage.SelectedIndexChanged += cboAppLanguage_SelectedIndexChanged;
            // 
            // chkTopics
            // 
            chkTopics.AutoSize = true;
            chkTopics.Font = new Font("Segoe UI", 10.2F);
            chkTopics.ForeColor = Color.Black;
            chkTopics.Location = new Point(134, 144);
            chkTopics.Name = "chkTopics";
            chkTopics.Size = new Size(176, 27);
            chkTopics.TabIndex = 20;
            chkTopics.Text = "Afficher les thèmes";
            chkTopics.CheckedChanged += chkTopics_CheckedChanged;
            // 
            // chkDescriptions
            // 
            chkDescriptions.AutoSize = true;
            chkDescriptions.Font = new Font("Segoe UI", 10.2F);
            chkDescriptions.ForeColor = Color.Black;
            chkDescriptions.Location = new Point(134, 111);
            chkDescriptions.Name = "chkDescriptions";
            chkDescriptions.Size = new Size(211, 27);
            chkDescriptions.TabIndex = 21;
            chkDescriptions.Text = "Afficher les descriptions";
            chkDescriptions.CheckedChanged += chkDescriptions_CheckedChanged;
            // 
            // pnlRight
            // 
            pnlRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pnlRight.BackColor = Color.FromArgb(245, 247, 250);
            pnlRight.Controls.Add(cmdAddTask);
            pnlRight.Controls.Add(cmdSearch);
            pnlRight.Controls.Add(cboTopics);
            pnlRight.Controls.Add(cmdAddTopic);
            pnlRight.Controls.Add(cmdBirthdayCalendar);
            pnlRight.Controls.Add(cmdExportToHtml);
            pnlRight.Controls.Add(cmdNextDay);
            pnlRight.Controls.Add(cmdToday);
            pnlRight.Controls.Add(cmdPreviousDay);
            pnlRight.Controls.Add(lblTaskDescription);
            pnlRight.Controls.Add(calMonth);
            pnlRight.Location = new Point(690, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(271, 658);
            pnlRight.TabIndex = 1;
            // 
            // cmdAddTask
            // 
            cmdAddTask.BackColor = Color.Transparent;
            cmdAddTask.BackgroundImage = Properties.Resources.addTask;
            cmdAddTask.BackgroundImageLayout = ImageLayout.Zoom;
            cmdAddTask.Cursor = Cursors.Hand;
            cmdAddTask.FlatAppearance.BorderSize = 0;
            cmdAddTask.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdAddTask.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdAddTask.FlatStyle = FlatStyle.Flat;
            cmdAddTask.Location = new Point(118, 598);
            cmdAddTask.Name = "cmdAddTask";
            cmdAddTask.Size = new Size(49, 38);
            cmdAddTask.TabIndex = 0;
            cmdAddTask.UseVisualStyleBackColor = false;
            cmdAddTask.Click += cmdAddTask_Click;
            // 
            // cmdSearch
            // 
            cmdSearch.BackColor = Color.Transparent;
            cmdSearch.BackgroundImage = Properties.Resources.search;
            cmdSearch.BackgroundImageLayout = ImageLayout.Zoom;
            cmdSearch.Cursor = Cursors.Hand;
            cmdSearch.FlatAppearance.BorderSize = 0;
            cmdSearch.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdSearch.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdSearch.FlatStyle = FlatStyle.Flat;
            cmdSearch.Location = new Point(195, 473);
            cmdSearch.Name = "cmdSearch";
            cmdSearch.Size = new Size(34, 34);
            cmdSearch.TabIndex = 1;
            cmdSearch.UseVisualStyleBackColor = false;
            cmdSearch.Click += cmdSearchByKeywords_Click;
            // 
            // cboTopics
            // 
            cboTopics.Font = new Font("Segoe UI Semilight", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cboTopics.Location = new Point(61, 546);
            cboTopics.Name = "cboTopics";
            cboTopics.Size = new Size(178, 28);
            cboTopics.TabIndex = 2;
            cboTopics.SelectedIndexChanged += cboTopics_SelectedIndexChanged;
            // 
            // cmdAddTopic
            // 
            cmdAddTopic.BackColor = Color.Transparent;
            cmdAddTopic.BackgroundImage = Properties.Resources.addTopic;
            cmdAddTopic.BackgroundImageLayout = ImageLayout.Zoom;
            cmdAddTopic.Cursor = Cursors.Hand;
            cmdAddTopic.FlatAppearance.BorderSize = 0;
            cmdAddTopic.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdAddTopic.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdAddTopic.FlatStyle = FlatStyle.Flat;
            cmdAddTopic.Location = new Point(24, 546);
            cmdAddTopic.Name = "cmdAddTopic";
            cmdAddTopic.Size = new Size(28, 28);
            cmdAddTopic.TabIndex = 3;
            cmdAddTopic.UseVisualStyleBackColor = false;
            cmdAddTopic.Click += cmdAddTopic_Click;
            cmdAddTopic.MouseEnter += cmdAddTopic_MouseEnter;
            cmdAddTopic.MouseLeave += cmdAddTopic_MouseLeave;
            // 
            // cmdBirthdayCalendar
            // 
            cmdBirthdayCalendar.BackColor = Color.Transparent;
            cmdBirthdayCalendar.BackgroundImage = Properties.Resources.birthdayCake;
            cmdBirthdayCalendar.BackgroundImageLayout = ImageLayout.Zoom;
            cmdBirthdayCalendar.Cursor = Cursors.Hand;
            cmdBirthdayCalendar.FlatAppearance.BorderSize = 0;
            cmdBirthdayCalendar.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdBirthdayCalendar.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdBirthdayCalendar.FlatStyle = FlatStyle.Flat;
            cmdBirthdayCalendar.Location = new Point(124, 468);
            cmdBirthdayCalendar.Name = "cmdBirthdayCalendar";
            cmdBirthdayCalendar.Size = new Size(38, 38);
            cmdBirthdayCalendar.TabIndex = 4;
            cmdBirthdayCalendar.UseVisualStyleBackColor = false;
            cmdBirthdayCalendar.Click += cmdBirthdayCalendar_Click;
            cmdBirthdayCalendar.MouseEnter += cmdBirthdayCalendar_MouseEnter;
            cmdBirthdayCalendar.MouseLeave += cmdBirthdayCalendar_MouseLeave;
            // 
            // cmdExportToHtml
            // 
            cmdExportToHtml.BackColor = Color.Transparent;
            cmdExportToHtml.BackgroundImage = Properties.Resources.exportToHtml;
            cmdExportToHtml.BackgroundImageLayout = ImageLayout.Zoom;
            cmdExportToHtml.Cursor = Cursors.Hand;
            cmdExportToHtml.FlatAppearance.BorderSize = 0;
            cmdExportToHtml.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdExportToHtml.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdExportToHtml.FlatStyle = FlatStyle.Flat;
            cmdExportToHtml.Location = new Point(61, 469);
            cmdExportToHtml.Name = "cmdExportToHtml";
            cmdExportToHtml.Size = new Size(38, 38);
            cmdExportToHtml.TabIndex = 5;
            cmdExportToHtml.UseVisualStyleBackColor = false;
            cmdExportToHtml.Click += cmdExportToHtml_Click;
            cmdExportToHtml.MouseEnter += cmdExportToHtml_MouseEnter;
            cmdExportToHtml.MouseLeave += cmdExportToHtml_MouseLeave;
            // 
            // cmdNextDay
            // 
            cmdNextDay.BackColor = Color.Transparent;
            cmdNextDay.BackgroundImage = Properties.Resources.rightChevron;
            cmdNextDay.BackgroundImageLayout = ImageLayout.Zoom;
            cmdNextDay.Cursor = Cursors.Hand;
            cmdNextDay.FlatAppearance.BorderSize = 0;
            cmdNextDay.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdNextDay.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdNextDay.FlatStyle = FlatStyle.Flat;
            cmdNextDay.Location = new Point(196, 400);
            cmdNextDay.Name = "cmdNextDay";
            cmdNextDay.Size = new Size(28, 26);
            cmdNextDay.TabIndex = 6;
            cmdNextDay.UseVisualStyleBackColor = false;
            cmdNextDay.Click += cmdNextDay_Click;
            cmdNextDay.MouseEnter += cmdNextDay_MouseEnter;
            cmdNextDay.MouseLeave += cmdNextDay_MouseLeave;
            // 
            // cmdToday
            // 
            cmdToday.BackColor = Color.Transparent;
            cmdToday.BackgroundImage = Properties.Resources.calendarToday;
            cmdToday.BackgroundImageLayout = ImageLayout.Zoom;
            cmdToday.Cursor = Cursors.Hand;
            cmdToday.FlatAppearance.BorderSize = 0;
            cmdToday.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdToday.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdToday.FlatStyle = FlatStyle.Flat;
            cmdToday.Location = new Point(128, 399);
            cmdToday.Name = "cmdToday";
            cmdToday.Size = new Size(30, 30);
            cmdToday.TabIndex = 7;
            cmdToday.UseVisualStyleBackColor = false;
            cmdToday.Click += cmdToday_Click;
            cmdToday.MouseEnter += cmdToday_MouseEnter;
            cmdToday.MouseLeave += cmdToday_MouseLeave;
            // 
            // cmdPreviousDay
            // 
            cmdPreviousDay.BackColor = Color.Transparent;
            cmdPreviousDay.BackgroundImage = Properties.Resources.leftChevron;
            cmdPreviousDay.BackgroundImageLayout = ImageLayout.Zoom;
            cmdPreviousDay.Cursor = Cursors.Hand;
            cmdPreviousDay.FlatAppearance.BorderSize = 0;
            cmdPreviousDay.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdPreviousDay.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdPreviousDay.FlatStyle = FlatStyle.Flat;
            cmdPreviousDay.Location = new Point(61, 400);
            cmdPreviousDay.Name = "cmdPreviousDay";
            cmdPreviousDay.Size = new Size(28, 26);
            cmdPreviousDay.TabIndex = 8;
            cmdPreviousDay.UseVisualStyleBackColor = false;
            cmdPreviousDay.Click += cmdPreviousDay_Click;
            cmdPreviousDay.MouseEnter += cmdPreviousDay_MouseEnter;
            cmdPreviousDay.MouseLeave += cmdPreviousDay_MouseLeave;
            // 
            // lblTaskDescription
            // 
            lblTaskDescription.BackColor = Color.FromArgb(243, 239, 218);
            lblTaskDescription.Font = new Font("Segoe UI", 10.2F);
            lblTaskDescription.ForeColor = Color.Black;
            lblTaskDescription.Location = new Point(28, 8);
            lblTaskDescription.Name = "lblTaskDescription";
            lblTaskDescription.Size = new Size(246, 151);
            lblTaskDescription.TabIndex = 9;
            lblTaskDescription.Visible = false;
            // 
            // calMonth
            // 
            calMonth.Location = new Point(32, 180);
            calMonth.MaxDate = new DateTime(2100, 12, 31, 0, 0, 0, 0);
            calMonth.MaxSelectionCount = 1;
            calMonth.Name = "calMonth";
            calMonth.ShowToday = false;
            calMonth.TabIndex = 10;
            calMonth.DateChanged += calMonth_DateChanged;
            // 
            // cmsTasksOptions
            // 
            cmsTasksOptions.ImageScalingSize = new Size(20, 20);
            cmsTasksOptions.Items.AddRange(new ToolStripItem[] { ValidateTask, EditTask, DeleteTask, ReassignTask });
            cmsTasksOptions.Name = "cmsTasksOptions";
            cmsTasksOptions.Size = new Size(154, 108);
            // 
            // ValidateTask
            // 
            ValidateTask.Image = Properties.Resources.validateTask;
            ValidateTask.Name = "ValidateTask";
            ValidateTask.Size = new Size(153, 26);
            ValidateTask.Text = "Valider";
            ValidateTask.Click += ValidateTask_Click;
            // 
            // EditTask
            // 
            EditTask.Image = Properties.Resources.editTask;
            EditTask.Name = "EditTask";
            EditTask.Size = new Size(153, 26);
            EditTask.Text = "Modifier";
            EditTask.Click += EditTask_Click;
            // 
            // DeleteTask
            // 
            DeleteTask.Image = Properties.Resources.deleteTask;
            DeleteTask.Name = "DeleteTask";
            DeleteTask.Size = new Size(153, 26);
            DeleteTask.Text = "Supprimer";
            DeleteTask.Click += DeleteTask_Click;
            // 
            // ReassignTask
            // 
            ReassignTask.Image = Properties.Resources.unapproveTask;
            ReassignTask.Name = "ReassignTask";
            ReassignTask.Size = new Size(153, 26);
            ReassignTask.Text = "Réassigner";
            ReassignTask.Click += ReassignTask_Click;
            // 
            // frmMain
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(987, 694);
            Controls.Add(pnlContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "frmMain";
            Padding = new Padding(13, 12, 13, 12);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Life Pro Manager";
            Load += frmMain_Load;
            Shown += frmMain_Shown;
            KeyDown += frmMain_KeyDown;
            pnlContainer.ResumeLayout(false);
            tabMain.ResumeLayout(false);
            tabDates.ResumeLayout(false);
            tabDates.PerformLayout();
            tabTopics.ResumeLayout(false);
            tabTopics.PerformLayout();
            tabFinished.ResumeLayout(false);
            tabSettings.ResumeLayout(false);
            tabSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudTaskDescriptionFontSize).EndInit();
            pnlRight.ResumeLayout(false);
            cmsTasksOptions.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.ComponentModel.IContainer components = null;

        internal ImageList ilsTabs;
        private SaveFileDialog saveFileDialog1;
        private ToolTip ttpTotalTasksToComplete;

        private Panel pnlContainer;
        private TabControl tabMain;

        private TabPage tabDates;
        internal Panel pnlToday;
        private Label lblWeek;
        private Label lblToday;

        private TabPage tabTopics;
        private Button cmdNextTopic;
        private Button cmdPreviousTopic;
        internal Panel pnlTopics;
        private Label lblTopic;
        private Button cmdDeleteTopic;

        private TabPage tabFinished;
        private Button cmdDeleteFinishedTasks;
        internal Panel pnlFinished;

        private TabPage tabSettings;

        private Panel pnlRight;
        private Button cmdAddTask;
        private Button cmdSearch;
        internal ComboBox cboTopics;
        private Button cmdAddTopic;
        private Button cmdBirthdayCalendar;
        private Button cmdExportToHtml;
        private Button cmdNextDay;
        private Button cmdToday;
        private Button cmdPreviousDay;
        private Label lblTaskDescription;
        private MonthCalendar calMonth;

        internal ContextMenuStrip cmsTasksOptions;
        internal ToolStripMenuItem ValidateTask;
        internal ToolStripMenuItem EditTask;
        internal ToolStripMenuItem DeleteTask;
        internal ToolStripMenuItem ReassignTask;
        private LinkLabel lnkChangeAppColors;
        private LinkLabel lnkExportTasksToSql;
        private LinkLabel lnkAppInLanguage;
        private LinkLabel lnkInsertTasksFromSql;
        private NumericUpDown nudTaskDescriptionFontSize;
        private Label lblTaskDescriptionFontSize;
        private CheckBox chkRunAtWindowsStartup;
        private Label lblExportDeadlineAndTitle;
        private ComboBox cboAppLanguage;
        private CheckBox chkTopics;
        private CheckBox chkDescriptions;
        internal Panel pnlWeek;
    }
}

