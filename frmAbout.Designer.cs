namespace LifeProManager
{
    partial class frmAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblLicence = new Label();
            cmdValidate = new Button();
            SuspendLayout();
            // 
            // lblLicence
            // 
            lblLicence.BackColor = Color.Transparent;
            lblLicence.Font = new Font("Segoe UI Semilight", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLicence.ForeColor = Color.FromArgb(34, 61, 64);
            lblLicence.Location = new Point(22, 28);
            lblLicence.Name = "lblLicence";
            lblLicence.Size = new Size(546, 306);
            lblLicence.TabIndex = 9;
            lblLicence.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmdValidate
            // 
            cmdValidate.BackgroundImage = Properties.Resources.validateTask;
            cmdValidate.BackgroundImageLayout = ImageLayout.Zoom;
            cmdValidate.Cursor = Cursors.Hand;
            cmdValidate.FlatAppearance.BorderSize = 0;
            cmdValidate.FlatStyle = FlatStyle.Flat;
            cmdValidate.Location = new Point(274, 353);
            cmdValidate.Margin = new Padding(3, 4, 3, 4);
            cmdValidate.Name = "cmdValidate";
            cmdValidate.Size = new Size(36, 45);
            cmdValidate.TabIndex = 16;
            // 
            // frmAbout
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(593, 411);
            Controls.Add(cmdValidate);
            Controls.Add(lblLicence);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmAbout";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "About this software";
            Load += frmAbout_Load;
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblLicence;
        private Button cmdValidate;
    }
}