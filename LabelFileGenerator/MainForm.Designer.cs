namespace LabelFileGenerator
{
    partial class MainForm
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.aosServiceFolderLabel = new System.Windows.Forms.Label();
            this.aosServiceFolderComboBox = new System.Windows.Forms.ComboBox();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.GenerateLabelFilesButton = new System.Windows.Forms.Button();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.labelFileProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // aosServiceFolderLabel
            // 
            this.aosServiceFolderLabel.AutoSize = true;
            this.aosServiceFolderLabel.Location = new System.Drawing.Point(128, 25);
            this.aosServiceFolderLabel.Name = "aosServiceFolderLabel";
            this.aosServiceFolderLabel.Size = new System.Drawing.Size(97, 13);
            this.aosServiceFolderLabel.TabIndex = 1;
            this.aosServiceFolderLabel.Text = "AOS Service folder";
            // 
            // aosServiceFolderComboBox
            // 
            this.aosServiceFolderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.aosServiceFolderComboBox.FormattingEnabled = true;
            this.aosServiceFolderComboBox.Location = new System.Drawing.Point(131, 42);
            this.aosServiceFolderComboBox.Name = "aosServiceFolderComboBox";
            this.aosServiceFolderComboBox.Size = new System.Drawing.Size(378, 21);
            this.aosServiceFolderComboBox.TabIndex = 2;
            this.aosServiceFolderComboBox.SelectedIndexChanged += new System.EventHandler(this.aosServiceFolderComboBox_SelectedIndexChanged);
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Location = new System.Drawing.Point(131, 97);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(378, 21);
            this.languageComboBox.TabIndex = 4;
            // 
            // languageLabel
            // 
            this.languageLabel.AutoSize = true;
            this.languageLabel.Location = new System.Drawing.Point(128, 80);
            this.languageLabel.Name = "languageLabel";
            this.languageLabel.Size = new System.Drawing.Size(55, 13);
            this.languageLabel.TabIndex = 3;
            this.languageLabel.Text = "Language";
            // 
            // GenerateLabelFilesButton
            // 
            this.GenerateLabelFilesButton.Location = new System.Drawing.Point(237, 139);
            this.GenerateLabelFilesButton.Name = "GenerateLabelFilesButton";
            this.GenerateLabelFilesButton.Size = new System.Drawing.Size(144, 31);
            this.GenerateLabelFilesButton.TabIndex = 5;
            this.GenerateLabelFilesButton.Text = "Generate label files";
            this.GenerateLabelFilesButton.UseVisualStyleBackColor = true;
            this.GenerateLabelFilesButton.Click += new System.EventHandler(this.GenerateLabelFilesButton_Click);
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(15, 251);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logTextBox.Size = new System.Drawing.Size(604, 180);
            this.logTextBox.TabIndex = 6;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(15, 203);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(604, 18);
            this.progressBar.TabIndex = 7;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(12, 187);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(325, 13);
            this.progressLabel.TabIndex = 8;
            this.progressLabel.Text = "This will take a while, specially for SYS label file. Go grab a coffee :)";
            this.progressLabel.Visible = false;
            // 
            // labelFileProgressBar
            // 
            this.labelFileProgressBar.Location = new System.Drawing.Point(15, 227);
            this.labelFileProgressBar.Name = "labelFileProgressBar";
            this.labelFileProgressBar.Size = new System.Drawing.Size(604, 18);
            this.labelFileProgressBar.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 443);
            this.Controls.Add(this.labelFileProgressBar);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.GenerateLabelFilesButton);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.languageLabel);
            this.Controls.Add(this.aosServiceFolderComboBox);
            this.Controls.Add(this.aosServiceFolderLabel);
            this.Name = "MainForm";
            this.Text = "Label file generator (by Pedro Tornich)";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label aosServiceFolderLabel;
        private System.Windows.Forms.ComboBox aosServiceFolderComboBox;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.Button GenerateLabelFilesButton;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.ProgressBar labelFileProgressBar;
    }
}

