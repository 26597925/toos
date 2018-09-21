using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;


namespace ClearProgram
{

    public partial class Watch : Form
    {
        private SerialPort mCom = null;
        
       
        public Watch()
        {
            InitializeComponent();
        }

        private void Watch_Load(object sender, EventArgs e)
        {
            mCom = new SerialPort();
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0, len = ports.Length; i < len; i++ )
            {
                if (Protocol.isPort((String)ports[i]))
                {
                    TabPage tabPage = new TabPage();
                    tabPage.Text = "串口：" + ports[i];
                    Control control = new Control((String)ports[i]);
                    control.openCom();
                    tabPage.Controls.Add(control);
                    ControlTab.TabPages.Add(tabPage);
                }
            }
        }

        //窗体关闭前，关闭搜索串口资源
        private void Watch_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0, len = ControlTab.TabPages.Count; i < len; i++)
            {
                Control control = (Control)ControlTab.TabPages[i].Controls[0];
                control.closeCom();
            }
            Dispose();
        }

    }
}
