namespace LifeProManager
{
    partial class frmAddTask
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblDescription = new Label();
            lblPriority = new Label();
            lblDeadline = new Label();
            lblTitle = new Label();
            lblTopic = new Label();
            cboTopics = new ComboBox();
            txtTitle = new TextBox();
            dtpDeadline = new DateTimePicker();
            chkImportant = new CheckBox();
            txtDescription = new TextBox();
            chkRepeatable = new CheckBox();
            chkBirthday = new CheckBox();
            lblYear = new Label();
            numYear = new NumericUpDown();
            cmdCancel = new Button();
            cmdValidate = new Button();
            ((System.ComponentModel.ISupportInitialize)numYear).BeginInit();
            SuspendLayout();
            // 
            // lblDescription
            // 
            lblDescription.Font = new Font("Segoe UI Semilight", 10.2F);
            lblDescription.Location = new Point(28, 155);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(119, 35);
            lblDescription.TabIndex = 0;
            lblDescription.Text = "Description";
            // 
            // lblPriority
            // 
            lblPriority.Font = new Font("Segoe UI Semilight", 10.2F);
            lblPriority.Location = new Point(28, 370);
            lblPriority.Name = "lblPriority";
            lblPriority.Size = new Size(119, 35);
            lblPriority.TabIndex = 1;
            lblPriority.Text = "Priorité";
            // 
            // lblDeadline
            // 
            lblDeadline.Font = new Font("Segoe UI Semilight", 10.2F);
            lblDeadline.Location = new Point(28, 40);
            lblDeadline.Name = "lblDeadline";
            lblDeadline.Size = new Size(119, 35);
            lblDeadline.TabIndex = 2;
            lblDeadline.Text = "Échéance";
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI Semilight", 10.2F);
            lblTitle.Location = new Point(28, 98);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(119, 35);
            lblTitle.TabIndex = 3;
            lblTitle.Text = "Titre";
            // 
            // lblTopic
            // 
            lblTopic.Font = new Font("Segoe UI Semilight", 10.2F);
            lblTopic.Location = new Point(28, 492);
            lblTopic.Name = "lblTopic";
            lblTopic.Size = new Size(119, 35);
            lblTopic.TabIndex = 4;
            lblTopic.Text = "Thème";
            // 
            // cboTopics
            // 
            cboTopics.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTopics.Font = new Font("Segoe UI Semilight", 9F);
            cboTopics.Location = new Point(156, 491);
            cboTopics.Margin = new Padding(3, 4, 3, 4);
            cboTopics.Name = "cboTopics";
            cboTopics.Size = new Size(160, 28);
            cboTopics.TabIndex = 5;
            // 
            // txtTitle
            // 
            txtTitle.Font = new Font("Microsoft Sans Serif", 9F);
            txtTitle.Location = new Point(156, 96);
            txtTitle.Margin = new Padding(3, 4, 3, 4);
            txtTitle.MaxLength = 70;
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(275, 24);
            txtTitle.TabIndex = 6;
            txtTitle.TextChanged += txtTitle_TextChanged;
            // 
            // dtpDeadline
            // 
            dtpDeadline.Font = new Font("Microsoft Sans Serif", 9F);
            dtpDeadline.Location = new Point(155, 40);
            dtpDeadline.Margin = new Padding(3, 4, 3, 4);
            dtpDeadline.Name = "dtpDeadline";
            dtpDeadline.Size = new Size(276, 24);
            dtpDeadline.TabIndex = 7;
            // 
            // chkImportant
            // 
            chkImportant.CheckAlign = ContentAlignment.MiddleRight;
            chkImportant.Font = new Font("Microsoft Sans Serif", 9F);
            chkImportant.Location = new Point(155, 365);
            chkImportant.Margin = new Padding(3, 4, 3, 4);
            chkImportant.Name = "chkImportant";
            chkImportant.Size = new Size(126, 42);
            chkImportant.TabIndex = 8;
            chkImportant.Text = "Important";
            // 
            // txtDescription
            // 
            txtDescription.Font = new Font("Microsoft Sans Serif", 9F);
            txtDescription.Location = new Point(155, 159);
            txtDescription.Margin = new Padding(3, 4, 3, 4);
            txtDescription.MaxLength = 400;
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(275, 180);
            txtDescription.TabIndex = 9;
            // 
            // chkRepeatable
            // 
            chkRepeatable.CheckAlign = ContentAlignment.MiddleRight;
            chkRepeatable.Font = new Font("Microsoft Sans Serif", 9F);
            chkRepeatable.Location = new Point(291, 365);
            chkRepeatable.Margin = new Padding(3, 4, 3, 4);
            chkRepeatable.Name = "chkRepeatable";
            chkRepeatable.Size = new Size(138, 42);
            chkRepeatable.TabIndex = 10;
            chkRepeatable.Text = "Répétable";
            // 
            // chkBirthday
            // 
            chkBirthday.CheckAlign = ContentAlignment.MiddleRight;
            chkBirthday.Font = new Font("Microsoft Sans Serif", 9F);
            chkBirthday.Location = new Point(154, 426);
            chkBirthday.Margin = new Padding(3, 4, 3, 4);
            chkBirthday.Name = "chkBirthday";
            chkBirthday.Size = new Size(127, 38);
            chkBirthday.TabIndex = 11;
            chkBirthday.Text = "Anniversaire";
            chkBirthday.CheckedChanged += chkBirthday_CheckedChanged;
            // 
            // lblYear
            // 
            lblYear.Font = new Font("Microsoft Sans Serif", 9F);
            lblYear.Location = new Point(291, 426);
            lblYear.Name = "lblYear";
            lblYear.Size = new Size(62, 38);
            lblYear.TabIndex = 12;
            lblYear.Text = "Année";
            lblYear.Visible = false;
            // 
            // numYear
            // 
            numYear.Font = new Font("Microsoft Sans Serif", 10.2F);
            numYear.Location = new Point(357, 426);
            numYear.Margin = new Padding(3, 4, 3, 4);
            numYear.Name = "numYear";
            numYear.Size = new Size(70, 27);
            numYear.TabIndex = 13;
            numYear.Visible = false;
            // 
            // cmdCancel
            // 
            cmdCancel.BackgroundImage = Properties.Resources.deleteTask;
            cmdCancel.BackgroundImageLayout = ImageLayout.Zoom;
            cmdCancel.Cursor = Cursors.Hand;
            cmdCancel.FlatAppearance.BorderSize = 0;
            cmdCancel.FlatStyle = FlatStyle.Flat;
            cmdCancel.Location = new Point(326, 549);
            cmdCancel.Margin = new Padding(3, 4, 3, 4);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(36, 45);
            cmdCancel.TabIndex = 14;
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdValidate
            // 
            cmdValidate.BackgroundImage = Properties.Resources.validateTask;
            cmdValidate.BackgroundImageLayout = ImageLayout.Zoom;
            cmdValidate.Cursor = Cursors.Hand;
            cmdValidate.FlatAppearance.BorderSize = 0;
            cmdValidate.FlatStyle = FlatStyle.Flat;
            cmdValidate.Location = new Point(381, 549);
            cmdValidate.Margin = new Padding(3, 4, 3, 4);
            cmdValidate.Name = "cmdValidate";
            cmdValidate.Size = new Size(36, 45);
            cmdValidate.TabIndex = 15;
            cmdValidate.Click += cmdValidate_Click;
            // 
            // frmAddTask
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(230, 235, 239);
            ClientSize = new Size(476, 618);
            Controls.Add(lblDescription);
            Controls.Add(lblPriority);
            Controls.Add(lblDeadline);
            Controls.Add(lblTitle);
            Controls.Add(lblTopic);
            Controls.Add(cboTopics);
            Controls.Add(txtTitle);
            Controls.Add(dtpDeadline);
            Controls.Add(chkImportant);
            Controls.Add(txtDescription);
            Controls.Add(chkRepeatable);
            Controls.Add(chkBirthday);
            Controls.Add(lblYear);
            Controls.Add(numYear);
            Controls.Add(cmdCancel);
            Controls.Add(cmdValidate);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmAddTask";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ajouter une tâche";
            Load += frmAddTask_Load;
            ((System.ComponentModel.ISupportInitialize)numYear).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.Label lblDeadline;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTopic;
        private System.Windows.Forms.ComboBox cboTopics;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.DateTimePicker dtpDeadline;
        private System.Windows.Forms.CheckBox chkImportant;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.CheckBox chkRepeatable;
        private System.Windows.Forms.CheckBox chkBirthday;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.NumericUpDown numYear;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdValidate;
    }
}
