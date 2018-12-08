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
using System.Collections.ObjectModel;


namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for Apparatus_Inventory_Summary.xaml
    /// </summary>
    public partial class Apparatus_Inventory_Summary : Page
    {
        int i = 1;
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<LVApparatusStockOut> summary = new ObservableCollection<LVApparatusStockOut>();
        public Apparatus_Inventory_Summary()
        {
            InitializeComponent();
            date.Text = DateTime.Now.ToString("dd MMMM yyyy");
            view.Source = summary;
            lvInvent.DataContext = view;
            updateListView();
        }

        private void updateListView()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from ApparatusInventory", conn))
            {
                lvInvent.Items.Clear();
                using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    while (reader.Read())
                    {
                        int prodCodeIndex = reader.GetOrdinal("prodCode");
                        string prodCode = Convert.ToString(reader.GetValue(prodCodeIndex));

                        int inventNameIndex = reader.GetOrdinal("name");
                        string inventName = Convert.ToString(reader.GetValue(inventNameIndex));

                        int manufIndex = reader.GetOrdinal("manuf");
                        string manuf = Convert.ToString(reader.GetValue(manufIndex));

                        int qtyIndex = reader.GetOrdinal("qty");
                        int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                        int sizeIndex = reader.GetOrdinal("size");
                        string size = Convert.ToString(reader.GetValue(sizeIndex));

                        int unitIndex = reader.GetOrdinal("unit");
                        string unit = Convert.ToString(reader.GetValue(unitIndex));

                        int remarksIndex = reader.GetOrdinal("remarks");
                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                        summary.Add(new LVApparatusStockOut
                        {
                            i = i,
                            prodCode = prodCode,
                            inventName = inventName,
                            manuf = manuf,
                            qty = qty,
                            size = size,
                            unit = unit,
                            remarks = remarks
                        });
                        if (qty < 3)
                        {
                            MessageBox.Show("Product: " + inventName + " quantity is at critical level! Restock as soon as possible");
                        }
                        i++;
                    }
                }
            }
        }
    }
}
