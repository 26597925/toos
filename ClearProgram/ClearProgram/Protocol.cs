using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Windows.Forms;

namespace ClearProgram
{
    class Protocol
    {
        public enum HardwareEnum
        {
            Win32_SerialPort, // 串口
        }

        static int len = 137;
        static byte[] start = { 0x55, 0x00, 0x84,0x63, 0x07, 0x00, 0x80, 0x00 };//头信息
        static String connect = "AT:connect ok!#";
        static String version = "AT+uart:ver,?#";
        static String imei = "AT+uart:imei,{0}#";
        static String server = "AT+uart:ip_url,{0},port,{1},url,{2}#";
        static String number = "AT+uart:center_number,{0},slave_number,{1}#";
        static String qzone = "AT+uart:language,{0},zone,{1}#";
        static String sos = "AT+uart:sos1,{0},sos2,{1},sos3,{2}#";
        static String monitor = "AT+uart:monitor,{0}#";
        static String upload = "AT+uart:upload,{0}#";
        static String factoryTest = "AT+uart:factory_test#";

        public static byte[] connectDevice()
        {
            return buildCommand(connect);
        }

        public static byte[] lookVer() {
            return buildCommand(version);
        }

        public static byte[] readImei() {
            String command = String.Format(imei, "?");
            return buildCommand(command);
           
        }

        public static byte[] writeImei(String data) {
            String command = String.Format(imei, data);
            return buildCommand(command);
        }

        public static byte[] readServer() {
            String command = String.Format(server, "?", "?", "?");
            return buildCommand(command);
        }

        public static byte[] writeServer(String ip, String port, String url)
        {
            String command = String.Format(server, ip, port, url);
            return buildCommand(command);
        }

        public static byte[] readNumber() {
            String command = String.Format(number, "?", "?");
            return buildCommand(command);
        }

        public static byte[] writeNumber(String center_number, String slave_number)
        {
            String command = String.Format(number, center_number, slave_number);
            return buildCommand(command);
        }

        public static byte[] readQzone() {
            String command = String.Format(qzone, "?", "?");
            return buildCommand(command);
        }

        public static byte[] writeQzone(String language, String zone)
        {
            String command = String.Format(qzone, language, zone);
            return buildCommand(command);
        }

        public static byte[] readSos() {
            String command = String.Format(sos, "?", "?", "?");
            return buildCommand(command);
        }

        public static byte[] writeSos(String sos1, String sos2, String sos3)
        {
            String command = String.Format(sos, sos1, sos2, sos3);
            return buildCommand(command);
        }

        public static byte[] readMonitor() {
            String command = String.Format(monitor, "?");
            return buildCommand(command);
        }

        public static byte[] writeMonitor(String number)
        {
            String command = String.Format(monitor, number);
            return buildCommand(command);
        }

        public static byte[] readUpload() {
            String command = String.Format(upload, "?");
            return buildCommand(command);
        }

        public static byte[] writeUpload(String data)
        {
            String command = String.Format(upload, data);
            return buildCommand(command);
        }

        public static byte[] testDevice() {
            return buildCommand(factoryTest);
        }

        public static int getDataLen(){
            return len;
        }

        public static bool isPort(String name) {
            string[] ss = Protocol.getPort();
            return (from c in ss where c.Contains(name) select 1).ToList().Count > 0;
        }

        public static byte[] sendCommand(String data)
        {
            String command = "AT+uart:" + data;
            return buildCommand(command);
        }

        private static string[] getPort()
        {
            return MulGetHardwareInfo(HardwareEnum.Win32_SerialPort, "Name");
        }

        private static byte[] buildCommand(String command) {
            byte[] soure = new byte[len];
            byte[] content = Encoding.Default.GetBytes(command);
            soure[len - 1] = verification(content);
            int endLen = start.Length - 1;
            copyArray(soure, start, 0);
            copyArray(soure, content, start.Length);
            return soure;
        }

        private static byte verification(byte[] context) {
            byte val = 0x35;
            for (int i = 0, len = context.Length; i < len; i++ )
            {
                val ^= context[i];
            }
            return val;
        }

        private static void copyArray(byte[] target, byte[] source, int start)
        {
            int len = 0;
            if ((target.Length - start) > source.Length)
            {
                len = source.Length;
            }
            else {
                len = target.Length;
            }
            for (int i = start; i < (len + start); i++)
            {
                target[i] = source[i - start];
            }
        }

        private static string[] MulGetHardwareInfo(HardwareEnum hardType, string propKey)
        {

            List<string> strs = new List<string>();
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties[propKey].Value.ToString().Contains("COM"))
                        {
                            strs.Add(hardInfo.Properties[propKey].Value.ToString());
                        }

                    }
                    searcher.Dispose();
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            { strs = null; }
        }
    }
}
