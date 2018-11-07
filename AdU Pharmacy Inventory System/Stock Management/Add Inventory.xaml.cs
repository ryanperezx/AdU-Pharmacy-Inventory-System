using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Data.Common;
namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for Add_Inventory.xaml
    /// </summary>
    public partial class Add_Inventory : Page
    {
        public Add_Inventory()
        {
            InitializeComponent();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInventName.Text))
            {
                MessageBox.Show("Inventory name field is empty!");
                txtInventName.Focus();
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbInventType.Text) || string.IsNullOrEmpty(txtInventName.Text) || string.IsNullOrEmpty(txtQty.Text) || string.IsNullOrEmpty(txtSize.Text) || string.IsNullOrEmpty(cmbUnit.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                string sMessageBoxText = "Are all fields correct?";
                string sCaption = "Add Inventory";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlCeConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCeCommand cmd = new SqlCeCommand("INSERT into InventoryStock (inventType, name, qty, size, unit, remarks) VALUES (@inventType, @inventName, @qty, @size, @unit, @remarks)", conn))
                        {
                            cmd.Parameters.AddWithValue("@inventType", cmbInventType.Text);
                            cmd.Parameters.AddWithValue("@inventName", txtInventName.Text);
                            cmd.Parameters.AddWithValue("@qty", txtQty.Text);
                            cmd.Parameters.AddWithValue("@size", txtSize.Text);
                            cmd.Parameters.AddWithValue("@unit", cmbUnit.Text);
                            cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Added Successfully");
                            }
                            catch (SqlCeException ex)
                            {
                                MessageBox.Show("Error! Log has been updated with the error.");
                            }
                        }
                        break;
                    case MessageBoxResult.No: break;

                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInventName.Text))
            {
                MessageBox.Show("Inventory name field is empty!");
                txtInventName.Focus();
            }
            else
            {
                string sMessageBoxText = "Do you want to delete this record?";
                string sCaption = "Delete Inventory record";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlCeConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using(SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(1) from inventoryStock where name = @inventName", conn))
                        {
                            cmd.Parameters.AddWithValue("@inventName", txtInventName.Text);
                            int count = (int)cmd.ExecuteScalar();
                            if(count > 0)
                            {

                            }
                            else
                            {


                            }
                        }
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void searchInName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInventName.Text))
            {
                MessageBox.Show("Inventory Name field is empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(1) from inventoryStock where name = @inventName", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", txtInventName.Text);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT * from inventoryStock where name = @inventName", conn))
                        {
                            cmd1.Parameters.AddWithValue("@inventName", txtInventName.Text);
                            using (DbDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();

                                    string inventType = Convert.ToString(reader.GetValue(1));

                                    int qtyIndex = reader.GetOrdinal("qty");
                                    int qty = Convert.ToInt32(reader.GetValue(qtyIndex));
 
                                    int sizeIndex = reader.GetOrdinal("size");
                                    string size = Convert.ToString(reader.GetValue(sizeIndex));

                                    int unitIndex = reader.GetOrdinal("unit");
                                    string unit = Convert.ToString(reader.GetValue(unitIndex));

                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));

                                    cmbInventType.Text = inventType;
                                    txtQty.Text = qty.ToString();
                                    txtSize.Text = size;
                                    cmbUnit.Text = unit;
                                    txtRemarks.Text = remarks;

                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Item does not exist!");
                        emptyFields();
                    }
                }
            }
        }

        private void emptyFields()
        {
            cmbInventType.SelectedIndex = -1;
            txtInventName.Text = null;
            txtQty.Text = null;
            txtSize.Text = null;
            cmbUnit.SelectedIndex = -1;
            txtRemarks.Text = null;
        }

        private void disableFields()
        {
            cmbInventType.IsEnabled = false;
            cmbUnit.IsEnabled = false;

            txtQty.IsReadOnly = true;
            txtSize.IsReadOnly = true;
            txtRemarks.IsReadOnly = true;
        }
        
        private void enableFields()
        {
            cmbInventType.IsEnabled = true;
            cmbUnit.IsEnabled = true;

            txtQty.IsReadOnly = false;
            txtSize.IsReadOnly = false;
            txtRemarks.IsReadOnly = false;
        }
    }
}
