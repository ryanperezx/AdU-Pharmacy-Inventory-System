using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace AdU_Pharmacy_Inventory_System
{
    class LVBorrower : INotifyPropertyChanged
    {
        public string dateReq
        {
            get;
            set;
        }

        public string dateExp
        {
            get;
            set;
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

        public string subj
        {
            get;
            set;
        }

        public int grpID
        {
            get;
            set;
        }

        public string experiment
        {
            get;
            set;
        }

        public string lockNo
        {
            get;
            set;
        }

        public string prodName
        {
            get;
            set;
        }

        public string manuf
        {
            get;
            set;
        }

        public int _qty;
        public int qty
        {
            get { return _qty; }
            set { _qty = value; OnPropertyChanged("qty"); }
        }

        public string size
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

        public static ObservableCollection<LVBorrower> getList() //updates observablecollection
        {
            var list = new ObservableCollection<LVBorrower>();
            return list;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
