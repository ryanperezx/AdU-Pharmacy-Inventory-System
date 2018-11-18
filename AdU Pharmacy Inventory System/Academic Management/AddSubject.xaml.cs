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
using System.Data.Common;
using System.Text.RegularExpressions;


namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for AddSubject.xaml
    /// </summary>
    public partial class AddSubject : Page
    {
        int i = 1;
        bool check = false;
        public AddSubject()
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            fillInventory();
        }

        private void searchCode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubjCode.Text))
            {
                MessageBox.Show("Subject code field is empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from Subjects where subjCode = @subjCode", conn))
                {
                    cmd.Parameters.AddWithValue("@subjCode", txtSubjCode.Text);
                    int subjCount;
                    subjCount = (int)cmd.ExecuteScalar();
                    if (subjCount > 0)
                    {
                        string subjCode = txtSubjCode.Text;
                        using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT * from Subjects where subjCode = @subjCode", conn))
                        {
                            cmd1.Parameters.AddWithValue("@subjCode", subjCode);
                            using (SqlCeDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();
                                    //1
                                    int subjNameIndex = reader.GetOrdinal("subjName");
                                    string subjName = Convert.ToString(reader.GetValue(subjNameIndex));

                                    txtSubjName.Text = subjName;

                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Subject does not exist or is not in the database!");
                    }
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubjCode.Text) || string.IsNullOrEmpty(txtSubjName.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if(lvApparatus.Items.Count == 0)
            {
                MessageBox.Show("There are no apparatuses!");
            }
            else
            {
                string sMessageBoxText = "Are all fields correct?";
                string sCaption = "Add Subject";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlCeConnection conn = DBUtils.GetDBConnection();
                        conn.Open();

                        string subjCode = txtSubjCode.Text;
                        string subjName = txtSubjName.Text;
                        foreach (LVApparatusStockOut row in lvApparatus.Items)
                        {

                            using (SqlCeCommand cmd = new SqlCeCommand("INSERT into Subjects (subjCode, subjName, inventName, manuf, qty, size) VALUES (@subjCode, @subjName, @inventName, @manuf, @qty, @size)", conn))
                            {
                                cmd.Parameters.AddWithValue("@subjCode", subjCode);
                                cmd.Parameters.AddWithValue("@subjName", subjName);
                                cmd.Parameters.AddWithValue("@inventName", row.inventName);
                                cmd.Parameters.AddWithValue("@manuf", row.manuf);
                                cmd.Parameters.AddWithValue("@qty", row.qty);
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
                                    lvApparatus.Items.Clear();
                                    check = true;
                                }
                                catch (SqlCeException ex)
                                {
                                    MessageBox.Show("Error! Log has been updated with the error.");
                                }
                            }
                        }
                        if (check == true)
                        {
                            MessageBox.Show("Subject Added!");
                            emptyFields();
                        }
                        break;
                    case MessageBoxResult.No: break;
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubjCode.Text) || string.IsNullOrEmpty(txtSubjName.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from Subjects where subjCode = @subjCode", conn))
                {
                    cmd.Parameters.AddWithValue("@subjCode", txtSubjCode.Text);
                    int subjCount;
                    subjCount = (int)cmd.ExecuteScalar();
                    if (subjCount > 0)
                    {
                        string sMessageBoxText = "Do you want to delete the subject?";
                        string sCaption = "Delete Subject";
                        MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                        MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                        MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                        switch (dr)
                        {
                            case MessageBoxResult.Yes:
                                using (SqlCeCommand cmd1= new SqlCeCommand("INSERT into ArchivedSubjects (subjCode, subjName) select subjCode, subjName from Subjects where subjCode = @subjCode", conn))
                                {
                                    cmd1.Parameters.AddWithValue("@subjCode", txtSubjCode.Text);
                                    int result = cmd1.ExecuteNonQuery();
                                    if (result == 1)
                                    {
                                        using (SqlCeCommand command = new SqlCeCommand("DELETE from Subjects where subjCode= @subjCode", conn))
                                        {
                                            command.Parameters.AddWithValue("@subjCode", txtSubjCode.Text);
                                            int query = command.ExecuteNonQuery();
                                            MessageBox.Show("Subject has been deleted!");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Subject does not exist!");
                                    }

                                }

                                emptyFields();
                                break;

                            case MessageBoxResult.No: break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Subject does not exist!");
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
                        if (!string.IsNullOrEmpty(cmbSize.Text)){
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
                            lvApparatus.Items.Add(new LVApparatusStockOut
                            {
                                i = i,
                                inventName = cmbInventName.Text,
                                qty = reqQty,
                                manuf = manuf,
                                size = size,
                            });
                            i++;
                            emptyAppa();
                        }

                    }
                }
            }
        }

        private void emptyFields()
        {
            txtSubjCode.Text = null;
            txtSubjName.Text = null;
            check = false;
            i = 1;
        }

        private void emptyAppa()
        {
            cmbInventName.SelectedIndex = -1;
            cmbManuf.SelectedIndex = -1;
            cmbManuf.Items.Clear();
            cmbSize.SelectedIndex = -1;
            cmbSize.Items.Clear();
            txtQty.Text = null;
        }

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void PackIconMaterial_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new AddSubject());
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

    }
}
