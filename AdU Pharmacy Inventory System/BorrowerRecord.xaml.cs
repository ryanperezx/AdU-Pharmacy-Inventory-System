using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlServerCe;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for BorrowerRecord.xaml
    /// </summary>
    public partial class BorrowerRecord : Page
    {
        public BorrowerRecord(string date, string studno, string subj, int grpID)
        { 
            InitializeComponent();
            txtDate.Text = date;
            txtStudNo.Text = studno;
            txtSubj.Text = subj;
            txtGrpID.Text = Convert.ToString(grpID);
            updateRecords();
        }

        private void updateRecords()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
        }
    }
}
