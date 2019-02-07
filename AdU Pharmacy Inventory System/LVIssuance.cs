using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AdU_Pharmacy_Inventory_System
{
    class LVIssuance : INotifyPropertyChanged
    {
        public int i
        {
            get;
            set;
        }
        public string prodCode
        {
            get;
            set;
        }
        public string inventName
        {
            get;
            set;
        }

        public List<string> manufList
        {
            get;
            set;
        }

        private string _manuf;
        public string manuf
        {
            get { return _manuf; }
            set { _manuf = value; OnPropertyChanged("manuf"); }
        }

        public string size
        {
            get;
            set;
        }

        public string unit
        {
            get;
            set;
        }

        private int _qty;

        public int qty
        {
            get { return _qty; }
            set { _qty = value; OnPropertyChanged("qty"); }
        }

        public string returnChk
        {
            get;
            set;
        }

        public Nullable<int> breakages
        {
            get;
            set;
        }

        public string amountCharge
        {
            get;
            set;
        }

        public static ObservableCollection<LVIssuance> getList() //updates observablecollection
        {
            var list = new ObservableCollection<LVIssuance>();
            return list;
        }

        public string studentNo
        {
            get;
            set;
        }

        public string fullName
        {
            get;
            set;
        }

        public string issuedDate
        {
            get;
            set;
        }

        public string issuedBy
        {
            get;
            set;
        }

        public string lockNo
        {
            get;
            set;
        }

        public string sect
        {
            get;
            set;
        }

        public string sched
        {
            get;
            set;
        }

        private bool _breakage;
        public bool breakage
        {
            get { return _breakage; }
            set { _breakage = value; OnPropertyChanged("breakage"); }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
