namespace CreatSerial
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.creat = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.len = new System.Windows.Forms.MaskedTextBox();
            this.sarial = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "起始系列号：";
            // 
            // creat
            // 
            this.creat.Location = new System.Drawing.Point(131, 94);
            this.creat.Name = "creat";
            this.creat.Size = new System.Drawing.Size(208, 23);
            this.creat.TabIndex = 2;
            this.creat.Text = "生成并导出excel";
            this.creat.UseVisualStyleBackColor = true;
            this.creat.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "长度：";
            // 
            // len
            // 
            this.len.Location = new System.Drawing.Point(132, 58);
            this.len.Mask = "9999";
            this.len.Name = "len";
            this.len.Size = new System.Drawing.Size(207, 21);
            this.len.TabIndex = 5;
            this.len.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.len_MaskInputRejected);
            // 
            // sarial
            // 
            this.sarial.Location = new System.Drawing.Point(132, 20);
            this.sarial.Mask = "999999999";
            this.sarial.Name = "sarial";
            this.sarial.Size = new System.Drawing.Size(207, 21);
            this.sarial.TabIndex = 4;
            this.sarial.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.sarial_MaskInputRejected);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 129);
            this.Controls.Add(this.len);
            this.Controls.Add(this.sarial);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.creat);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "序列号生成工具";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button creat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MaskedTextBox len;
        private System.Windows.Forms.MaskedTextBox sarial;
    }
}

