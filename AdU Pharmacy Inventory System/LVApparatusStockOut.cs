using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AdU_Pharmacy_Inventory_System
{
    class LVApparatusStockOut : INotifyPropertyChanged
    {
        public int i
        {
            get;
            set;
        }

        public string inventName
        {
            get;
            set;
        }

        public string manuf
        {
            get;
            set;
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

        public string remarks
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
