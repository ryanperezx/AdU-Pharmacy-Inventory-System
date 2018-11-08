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
using System.Data.SqlClient;
using System.Data.Common;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for ApparatusStockOut.xaml
    /// </summary>
    public partial class ApparatusStockOut : Page
    {
        public ApparatusStockOut()
        {
            InitializeComponent();
            fillInventory();
        }

        private void fillInventory()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT name from inventoryStock",conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        cmbAppaName.Items.Clear();
                        while (reader.Read())
                        {
                            string appaName = reader["name"].ToString();
                            cmbAppaName.Items.Add(appaName);
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStockOut_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
