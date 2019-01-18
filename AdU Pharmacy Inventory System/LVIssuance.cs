using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdU_Pharmacy_Inventory_System
{
    class LVIssuance
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

        public int qty
        {
            get;
            set;
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

        public static ObservableCollection<LVBorrower> getList() //updates observablecollection
        {
            var list = new ObservableCollection<LVBorrower>();
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

        public bool breakage
        {
            get;
            set;
        }


    }
}
