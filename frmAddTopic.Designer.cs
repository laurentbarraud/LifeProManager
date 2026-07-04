namespace LifeProManager
{
    partial class frmAddTopic
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
            txtTopic = new TextBox();
            lblTopic = new Label();
            cmdCancel = new Button();
            cmdValidate = new Button();
            SuspendLayout();
            // 
            // txtTopic
            // 
            txtTopic.Location = new Point(121, 26);
            txtTopic.Margin = new Padding(4, 5, 4, 5);
            txtTopic.MaxLength = 32;
            txtTopic.Name = "txtTopic";
            txtTopic.Size = new Size(263, 27);
            txtTopic.TabIndex = 0;
            // 
            // lblTopic
            // 
            lblTopic.Font = new Font("Segoe UI Semilight", 10.8F);
            lblTopic.Location = new Point(24, 26);
            lblTopic.Margin = new Padding(4, 0, 4, 0);
            lblTopic.Name = "lblTopic";
            lblTopic.Size = new Size(85, 35);
            lblTopic.TabIndex = 1;
            lblTopic.Text = "Thème";
            // 
            // cmdCancel
            // 
            cmdCancel.BackgroundImage = Properties.Resources.cancel;
            cmdCancel.BackgroundImageLayout = ImageLayout.Zoom;
            cmdCancel.Cursor = Cursors.Hand;
            cmdCancel.FlatAppearance.BorderSize = 0;
            cmdCancel.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdCancel.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdCancel.FlatStyle = FlatStyle.Flat;
            cmdCancel.Location = new Point(337, 77);
            cmdCancel.Margin = new Padding(0);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(36, 45);
            cmdCancel.TabIndex = 2;
            cmdCancel.UseVisualStyleBackColor = true;
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdValidate
            // 
            cmdValidate.BackgroundImage = Properties.Resources.validate;
            cmdValidate.BackgroundImageLayout = ImageLayout.Zoom;
            cmdValidate.Cursor = Cursors.Hand;
            cmdValidate.FlatAppearance.BorderSize = 0;
            cmdValidate.FlatAppearance.MouseDownBackColor = Color.Transparent;
            cmdValidate.FlatAppearance.MouseOverBackColor = Color.Transparent;
            cmdValidate.FlatStyle = FlatStyle.Flat;
            cmdValidate.Location = new Point(392, 77);
            cmdValidate.Margin = new Padding(0);
            cmdValidate.Name = "cmdValidate";
            cmdValidate.Size = new Size(36, 45);
            cmdValidate.TabIndex = 3;
            cmdValidate.UseVisualStyleBackColor = true;
            cmdValidate.Click += cmdValidate_Click;
            // 
            // frmAddTopic
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(230, 235, 239);
            ClientSize = new Size(467, 133);
            Controls.Add(cmdValidate);
            Controls.Add(cmdCancel);
            Controls.Add(lblTopic);
            Controls.Add(txtTopic);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmAddTopic";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ajouter un thème";
            Load += frmAddTopic_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtTopic;
        private System.Windows.Forms.Label lblTopic;
        private System.Windows.Forms.Button cmdValidate;
        private System.Windows.Forms.Button cmdCancel;
    }
}
