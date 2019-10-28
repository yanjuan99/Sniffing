using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;

namespace Sniffing
{
    public class Network_card_data : INotifyPropertyChanged
    {

        string _name;

        string _description;

        string _lpv4;
        public string lpv4
        {
            get { return _lpv4; }
            set { _lpv4 = value; }
        }

        IntPtr _netmask;
        public IntPtr netmask
        {
            get { return _netmask; }
            set { _netmask = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public string Description
        {
            get { return _description; }
            set { 
                string tempStr = value.Substring(value.IndexOf("'") + "'".Length);
                tempStr = tempStr.Substring(0, tempStr.IndexOf("'"));
                _description = tempStr;
                OnPropertyChanged("Description"); }
        }

        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Ip_list_data : INotifyPropertyChanged
    {
        String _time;
        public String Time
        {
            get { return _time; }
            set { _time = value; OnPropertyChanged("Time"); }
        }

        string _source_ip;
        public string Source_ip
        {
            get { return _source_ip; }
            set { _source_ip = value; /*OnPropertyChanged("Source_ip"); */}
        }

        ushort _source_port;
        public ushort Source_port
        {
            get { return _source_port; }
            set { _source_port = value;/* OnPropertyChanged("Source_port");*/ }
        }

        string _dest_ip;
        public string Dest_ip
        {
            get { return _dest_ip; }
            set { _dest_ip = value;/* OnPropertyChanged("Dest_ip");*/ }
        }

        ushort _dest_port;
        public ushort Dest_port
        {
            get { return _dest_port; }
            set { _dest_port = value;/* OnPropertyChanged("Dest_port");*/ }
        }

        int _conut;
        public int Conut
        {
            get { return _conut; }
            set { _conut = value; OnPropertyChanged("Conut"); }
        }

        byte _ttl;
        public byte Ttl
        {
            get { return _ttl; }
            set { _ttl = value; OnPropertyChanged("Ttl"); }
        }

        string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged("Type"); }
        }

        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
