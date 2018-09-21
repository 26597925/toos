using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Reflection;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace ClearProgram
{

    public partial class Control : UserControl
    {
        SynchronizationContext mSyncContext = null;

        private CommPort mCom = null;

        private String mName = "";

        private Thread mListenThread;

        private bool mContinue = true;

        private bool mIsStop = false;

        private Boolean mIsExport = true;

        public delegate void ParseEventHandler(Object sender, ParseEventArgs e);
        public event ParseEventHandler Connect;
        public event ParseEventHandler LookVer;
        public event ParseEventHandler LookServer;
        public event ParseEventHandler LookImei;
        public event ParseEventHandler LookNumber;
        public event ParseEventHandler LookQzone;
        public event ParseEventHandler LookSos;
        public event ParseEventHandler LookMonitor;
        public event ParseEventHandler LookUpload;

        public class ParseEventArgs : EventArgs
        {
            public readonly String mLine;
            public ParseEventArgs(String line)
            {
                mLine = line;
            }
        }

        public Control(String name)
        {
            mName = name;
            InitializeComponent();
            mSyncContext = SynchronizationContext.Current;
            mListenThread = new Thread(listen) { IsBackground = true };
            mListenThread.Start();
            DebugInfo.WordWrap = true;

            Connect += connect;
            LookVer += look_ver;
            LookServer += look_server;
            LookImei += look_imei;
            LookNumber += look_number;
            LookQzone += look_qzone;
            LookSos += look_sos;
            LookMonitor += look_monitor;
            LookUpload += look_upload;
        }

        public void openCom()
        {
           if (mCom == null)
            {
                mCom = new CommPort();
                mCom.PortName = mName;
                mCom.BaudRate = 115200;
                mCom.Parity = 0; //奇偶校验 
                mCom.StopBits = 1;//停止位 
                mCom.ReadTimeout = 1000; //读超时 
            }

            if (mCom.Opened)
            {
                mCom.Close();
            }
            
            connect();
        }

        public void closeCom() {
            if (mCom.IsOpen)
            {
                mIsStop = true;
                mContinue = false;

                /*if (mListenThread != null)
                {
                    mListenThread.Join();
                }*/

                mCom.Close();
            }
        }

        //监听串口状态
        private void listen()
        {
            int count = 1024;
            while (mContinue)
            {
                if (!mIsStop)
                {

                    if (!Protocol.isPort(mName) || !mCom.Opened)
                    {

                        mIsStop = true;
                        mSyncContext.Post(status_Callback, mIsStop);
                    }
                    else 
                    {
                        if (mCom != null)
                        {
                            byte[] read = mCom.Read(count);
                            if (read.Length > 0 && read[0] == 0x55)
                            {
                                mSyncContext.Post(add_row, read);
                            }
                        }
                    }
                }
                else 
                {
                    if (Protocol.isPort(mName))
                    {
                        mIsStop = false;
                        mSyncContext.Post(status_Callback, mIsStop);
                    }
                }
            }
        }

        private void arrToList(byte[] bufer, List<byte> list, int start, int end) {
            int len = bufer.Length;
            if (end != 0) len = end;
            for (int i = start; i < len; i++)
            {
                list.Add(bufer[i]);
            }
        }

        private void connect()
        {
            if (mIsStop) return;
            mCom.Open();
           
            if (mCom != null && mCom.IsOpen)
            {
                SerialStatusText.Text = "打开";
            }

            mCom.Write(Protocol.connectDevice(), 0, Protocol.getDataLen());
        }

        private void status_Callback(object isMove)
        {
            if (mIsStop) return;
            bool move = (Boolean) isMove;
            if (move)
            {
                mCom.Close();
                SerialStatusText.Text = "关闭";
                DataStatusText.Text = "正在链接";
            }
            else{
                connect();
            }
          
        }

        private void add_row(object obj) {
            if (mIsStop) return;

            if (mIsExport)
            {
                byte[] bytes = (byte[])obj;
                String data = HexCon.ByteToString(bytes);
                String[] arr = Regex.Split(data, "55", RegexOptions.IgnoreCase);
                for (int i = 0, len = arr.Length; i < len; i++ )
                {
                    if(i != 0 ){
                        data = "55" + arr[i];
                        if (data.Length > 43)
                        {
                            data = data.Substring(36);
                            data = data.Substring(0, data.Length-7);
                            byte[] result = HexCon.StringToByte(data);
                            //DebugInfo.AppendText(HexCon.ByteToString(result) + "\n");
                            data = Encoding.Default.GetString(result);
                            if (data != "")
                            {
                                parseData(data);
                                int line = DebugInfo.GetLineFromCharIndex(DebugInfo.TextLength) + 1;
                                String time = DateTime.Now.ToLongTimeString().ToString();
                                String text = line.ToString().PadRight(6, ' ') + time.PadRight(12, ' ') + data.Replace("\n", "");
                                DebugInfo.AppendText(text);
                                DebugInfo.AppendText("\n");
                            }
                        }
                    } 
                }
            }
        }

        private void ReadServer_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.readServer();
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void readImei_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.readImei();
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void ReadNumber_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.readNumber();
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void ReadQzone_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.readQzone();
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void ReadSOS_Click(object sender, EventArgs e)
        {
             byte[] command = Protocol.readSos();
             mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void ReadMonitor_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.readMonitor();
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void ReadUpload_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.readUpload();
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void Send_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.sendCommand(Commond.Text);
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void WriteServer_Click(object sender, EventArgs e)
        {
            byte[] command;
            if (Domain.Checked)
            {
                command = Protocol.writeServer(IpUrl.Text, PortText.Text, "1");
            }
            else {
                command = Protocol.writeServer(IpUrl.Text, PortText.Text, "0");
            }
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void WriteImei_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.writeImei(ImeiText.Text);
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void WriteNumber_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.writeNumber(CenterNumberText.Text, SlaveNumberText.Text);
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void WriteQzone_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.writeQzone(LanguageText.Text, QzoneText.Text);
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void WriteSos_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.writeSos(SOS1.Text, SOS2.Text, SOS3.Text);
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void ClearData_Click(object sender, EventArgs e)
        {
            Commond.Text = "";
        }

        private void WriteUpload_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.writeUpload(Upload.Text);
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void WriteMonitor_Click(object sender, EventArgs e)
        {
            byte[] command = Protocol.writeMonitor(Monitor.Text);
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                byte[] command = Protocol.testDevice();
                mCom.Write(command, 0, Protocol.getDataLen());
            }
        }

         private void Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "(*.log)|*.log|(*.*)|*.*";
            sfd.AddExtension = true;
            sfd.RestoreDirectory = true;
            sfd.Title = "保存调试文件";
            sfd.FileName = "debug.log";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                try
                {
                    sw.Write(DebugInfo.Text);
                    sw.Flush();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }
            }             
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            if (mIsExport)
            {
                mIsExport = false;
                Pause.Text = "继续显示";
            }
            else {
                mIsExport = true;
                Pause.Text = "暂停显示";
            }
        }

        private void ClearDisplay_Click(object sender, EventArgs e)
        {
            DebugInfo.Text = "";
        }

        private void parseData(String data)
        {
            if (mIsStop) return;
            ParseEventArgs e = null;
            if (data.Contains("AT:connect ok!#"))
            {
                e = new ParseEventArgs(data);
                OnConnect(e);
            }
            else if (data.Contains("AT+uart:ver,"))
            {
                e = new ParseEventArgs(data);
                OnLookVer(e);
            }
            else if (data.Contains("AT+uart:imei,"))
            {
                e = new ParseEventArgs(data);
                OnLookImei(e);
            }
            else if (data.Contains("AT+uart:ip_url,"))
            {
                e = new ParseEventArgs(data);
                OnLookServer(e);
            }
            else if (data.Contains("AT+uart:center_number,"))
            {
                e = new ParseEventArgs(data);
                OnLookNumber(e);
            }
            else if (data.Contains("AT+uart:language,"))
            {
                e = new ParseEventArgs(data);
                OnLookQzone(e);
            }
            else if (data.Contains("AT+uart:sos1,"))
            {
                e = new ParseEventArgs(data);
                OnLookSos(e);
            }
            else if (data.Contains("AT+uart:monitor,"))
            {
                e = new ParseEventArgs(data);
                OnLookMonitor(e);
            }
            else if (data.Contains("AT+uart:upload,"))
            {
                e = new ParseEventArgs(data);
                OnLookUpload(e);
            }
        }

        public void connect(Object sender, ParseEventArgs e)
        {
            if (mIsStop) return;
            DataStatusText.Text = "已连接";
            byte[] command = Protocol.lookVer();
            mCom.Write(command, 0, Protocol.getDataLen());
        }

        public void look_ver(Object sender, ParseEventArgs e)
        {
            if (mIsStop) return;
            Regex reg = new Regex("AT\\+uart:ver,(.+?)#");
            Match match = reg.Match(e.mLine);
            string value = match.Groups[1].Value;
            if(value != "?") LookVerText.Text = value;
        }

        public void look_server(Object sender, ParseEventArgs e)
        {
            if (mIsStop) return;
            Regex reg = new Regex("AT\\+uart:ip_url,(.+?),port,(.+?),url,(.+?)#");
            Match match = reg.Match(e.mLine);
            String ip = match.Groups[1].Value;
            String port = match.Groups[2].Value;
            String isIp = match.Groups[3].Value;
            if (ip != "?") IpUrl.Text = ip;
            if (port != "?") PortText.Text = port;
            if (isIp == "1")
            {
                Domain.Checked = true;
            }
        }

        public void look_imei(Object sender, ParseEventArgs e)
        {
            if (mIsStop) return;
            Regex reg = new Regex("AT\\+uart:imei,(.+?)#");
            Match match = reg.Match(e.mLine);
            String imei = match.Groups[1].Value;
            if (imei != "?") ImeiText.Text = imei;
        }

        public void look_number(Object sender, ParseEventArgs e) 
        {
            if (mIsStop) return;
            Regex reg = new Regex("AT\\+uart:center_number,(.+?),slave_number,(.+?)#");
            Match match = reg.Match(e.mLine);
            String centerNumber = match.Groups[1].Value;
            String slaveNumber = match.Groups[2].Value;
            if (centerNumber != "?") CenterNumberText.Text = centerNumber;
            if (slaveNumber != "?") SlaveNumberText.Text = slaveNumber;
        }

        public void look_qzone(Object sender, ParseEventArgs e)
        {
            if (mIsStop) return;
            Regex reg = new Regex("AT\\+uart:language,(.+?),zone,(.+?)#");
            Match match = reg.Match(e.mLine);
            String language = match.Groups[1].Value;
            String zone = match.Groups[2].Value;
            if (language != "?") LanguageText.Text = language;
            if (zone != "?") QzoneText.Text = zone;
        }

        public void look_sos(Object sender, ParseEventArgs e) 
        {
            if (mIsStop) return;
            Regex reg = new Regex("AT\\+uart:sos1,(.+?),sos2,(.+?),sos3,(.+?)#");
            Match match = reg.Match(e.mLine);
            String sos1 = match.Groups[1].Value;
            String sos2 = match.Groups[2].Value;
            String sos3 = match.Groups[3].Value;
            if (sos1 != "?") SOS1.Text = sos1;
            if (sos2 != "?") SOS2.Text = sos2;
            if (sos3 != "?") SOS3.Text = sos3;
        }

        public void look_monitor(Object sender, ParseEventArgs e) 
        {
            if (mIsStop) return;
            Regex reg = new Regex("AT\\+uart:monitor,(.+?)#");
            Match match = reg.Match(e.mLine);
            String monitor = match.Groups[1].Value;
            if (monitor != "?") Monitor.Text = monitor;
        }

        public void look_upload(Object sender, ParseEventArgs e)
        {
            if (mIsStop) return;
            Regex reg = new Regex("AT\\+uart:upload,(.+?)#");
            Match match = reg.Match(e.mLine);
            String upload = match.Groups[1].Value;
            if (upload != "?") Upload.Text = upload;
        }

        protected virtual void OnConnect(ParseEventArgs e)
        {
            if (Connect != null)
            {
                Connect(this, e);
            }
        }

        protected virtual void OnLookVer(ParseEventArgs e)
        {
            if (LookVer != null)
            {
                LookVer(this, e);
            }
        }

        protected virtual void OnLookServer(ParseEventArgs e)
        {
            if (LookServer != null)
            {
                LookServer(this, e);
            }
        }

        protected virtual void OnLookImei(ParseEventArgs e)
        {
            if (LookImei != null)
            {
                LookImei(this, e);
            }
        }

        protected virtual void OnLookNumber(ParseEventArgs e)
        {
            if (LookNumber != null)
            {
                LookNumber(this, e);
            }
        }

        protected virtual void OnLookQzone(ParseEventArgs e)
        {
            if (LookQzone != null)
            {
                LookQzone(this, e);
            }
        }

        protected virtual void OnLookSos(ParseEventArgs e)
        {
            if (LookSos != null)
            {
                LookSos(this, e);
            }
        }

        protected virtual void OnLookMonitor(ParseEventArgs e)
        {
            if (LookMonitor != null)
            {
                LookMonitor(this, e);
            }
        }

        protected virtual void OnLookUpload(ParseEventArgs e)
        {
            if (LookUpload != null)
            {
                LookUpload(this, e);
            }
        }

        class HexCon
        {
            // 把十六进制字符串转换成字节型和把字节型转换成十六进制字符串 converter hex string to byte and byte to hex string
            public static string ByteToString(byte[] InBytes)
            {
                string StringOut = "";
                foreach (byte InByte in InBytes)
                {
                    StringOut = StringOut + String.Format("{0:X2} ", InByte);
                }
                return StringOut;
            }
            public static byte[] StringToByte(string InString)
            {
                string[] ByteStrings;
                ByteStrings = InString.Split(" ".ToCharArray());
                byte[] ByteOut;
                ByteOut = new byte[ByteStrings.Length];
                for (int i = 0; i < ByteStrings.Length; i++)
                {
                    ByteOut[i] = Convert.ToByte(ByteStrings[i], 16);
                }
                return ByteOut;
            }
        }

    }
}
