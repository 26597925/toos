namespace ClearProgram
{
    partial class Watch
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
            this.ControlTab = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // ControlTab
            // 
            this.ControlTab.Location = new System.Drawing.Point(12, 12);
            this.ControlTab.Name = "ControlTab";
            this.ControlTab.SelectedIndex = 0;
            this.ControlTab.Size = new System.Drawing.Size(1168, 694);
            this.ControlTab.TabIndex = 0;
            // 
            // Watch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1192, 709);
            this.Controls.Add(this.ControlTab);
            this.Name = "Watch";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "百思威手表测试工具";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Watch_FormClosing);
            this.Load += new System.EventHandler(this.Watch_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl ControlTab;
    }
}