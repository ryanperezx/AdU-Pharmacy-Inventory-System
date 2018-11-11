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
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for ApparatusStockOut.xaml
    /// </summary>
    public partial class ApparatusStockOut : Page
    {
        int i = 1;
        public ApparatusStockOut()
        {
            InitializeComponent();
            fillInventory();
        }

        private void fillInventory()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT name from inventoryStock where inventType = 'Apparatus'",conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        cmbInventName.Items.Clear();
                        while (reader.Read())
                        {
                            string appaName = reader["name"].ToString();
                            cmbInventName.Items.Add(appaName);
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbInventName.Text) || string.IsNullOrEmpty(txtQty.Text) || string.IsNullOrEmpty(cmbManuf.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from inventoryStock where name = @inventName and manuf = @manuf", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    cmd.Parameters.AddWithValue("@manuf", cmbManuf.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        reader.Read();
                        int qtyIndex = reader.GetOrdinal("qty");
                        int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                        int manufIndex = reader.GetOrdinal("manuf");
                        string manuf = Convert.ToString(reader.GetValue(manufIndex));

                        int sizeIndex = reader.GetOrdinal("size");
                        string size = Convert.ToString(reader.GetValue(sizeIndex));

                        int unitIndex = reader.GetOrdinal("unit");
                        string unit = Convert.ToString(reader.GetValue(unitIndex));

                        int remarksIndex = reader.GetOrdinal("remarks");
                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));

                        int reqQty = Convert.ToInt32(txtQty.Text);
                        if (reqQty > qty)
                        {
                            MessageBox.Show("Requested quantity cannot be greater than the available quantity!");
                            return;
                        }
                        else
                        {
                            lvAppaStockOut.Items.Add(new LVApparatusStockOut {
                                i = i,
                                inventName = cmbInventName.Text,
                                qty = reqQty,
                                manuf = manuf,
                                size = size,
                                unit = unit,
                                remarks = remarks
                            });
                            i++;
                            emptyFields();
                        }
                        
                    }
                }
            }
        }

        private void btnStockOut_Click(object sender, RoutedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            if(lvAppaStockOut.Items.Count == 0)
            {
                MessageBox.Show("There are no apparatus(es) to be stock out");
            }
            else
            {
                foreach (LVApparatusStockOut row in lvAppaStockOut.Items) {
                    using (SqlCeCommand cmd = new SqlCeCommand("UPDATE InventoryStock set qty = qty - @qty where name = @inventType"))
                    {
                        cmd.Parameters.AddWithValue("@qty",row.qty);
                        cmd.Parameters.AddWithValue("@inventType",row.inventName);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Stock Out Successfully");
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show("Error! Log has been updated with the error");
                        }
                    }
                }
            }
        }



        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void emptyFields()
        {
            cmbInventName.SelectedIndex = -1;
            cmbManuf.SelectedIndex = -1;
            txtQty.Text = null;
        }

        private void txtInventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            fillManufacturer();
        }

        private void fillManufacturer()
        {
            if (!string.IsNullOrEmpty(cmbInventName.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                cmbManuf.Items.Clear();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT manuf from inventoryStock where name = @inventName", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int manufIndex = reader.GetOrdinal("manuf");
                                string manuf = Convert.ToString(reader.GetValue(manufIndex));
                                cmbManuf.Items.Add(manuf);
                            }
                        }
                    }
                }
            }
        }
    }

}
