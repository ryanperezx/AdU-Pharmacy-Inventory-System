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
using System.Text.RegularExpressions;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Data.Common;


namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for Stock_In.xaml
    /// </summary>
    public partial class Stock_In : Page
    {
        public Stock_In()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(cmbInventName.Text) || string.IsNullOrEmpty(cmbInventType.Text) || string.IsNullOrEmpty(dateStockIn.Text) || string.IsNullOrEmpty(cmbManuf.Text) || string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("One or more field is empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT count(1) from inventoryStock where name = @inventName", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE inventoryStock set qty = qty + @qty where name = @inventName and manuf = @manuf", conn))
                        {
                            cmd1.Parameters.AddWithValue("@qty", txtQty.Text);
                            cmd1.Parameters.AddWithValue("@inventName", txtInventName.Text);
                            cmd1.Parameters.AddWithValue("@manuf", cmbManuf.Text);

                            try
                            {
                                cmd1.ExecuteNonQuery();
                                MessageBox.Show("Added Successfully");
                                emptyFields();
                            }
                            catch (SqlCeException ex)
                            {
                                MessageBox.Show("Error! Log has been updated with the error.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Item does not exist!");
                    }
                }
            }
        }


        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtInventType_TextChanged(object sender, TextChangedEventArgs e)
        {
            fillInventoryList();
        }

        private void fillInventoryList()
        {
            if (!string.IsNullOrEmpty(txtInventType.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                cmbInventName.Items.Clear();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT name from inventoryStock where inventType = @inventType", conn))
                {
                    cmd.Parameters.AddWithValue("@inventType", txtInventType.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int inventoryNameIndex = reader.GetOrdinal("name");
                                string inventoryName = Convert.ToString(reader.GetValue(inventoryNameIndex));
                                cmbInventName.Items.Add(inventoryName);
                            }
                        }
                    }
                }
            }
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

        private void emptyFields()
        {
            dateStockIn.Text = null;
            cmbInventName.SelectedIndex = -1;
            cmbInventType.SelectedIndex = -1;
            cmbManuf.SelectedIndex = -1;
            txtQty.Text = null;
        }
    }
}
