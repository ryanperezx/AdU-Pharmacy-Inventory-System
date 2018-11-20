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
using System.Collections.ObjectModel;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for ApparatusStockOut.xaml
    /// </summary>
    public partial class ApparatusStockOut : Page
    {
        int i = 1;
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<LVApparatusStockOut> stockOut = new ObservableCollection<LVApparatusStockOut>();
        public ApparatusStockOut()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            view.Source = stockOut;
            lvAppaStockOut.DataContext = view;
            fillInventory();
            fillSubjects();
        }

        private void fillInventory()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT name from inventoryStock where inventType = 'Apparatus'", conn))
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

        private void fillSubjects()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT subjName from Subjects", conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        cmbSubject.Items.Clear();
                        while (reader.Read())
                        {
                            string subject = reader["subjName"].ToString();
                            cmbSubject.Items.Add(subject);
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

                        string size;
                        if (!string.IsNullOrEmpty(cmbSize.Text))
                        {
                            int sizeIndex = reader.GetOrdinal("size");
                            size = Convert.ToString(reader.GetValue(sizeIndex));
                        }
                        else
                        {
                            size = null;
                        }

                        int reqQty = Convert.ToInt32(txtQty.Text);
                        if (reqQty > qty)
                        {
                            MessageBox.Show("Requested quantity cannot be greater than the available quantity!");
                            return;
                        }
                        else
                        {
                            var found = stockOut.FirstOrDefault(x => (x.inventName == cmbInventName.Text) && (x.manuf == cmbManuf.Text) && (x.size == cmbSize.Text));
                            if (found != null)
                            {
                                if (found.qty + reqQty > qty)
                                {
                                    MessageBox.Show("Requested quantity cannot be greater than the available quantity!");
                                    return;
                                }
                                else
                                {
                                    found.qty = found.qty + reqQty;
                                }
                            }
                            else
                            {
                                stockOut.Add(new LVApparatusStockOut
                                {
                                    i = i,
                                    inventName = cmbInventName.Text,
                                    qty = reqQty,
                                    manuf = manuf,
                                    size = size
                                });
                                i++;
                            }
                            emptyFields();
                        }

                    }
                }
            }
        }

        private void btnStockOut_Click(object sender, RoutedEventArgs e) //NOT CHECKED
        {
            if (lvAppaStockOut.Items.Count == 0)
            {
                MessageBox.Show("There are no apparatus(es) to be stock out");
            }
            else
            {

                string sMessageBoxText = "Are all fields correct?";
                string sCaption = "Stock Out";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:

                        SqlCeConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        foreach (var row in stockOut)
                        {
                            using (SqlCeCommand cmd = new SqlCeCommand("UPDATE InventoryStock set qty = qty - @qty where name = @inventType and size = @size and manuf = @manuf", conn))
                            {
                                cmd.Parameters.AddWithValue("@qty", row.qty);
                                cmd.Parameters.AddWithValue("@inventType", row.inventName);
                                cmd.Parameters.AddWithValue("@manuf", row.manuf);
                                if (!string.IsNullOrEmpty(row.size))
                                {
                                    cmd.Parameters.AddWithValue("@size", row.size);
                                }
                                else
                                {
                                    row.size = "";
                                    cmd.Parameters.AddWithValue("@size", row.size);
                                }
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Stock Out Successfully");
                                    using (SqlCeCommand cmd1 = new SqlCeCommand("INSERT into BorrowerList (studentNo, fullName, date, groupID, subject, borrowedInventName, manuf, qty) VALUES (@studentNo, @fullName, @date, @groupID, @subject, @borrowedInventName, @manuf, @qty)", conn))
                                    {
                                        cmd1.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                                        cmd1.Parameters.AddWithValue("@fullName", txtBName.Text);
                                        cmd1.Parameters.AddWithValue("@date", txtDate.Text);
                                        cmd1.Parameters.AddWithValue("@groupID", txtGroup.Text);
                                        cmd1.Parameters.AddWithValue("@subject", cmbSubject.Text);
                                        cmd1.Parameters.AddWithValue("@borrowedInventName", row.inventName);
                                        cmd1.Parameters.AddWithValue("@manuf", row.manuf);
                                        cmd1.Parameters.AddWithValue("@qty", row.qty);
                                        try
                                        {
                                            cmd1.ExecuteNonQuery();
                                        }
                                        catch
                                        {
                                            MessageBox.Show("Error! Log has been updated with the error");
                                        }
                                    }

                                }
                                catch (SqlException ex)
                                {
                                    MessageBox.Show("Error! Log has been updated with the error");
                                }
                            }
                        }
                        stockOut.Clear();
                        i = 1;
                        emptyFields();
                        break;
                    case MessageBoxResult.No: break;

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
            cmbManuf.Items.Clear();
            cmbSize.SelectedIndex = -1;
            cmbSize.Items.Clear();
            txtQty.Text = null;

            txtDate.Text = null;
            txtBName.Text = null;
            txtGroup.Text = null;
            txtStudNo.Text = null;
            cmbSubject.SelectedIndex = -1;

        }

        private void txtInventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            fillSize();
            if (string.IsNullOrEmpty(txtSize.Text))
            {
                fillManufacturer();
            }
        }

        private void txtSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            fillManufacturer();
        }

        private void fillSize()
        {
            if (!string.IsNullOrEmpty(cmbInventName.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                cmbSize.Items.Clear();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT size from inventoryStock where name = @inventName", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
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

        private void fillManufacturer()
        {
            if (!string.IsNullOrEmpty(cmbSize.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                cmbManuf.Items.Clear();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT manuf from inventoryStock where name = @inventName and size = @size", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    cmd.Parameters.AddWithValue("@size", cmbSize.Text);
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
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                cmbManuf.Items.Clear();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT manuf from inventoryStock where name = @inventName", conn))
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

        private void PackIconMaterial_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new ApparatusStockOut());
        }

    }

}
