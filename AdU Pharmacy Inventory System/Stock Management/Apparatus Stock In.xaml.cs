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
            fillInventoryList();
            dateStockIn.Text = DateTime.Now.ToString("dd MMMM yyyy");
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(cmbInventName.Text) || string.IsNullOrEmpty(dateStockIn.Text) || string.IsNullOrEmpty(cmbManuf.Text) || string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("One or more field is empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT count(1) from ApparatusInventory where name = @inventName", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE ApparatusInventory set qty = qty + @qty where name = @inventName and manuf = @manuf and size = @size", conn))
                        {
                            cmd1.Parameters.AddWithValue("@qty", txtQty.Text);
                            cmd1.Parameters.AddWithValue("@inventName", txtInventName.Text);
                            cmd1.Parameters.AddWithValue("@manuf", cmbManuf.Text);
                            cmd1.Parameters.AddWithValue("@size", cmbSize.Text);
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



        private void fillInventoryList()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT name from ApparatusInventory", conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        cmbInventName.Items.Clear();
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
        private void fillManufacturer()
        {
            if (!string.IsNullOrEmpty(cmbSize.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT manuf from ApparatusInventory where name = @inventName and size = @size", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    cmd.Parameters.AddWithValue("@size", cmbSize.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbManuf.Items.Clear();
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
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                cmbManuf.Items.Clear();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT manuf from ApparatusInventory where name = @inventName", conn))
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
        private void fillSize()
        {
            if (!string.IsNullOrEmpty(cmbInventName.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT size from ApparatusInventory where name = @inventName and size IS NOT NULL", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbSize.Items.Clear();
                            while (reader.Read())
                            {
                                int sizeIndex = reader.GetOrdinal("size");
                                string size = Convert.ToString(reader.GetValue(sizeIndex));
                                cmbSize.Items.Add(size);
                            }
                        }
                    }
                }
            }
        }

        private void txtInventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmbSize.Items.Clear();
            cmbManuf.Items.Clear();
            fillSize();
            if(cmbSize.Items.Count == 0)
            {
                fillManufacturer();
            }
        }
        private void txtSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            fillManufacturer();
        }
        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void emptyFields()
        {
            dateStockIn.Text = null;
            cmbInventName.SelectedIndex = -1;
            cmbSize.SelectedIndex = -1;
            cmbSize.Items.Clear();
            cmbManuf.SelectedIndex = -1;
            cmbManuf.Items.Clear();
            txtQty.Text = null;
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
            fillInventoryList();
        }
    }
}
