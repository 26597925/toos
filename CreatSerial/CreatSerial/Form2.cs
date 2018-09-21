using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CreatSerial
{
    struct UInt96
    {
        ulong hi;
        uint lo;

        // 构造函数
        public UInt96(ulong h, uint l)
        {
            hi = h;
            lo = l;
        }

        // 解析字符串获得 UInt96
        public static UInt96 Parse(string s)
        {
            s = s.Replace(" ", "");
            s = s.Replace(":", "");
            if (s.Length == 0) return new UInt96();
            ulong h = 0;
            uint l = 0;
            if (s.Length <= 8) uint.TryParse(s, NumberStyles.HexNumber, null, out l);
            else
            {
                ulong.TryParse(s.Substring(0, s.Length - 8), NumberStyles.HexNumber, null, out h);
                uint.TryParse(s.Substring(s.Length - 8, 8), NumberStyles.HexNumber, null, out l);
            }
            return new UInt96(h, l);
        }

        // 返回加1后的结果
        public UInt96 Inc()
        {
            if (lo < uint.MaxValue) return new UInt96(hi, lo + 1);
            return new UInt96(hi + 1, 0);
        }

        // 返回减1后的结果
        public UInt96 Dec()
        {
            if (lo > 0) return new UInt96(hi, lo - 1);
            return new UInt96(hi - 1, uint.MaxValue);
        }

        // 自增运算符
        public static UInt96 operator ++(UInt96 x)
        {
            return x.Inc();
        }

        // 自减运算符
        public static UInt96 operator --(UInt96 x)
        {
            return x.Dec();
        }

        // 十六进制字符串表示
        public  string ToString(string split)
        {
            return string.Join(split, Regex.Split(string.Format("{0:X4}{1:X6}", hi, lo), "(?!^)(?=(?:.{2})+$)"));
        }
    }
 
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void creat_Click(object sender, EventArgs e)
        {
           // textBox2.Text = UInt96.Parse(textBox1.Text).Inc().ToString();
            saveFileDialog1.Title = "MAC地址生成工具----百思威科技";
            saveFileDialog1.Filter = "Excel(*.xls)|*.xls";
            saveFileDialog1.FileName = string.Format("MAC地址_{0}", DateTime.Now.ToString("yyyyMMdd"));
            DialogResult result = saveFileDialog1.ShowDialog();
            Excel._Application xlapp = new Excel.Application();
            Excel.Workbook xlbook = xlapp.Workbooks.Add(true);
            Excel.Worksheet xlsheet = (Excel.Worksheet)xlbook.Worksheets[1];
          //  MessageBox.Show(textBox2.Text);
            int RowCount = Convert.ToInt32(textBox2.Text);
            int RowIndex = 0;
            string val = textBox1.Text;
            val = val.Replace(":", "");
            for (int i = 0; i < RowCount; i++)
            {
                RowIndex++;
                if (i != 0)
                {
                    if (radioButton1.Checked)
                    {
                        val = UInt96.Parse(val).Inc().ToString(":");
                    }
                    else {
                        val = UInt96.Parse(val).Inc().ToString("");
                    }
                    xlsheet.Cells[RowIndex, 1] = val;
                }
                else
                {
                    if (radioButton1.Checked)
                    {
                        string[] sArray = Regex.Split(val, @"(\w{2})");
                        List<string> listTemp = new List<string>();
                        foreach (string s in sArray)
                        {
                            if (string.IsNullOrEmpty(s)) continue;
                            listTemp.Add(s);
                        }
                        string[] newlist = listTemp.ToArray();
                        val = String.Join(":", newlist);
                    }
                    xlsheet.Cells[RowIndex, 1] = val;
                }
            }
            xlbook.Saved = true;
            xlbook.SaveCopyAs(saveFileDialog1.FileName);
            xlapp.Quit();
            MessageBox.Show("导出成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
