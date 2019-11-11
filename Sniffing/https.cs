using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Sniffing
{
    internal class Https
    {
        public static bool th_b = false;

        public static void Listener(TcpListener tcpListener, System.Windows.Controls.TextBox textbox)
        {
            while (th_b)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    NetworkStream networkStream = tcpClient.GetStream();

                    byte[] buffer = new byte[0x1000];
                    networkStream.Read(buffer, 0, 0x1000);
                    networkStream.WriteByte(1);
                    networkStream.Close();
                    tcpClient.Close();

                    string receiveString = Encoding.Default.GetString(buffer).Trim('\0');

                    textbox.Dispatcher.Invoke(() =>
                    {
                        textbox.Text += receiveString + "\n\n";
                    });

                    // Console.WriteLine(receiveString);
                }
                catch (Exception e)
                {
                    //throw e;
                }
            }
        }

        private static TcpListener tcpListener = null;

        public static void Start(int port, System.Windows.Controls.TextBox textbox)
        {
            try
            {
                IPAddress localAddres = IPAddress.Parse("0.0.0.0");
                tcpListener = new TcpListener(localAddres, port);
                tcpListener.Start();

                th_b = true;
                Thread threadHand1 = new Thread(delegate () { Listener(tcpListener, textbox); });
                threadHand1.Start();

                textbox.Dispatcher.Invoke(() =>
                {
                    textbox.Text += "服务器启动完成 \n";
                });
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.Message.ToString(), "信息", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        public static void Stop(System.Windows.Controls.TextBox textbox)
        {
            th_b = false;
            tcpListener.Stop();
            textbox.Dispatcher.Invoke(() =>
            {
                textbox.Text += "服务器停止完成 \n";
            });
        }
    }
}