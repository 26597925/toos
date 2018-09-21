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
    public partial class Clear : Form
    {
        private SerialPort comm = null;
        private StringBuilder builder = null;
        public Clear()
        {
            InitializeComponent();
        }

        private void Clear_Load(object sender, EventArgs e)
        {
            comm = new SerialPort();
            builder = new StringBuilder();//避免在事件处理方法中反复的创建
            //初始化下拉串口名称列表框
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            comboPortName.Items.AddRange(ports);
            comboPortName.SelectedIndex = comboPortName.Items.Count > 0 ? 0 : -1;
            comboBaudrate.SelectedIndex = comboBaudrate.Items.IndexOf("115200");

            //初始化SerialPort对象
            comm.NewLine = "\r\n";
            comm.RtsEnable = true;//根据实际情况吧。

            //添加事件注册
            comm.DataReceived += comm_DataReceived;
        }
        //串口事件
        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = comm.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据
            comm.Read(buf, 0, n);//读取缓冲数据
            builder.Clear();//清除字符串构造器的内容
            //因为要访问ui资源，所以需要使用invoke方式同步ui。
            this.Invoke((EventHandler)(delegate
            {
                //直接按ASCII规则转换成字符串
                builder.Append(Encoding.ASCII.GetString(buf));
                //设置自动换行
                this.txGet.WordWrap = true;
                //追加的形式添加到文本框末端，并滚动到最后。
                this.txGet.AppendText(builder.ToString());
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //根据当前串口对象，来判断操作
            if (comm.IsOpen)
            {
                //打开时点击，则关闭串口
                comm.Close();
            }
            else
            {
                //关闭时点击，则设置好端口，波特率后打开
                comm.PortName = comboPortName.Text;
                comm.BaudRate = int.Parse(comboBaudrate.Text);
                try
                {
                    comm.Open();
                    action.Visible = true;
                }
                catch (Exception ex)
                {
                    //捕获到异常信息，创建一个新的comm对象，之前的不能用了。
                    comm = new SerialPort();
                    //现实异常信息给客户。
                    MessageBox.Show(ex.Message);
                }
            }
            //设置按钮的状态
            buttonOpenClose.Text = comm.IsOpen ? "关闭" : "开启";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(comm.IsOpen){
                //comm.WriteLine("\r");
                //comm.WriteLine("sf probe 2;sf erase 0 300000;nand info;nand erase 0");
              /*  byte[] buf = new byte[137];
                buf[0] = 0x55;
                buf[1] = 0x00;
                buf[2] = 0x84;
                buf[3] = 0x63;
                buf[4] = 0x07;
                buf[5] = 0x00;
                buf[6] = 0x80;
                buf[7] = 0x00;
                comm.Write(buf, 0, 137);*/
                comm.Write(Protocol.connectDevice(), 0, Protocol.getDataLen());
                comm.Write(Protocol.lookVer(), 0, Protocol.getDataLen());
            }
        }
    }
}
