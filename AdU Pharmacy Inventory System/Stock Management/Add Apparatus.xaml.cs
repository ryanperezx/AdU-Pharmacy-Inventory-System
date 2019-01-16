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
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for Add_Inventory.xaml
    /// </summary>
    public partial class Add_Inventory : Page
    {
        int process = 0;
        int i = 1;
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<LVOutstanding> summary = new ObservableCollection<LVOutstanding>();
        public Add_Inventory()
        {
            InitializeComponent();
            date.Text = DateTime.Now.ToString("dd MMMM yyyy");
            view.Source = summary;
            lvInvent.DataContext = view;
            updateListView();
        }

        private void searchInName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtProdCode.Text))
            {
                MessageBox.Show("Inventory Name field is empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(1) from ApparatusInventory where prodCode = @prodCode", conn))
                {
                    cmd.Parameters.AddWithValue("@prodCode", txtProdCode.Text);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT * from ApparatusInventory where prodCode = @prodCode", conn))
                        {
                            cmd1.Parameters.AddWithValue("@prodCode", txtProdCode.Text);
                            using (DbDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {

                                        int prodCodeIndex = reader.GetOrdinal("prodCode");
                                        string prodCode = Convert.ToString(reader.GetValue(prodCodeIndex));

                                        int appaNameIndex = reader.GetOrdinal("name");
                                        string appaName = Convert.ToString(reader.GetValue(appaNameIndex));

                                        int manufIndex = reader.GetOrdinal("manuf");
                                        string manuf = Convert.ToString(reader.GetValue(manufIndex));

                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                                        int sizeIndex = reader.GetOrdinal("size");
                                        string size = Convert.ToString(reader.GetValue(sizeIndex));

                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));

                                        var unit = Regex.Replace(size, @"[\d-]", string.Empty);
                                        string numberOnly = Regex.Replace(size, "[^0-9.]", "");

                                        txtProdCode.Text = prodCode;
                                        txtInventName.Text = appaName;
                                        txtManuf.Text = manuf;
                                        txtQty.Text = qty.ToString();
                                        txtSize.Text = numberOnly;
                                        cmbUnit.Text = unit;
                                        txtRemarks.Text = remarks;
                                        disableFields();
                                        txtSize.IsEnabled = true;
                                        txtRemarks.IsEnabled = true;
                                        cmbUnit.IsEnabled = true;
                                        process = 1;
                                    }
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
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInventName.Text))
            {
                MessageBox.Show("Inventory name field is empty!");
                txtInventName.Focus();
            }
            else
            {
                if (process == 1)
                {
                    string sMessageBoxText = "Do you want to update the record?";
                    string sCaption = "Edit Record";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            SqlCeConnection conn = DBUtils.GetDBConnection();
                            conn.Open();
                            using (SqlCeCommand cmd = new SqlCeCommand("UPDATE ApparatusInventory set size = @size, unit = @unit, remarks = @remarks where name = @inventName and manuf = @manuf and prodCode = @prodCode", conn))
                            {
                                cmd.Parameters.AddWithValue("@size", txtSize.Text);
                                cmd.Parameters.AddWithValue("@unit", cmbUnit.Text);
                                cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);
                                cmd.Parameters.AddWithValue("@inventName", txtInventName.Text);
                                cmd.Parameters.AddWithValue("@manuf", txtManuf.Text);
                                cmd.Parameters.AddWithValue("@prodCode", txtProdCode.Text);

                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Updated Successfully");
                                    emptyFields();
                                    enableFields();

                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("Error! Log has been updated with the error.");
                                }

                            }
                            break;
                        case MessageBoxResult.No: break;
                    }
                }
                else
                {
                    MessageBox.Show("Please search the inventory item first before editing any record!");
                }

            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInventName.Text) || string.IsNullOrEmpty(txtManuf.Text) || string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (process == 1)
            {
                MessageBox.Show("Search only works in junction with Edit and Delete Record, please press the reset button if you're trying to add new inventory record!");
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
                        using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT COUNT(1) from ApparatusInventory where prodCode = @prodCode", conn))
                        {
                            cmd1.Parameters.AddWithValue("@prodCode", txtProdCode.Text);
                            int count = (int)cmd1.ExecuteScalar();
                            if (count > 0)
                            {
                                MessageBox.Show("Product code already exists! Please choose another and try again.");
                            }
                            else
                            {
                                string unit = "";
                                if(cmbUnit.Text == "N/A")
                                {
                                    unit = null;
                                }
                                else
                                {
                                    unit = cmbUnit.Text;
                                }
                                using (SqlCeCommand cmd = new SqlCeCommand("INSERT into ApparatusInventory (prodCode, name, manuf, qty, size, remarks) VALUES (@prodCode, @inventName, @manuf, @qty, @size, @remarks)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@prodCode", txtProdCode.Text);
                                    cmd.Parameters.AddWithValue("@inventName", txtInventName.Text);
                                    cmd.Parameters.AddWithValue("@manuf", txtManuf.Text);
                                    cmd.Parameters.AddWithValue("@qty", txtQty.Text);
                                    if (!string.IsNullOrEmpty(txtSize.Text))
                                    {
                                        cmd.Parameters.AddWithValue("@size", txtSize.Text + unit);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@size", DBNull.Value);

                                    }
                                    cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text);

                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                        MessageBox.Show("Added Successfully");
                                        emptyFields();
                                    }
                                    catch (SqlCeException ex)
                                    {
                                        MessageBox.Show("Error! Log has been updated with the error. " + ex);
                                    }
                                }
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
            else if (string.IsNullOrEmpty(txtManuf.Text))
            {
                MessageBox.Show("Manufacturer field is empty!");
                txtManuf.Focus();
            }
            else
            {
                if (process == 1)
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
                            using (SqlCeCommand cmd = new SqlCeCommand("INSERT into ArchiveApparatusInventory (prodCode, name, qty, size, remarks, manuf) SELECT prodCode, name, qty, size, remarks, manuf from ApparatusInventory where name = @inventName and manuf = @manuf", conn))
                            {
                                cmd.Parameters.AddWithValue("@inventName", txtInventName.Text);
                                cmd.Parameters.AddWithValue("@manuf", txtManuf.Text);
                                int result = cmd.ExecuteNonQuery();
                                if (result > 0)
                                {
                                    using (SqlCeCommand command = new SqlCeCommand("DELETE from ApparatusInventory where name = @inventName and manuf = @manuf", conn))
                                    {
                                        command.Parameters.AddWithValue("@inventName", txtInventName.Text);
                                        command.Parameters.AddWithValue("@manuf", txtManuf.Text);

                                        int query = command.ExecuteNonQuery();
                                        MessageBox.Show("Item has been deleted!");
                                        emptyFields();
                                        enableFields();
                                        process = 0;

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Item does not exist!");
                                }
                            }
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Please search the inventory item first before deleting any record!");
                }
            }
        }



        private void emptyFields()
        {
            txtProdCode.Text = null;
            txtManuf.Text = null;
            txtInventName.Text = null;
            txtQty.Text = null;
            txtSize.Text = null;
            cmbUnit.SelectedIndex = -1;
            txtRemarks.Text = null;
            i = 1;
        }
        private void disableFields()
        {
            txtProdCode.IsEnabled = false;
            cmbUnit.IsEnabled = false;
            txtManuf.IsEnabled = false;

            txtQty.IsEnabled = false;
            txtSize.IsEnabled = false;
            txtRemarks.IsEnabled = false;
        }
        private void enableFields()
        {
            txtProdCode.IsEnabled = true;
            cmbUnit.IsEnabled = true;
            txtManuf.IsEnabled = true;

            txtQty.IsEnabled = true;
            txtSize.IsEnabled = true;
            txtRemarks.IsEnabled = true;
        }
        private void updateListView()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from ApparatusInventory", conn))
            {
                summary.Clear();
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

                        int remarksIndex = reader.GetOrdinal("remarks");
                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                        summary.Add(new LVOutstanding
                        {
                            i = i,
                            prodCode = prodCode,
                            inventName = inventName,
                            manuf = manuf,
                            qty = qty.ToString(),
                            size = size,
                            remarks = remarks
                        });
                        i++;
                    }
                }
            }
        }

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
        private void txtInventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInventName.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from ApparatusInventory", conn))
                {
                    i = 1;
                    summary.Clear();
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

                            int remarksIndex = reader.GetOrdinal("remarks");
                            string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                            summary.Add(new LVOutstanding
                            {
                                i = i,
                                prodCode = prodCode,
                                inventName = inventName,
                                manuf = manuf,
                                qty = qty.ToString(),
                                size = size,
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
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from ApparatusInventory where name LIKE @name", conn))
                {
                    cmd.Parameters.AddWithValue("@name", "%" + txtInventName.Text + "%");
                    i = 1;
                    summary.Clear();
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


                            int remarksIndex = reader.GetOrdinal("remarks");
                            string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                            summary.Add(new LVOutstanding
                            {
                                i = i,
                                prodCode = prodCode,
                                inventName = inventName,
                                manuf = manuf,
                                qty = qty.ToString(),
                                size = size,
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

        private void PackIconMaterial_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
            enableFields();
            process = 0;
        }


    }
}

