using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Xml;

namespace Sniffing
{
    public partial class MainWindow : MetroWindow
    {
        public ObservableCollection<Network_card_data> networl_list = new ObservableCollection<Network_card_data>() { };
        public ObservableCollection<Ip_list_data> ip_list = new ObservableCollection<Ip_list_data>() { };
        private string lpv4;
        private bool thread_br;

        public MainWindow()
        {
            InitializeComponent();

            myCombobox.ItemsSource = networl_list;
            ip_list_name.ItemsSource = ip_list;

            //快速搜索功能
            SearchPanel.Install(textEditor.TextArea);
            //设置语法规则
            string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".Lua.xshd";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream s = assembly.GetManifestResourceStream(name))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    var xshd = HighlightingLoader.LoadXshd(reader);
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
                }
            }

            InitCombobox();
        }

        private bool InitCombobox()
        {
            var tool = new clrtool.mytool();
            IntPtr pint = tool.alldevs_lode();

            var tmp = new pcap_if_c();
            tmp.UnmanagedPtr2ManagedStru(pint);

            for (; ; tmp.UnmanagedPtr2ManagedStru(tmp.next))
            {
                Network_card_data _Card_Data = new Network_card_data() { netmask = tool.getnetmask(tmp.addresses), lpv4 = tool.getlpv4(tmp.addresses), Name = tmp.cname, Description = tmp.cdescription };
                networl_list.Add(_Card_Data);
                // Console.WriteLine(tmp.cname + tmp.cdescription);
                if (tmp.next.ToInt64() == 0)
                {
                    break;
                }
            }
            if (networl_list.Count > 0)
            {
                tool.dev_free(pint);
            }

            return networl_list.Count > 0 ? true : false;
        }

        private void Onclick_stars(object sender, RoutedEventArgs e)
        {
            if (myCombobox.SelectedIndex == -1)
            {
                return;
            }
            Network_card_data _Card_Data = (Network_card_data)myCombobox.SelectedItem;
            Console.WriteLine(_Card_Data.Name + _Card_Data.Description);

            var tool = new clrtool.mytool();
            var ahandle = tool.open(_Card_Data.Name, _Card_Data.netmask, filter_text.Text);
            lpv4 = _Card_Data.lpv4;
            if (ahandle)
            {
                thread_br = true;

                Thread threadHand1 = new Thread(delegate () { msegloop(tool); });
                threadHand1.Start();
            }
        }

        unsafe private string Toarray(IntPtr Ptr, int len)
        {
            string result = "";
            for (int i = 0; i < len; i++)
            {
                Byte b = ((Byte*)Ptr.ToPointer())[i];
                result += b.ToString("X2");
                result += " ";
                if ((i + 1) % 16 == 0)
                {
                    result += "\r";
                }
            }
            return result;
        }

        private void OutputAction(pcap_data data)
        {
            var inst = new Ip_list_data
            {
                Source_ip = data.source_ip,
                Source_port = data.source_port,
                Dest_ip = data.dest_ip,
                Dest_port = data.dest_port,
                Time = data.time,
                Conut = 1,
                Ttl = data.ttl,
                Type = (data.type == 6) ? "Tcp" : "Udp"
            };

            string destip = lpv4 == data.source_ip ? data.dest_ip : data.source_ip;
            ushort destport = lpv4 == data.source_ip ? data.dest_port : data.source_port;

            string code = Toarray(data.p_data, data.len);
            string path = Directory.GetCurrentDirectory() + "\\cache";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.AppendAllText(path + "\\" + destip + "." + destport.ToString() + ".txt",
                data.time + " len " + data.len.ToString() + " " +
                (data.source_ip == lpv4 ? "Send" : "Recv") + " " +
                data.source_ip + ":" + data.source_port.ToString() + "-->" +
                data.dest_ip + ":" + data.dest_port.ToString() + "\r" + code + "\n");

            ip_list_name.Dispatcher.Invoke(() =>
            {
                try
                {
                    foreach (var item in ip_list)
                    {
                        if (item.Source_ip == destip || item.Dest_ip == destip)
                        {
                            if (item.Source_port == destport || item.Dest_port == destport)
                            {
                                item.Conut++;
                                item.Time = inst.Time;
                                item.Ttl = inst.Ttl;
                                return;
                            }
                        }
                    }
                    ip_list.Add(inst);
                }
                catch (Exception e)
                {
                    throw e;
                }
            });
        }

        private void msegloop(object param)
        {
            clrtool.mytool tool = (clrtool.mytool)param;
            while (thread_br)
            {
                pcap_data data = tool.read();
                if (data.res == 0)
                {
                    continue;
                }
                if (data.res == -1)
                {
                    return;
                }
                OutputAction(data);
                // Console.WriteLine(data.time + " " + data.len.ToString() +" "+data.source_ip + ":" + data.source_port.ToString()+"-->"+data.dest_ip+":"+data.dest_port.ToString());
            }
        }

        private void Loadtxt(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException();
            }
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException();
            }
            using (FileStream stream = File.OpenRead(filename))
            {
                StreamReader sr = new StreamReader(stream, Encoding.Default);
                var read_stream = new MemoryStream(Encoding.UTF8.GetBytes(sr.ReadToEnd()));
                textEditor.Dispatcher.Invoke(() =>
                {
                    textEditor.Load(read_stream);
                });
            }
        }

        private void ListView_Doubleclick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ip_list_name.SelectedIndex == -1) return;

            var item = (Ip_list_data)ip_list_name.SelectedItem;

            string destip = lpv4 == item.Source_ip ? item.Dest_ip : item.Source_ip;
            ushort destport = lpv4 == item.Source_ip ? item.Dest_port : item.Source_port;
            string path = Directory.GetCurrentDirectory() + "\\cache" + "\\" + destip + "." + destport.ToString() + ".txt";

            Thread threadHand1 = new Thread(delegate () { Loadtxt(path); });
            threadHand1.Start();

            Console.WriteLine(item.Time + " " + item.Source_ip + ":" + item.Source_port.ToString() + "-->" + item.Dest_ip + ":" + item.Dest_port.ToString());
        }

        private void Onclick_stop(object sender, RoutedEventArgs e)
        {
            thread_br = false;
        }

        private void Menu_Getsize(object sender, RoutedEventArgs e)
        {
            string str = Regex.Replace(textEditor.SelectedText, @"\s", "");
            if (str.Length != 0)
            {
                if (str.Length % 2 == 0)
                {
                    MessageBox.Show((str.Length / 2).ToString(), "信息", MessageBoxButton.OK, MessageBoxImage.None);
                    //ShowMessage((str.Length / 2).ToString());
                }
                else
                {
                    MessageBox.Show("请选择完整字节", "信息", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Onclick_clear(object sender, RoutedEventArgs e)
        {
            ip_list.Clear();
            string file = Directory.GetCurrentDirectory() + "\\cache";

            if (Directory.Exists(file))
            {
                try
                {
                    Directory.Delete(file, true);
                }
                catch (Exception ep)
                {
                    Console.WriteLine(ep);
                }
            }
        }

        private void window_close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            thread_br = false;
            string file = Directory.GetCurrentDirectory() + "\\cache";

            if (Directory.Exists(file))
            {
                Directory.Delete(file, true);
            }
            if (Https.th_b)
            {
                Https.Stop(http_print);
            }
        }

        private void http_start(object sender, RoutedEventArgs e)
        {
            if (!Https.th_b)
            {
                Https.Start(int.Parse(servser_port.Text), http_print);
            }
        }

        private void http_stop(object sender, RoutedEventArgs e)
        {
            if (Https.th_b)
            {
                Https.Stop(http_print);
            }
        }

        private void http_clear(object sender, RoutedEventArgs e)
        {
            http_print.Text = "";
        }

        private void Menu_strformat(object sender, RoutedEventArgs e)
        {
            List<string> substr = new List<string>();

            string str = Regex.Replace(textEditor.SelectedText, @"\s", "");
            int len = str.Length;
            if (len != 0 && len % 2 == 0)
            {
                int index = 0;
                do
                {
                    substr.Add(str.Substring(index, 2));
                    index += 2;
                } while (len != index);
                str = "";
                for (int i = 0; i < substr.Count; i++)
                {
                    str += substr[i] + " ";
                    if ((i + 1) % 16 == 0)
                    {
                        str += "\r";
                    }
                }
                textEditor.SelectedText = str;
            }
        }

        private async void ShowMessage(string msg)
        {
            await this.ShowMessageAsync("", msg);
        }

        private void Btn_click_e(object sender, RoutedEventArgs e)
        {
            byte[] tmp_key = QQTea.Strtobyte(Tea_key.Text);
            byte[] tmp_data = QQTea.Strtobyte(Tea_in.Text);
            if (tmp_data.Length > 0 && tmp_key.Length > 0)
            {
                byte[] redata = QQTea.Encrypt(tmp_data, 0, tmp_data.Length, tmp_key);
                if (redata != null)
                {
                    Tea_out.Text = QQTea.Bytetostr(redata);
                }
            }
        }

        private void Btn_click_d(object sender, RoutedEventArgs e)
        {
            byte[] tmp_key = QQTea.Strtobyte(Tea_key.Text);
            byte[] tmp_data = QQTea.Strtobyte(Tea_in.Text);
            if (tmp_data.Length > 0 && tmp_key.Length > 0)
            {
                byte[] redata = QQTea.Decrypt(tmp_data, 0, tmp_data.Length, tmp_key);
                if (redata != null)
                {
                    Tea_out.Text = QQTea.Bytetostr(redata);
                }
            }
        }

        private void Menu_toint(object sender, RoutedEventArgs e)
        {
            if (textEditor.SelectedText.Length > 0)
            {
                try
                {
                    string str = Regex.Replace(textEditor.SelectedText, @"\s", "");
                    string cnt = Convert.ToInt32(str, 16).ToString();
                    MessageBox.Show(cnt, "信息", MessageBoxButton.OK, MessageBoxImage.None);
                }
                catch (Exception)
                {
                }
            }
        }

        private void Menu_Tea_d0(object sender, RoutedEventArgs e)
        {
            byte[] tmp_key = new byte[16];
            byte[] tmp_data = QQTea.Strtobyte(textEditor.SelectedText);
            if (tmp_data.Length > 0 && tmp_key.Length > 0)
            {
                byte[] redata = QQTea.Decrypt(tmp_data, 0, tmp_data.Length, tmp_key);
                if (redata != null)
                {
                    textEditor.SelectedText += "\nTea解密 key " + QQTea.Bytetostr(tmp_key) + "\n" + QQTea.Bytetostr(redata) + "\nTea解密end";
                }
            }
        }

        private void Menu_C_tostr(object sender, RoutedEventArgs e)
        {
            if (textEditor.SelectedText.Length > 0)
            {
                textEditor.SelectedText += "  //" + Encoding.Default.GetString(QQTea.Strtobyte(textEditor.SelectedText));
            }
        }
    }
}