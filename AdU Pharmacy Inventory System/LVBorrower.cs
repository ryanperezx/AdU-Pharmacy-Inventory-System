using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdU_Pharmacy_Inventory_System
{
    class LVBorrower
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

        public int qty
        {
            get;
            set;
        }


        public static ObservableCollection<LVBorrower> getList()
        {
            var list = new ObservableCollection<LVBorrower>();
            return list;
        }
    }
}
