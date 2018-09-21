using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Excel;
namespace CreatSerial
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sarial.ValidatingType = typeof(int);
            sarial.Mask = @"100000000";
            sarial.PromptChar = '0'; // 设置提示字符。
            sarial.HidePromptOnLeave = false; // 无焦点时，输入掩码中的提示字符仍保持显示。
            sarial.TextMaskFormat = MaskFormat.IncludePromptAndLiterals; // 返回用户输入的文本、掩码中定义的任意文本字符以及提示字符的任意实例。
            sarial.Focus();
            len.ValidatingType = typeof(int);
            len.Mask = @"1000";
            len.PromptChar = '0'; // 设置提示字符。
            len.HidePromptOnLeave = false; // 无焦点时，输入掩码中的提示字符仍保持显示。
            len.TextMaskFormat = MaskFormat.IncludePromptAndLiterals; // 返回用户输入的文本、掩码中定义的任意文本字符以及提示字符的任意实例。
        }

        private void sarial_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            if (e.Position < sarial.TextLength)
            {
                ToolTip toolTip = new ToolTip();
                toolTip.IsBalloon = true;   // 使用气球状窗口。
                toolTip.ToolTipIcon = ToolTipIcon.Warning;
                toolTip.ToolTipTitle = "系统提示";
                toolTip.Show("请输入数字！", sarial, 1000);
            }
        }

        private void len_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            if (e.Position < sarial.TextLength)
            {
                ToolTip toolTip = new ToolTip();
                toolTip.IsBalloon = true;   // 使用气球状窗口。
                toolTip.ToolTipIcon = ToolTipIcon.Warning;
                toolTip.ToolTipTitle = "系统提示";
                toolTip.Show("请输入数字！", len, 1000);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "序列号生成工具----百思威科技";
            saveFileDialog1.Filter = "Excel(*.xls)|*.xls";
            saveFileDialog1.FileName = string.Format("序列号_{0}", DateTime.Now.ToString("yyyyMMdd"));
            DialogResult result = saveFileDialog1.ShowDialog();
            Excel._Application xlapp = new Excel.Application();
            Excel.Workbook xlbook = xlapp.Workbooks.Add(true);
            Excel.Worksheet xlsheet = (Excel.Worksheet)xlbook.Worksheets[1];
            int RowCount = Convert.ToInt32(len.Text);
            int RowIndex = 0;
            for (int i = 0; i < RowCount; i++)
            {
                RowIndex++;
                xlsheet.Cells[RowIndex, 1] = "SN:" + (Convert.ToInt32(sarial.Text) + i).ToString();
            }
            xlbook.Saved = true;
            xlbook.SaveCopyAs(saveFileDialog1.FileName);
            xlapp.Quit();
            MessageBox.Show("导出成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
