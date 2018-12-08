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
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(1) from Subjects where subjCode = @subjCode", conn))
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
                                    string subjName = null;
                                    while (reader.Read())
                                    {
                                        int subjNameIndex = reader.GetOrdinal("subjName");
                                        subjName = Convert.ToString(reader.GetValue(subjNameIndex));

                                        int prodCodeIndex = reader.GetOrdinal("prodCode");
                                        string prodCode = Convert.ToString(reader.GetValue(prodCodeIndex));

                                        int sizeIndex = reader.GetOrdinal("size");
                                        string size = Convert.ToString(reader.GetValue(sizeIndex));

                                        int qtyIndex = reader.GetOrdinal("qty");
                                        int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                                        using (SqlCeCommand cmd2 = new SqlCeCommand("SELECT qty, name from ApparatusInventory where prodCode = @prodCode and size = @size", conn))
                                        {
                                            cmd2.Parameters.AddWithValue("@prodCode", prodCode);
                                            cmd2.Parameters.AddWithValue("@size", size);
                                            using (DbDataReader dr = cmd2.ExecuteResultSet(ResultSetOptions.Scrollable))
                                            {
                                                if (dr.HasRows)
                                                {
                                                    dr.Read();
                                                    int nameIndex = dr.GetOrdinal("name");
                                                    string name = Convert.ToString(dr.GetValue(nameIndex));

                                                    lvApparatus.Items.Add(new LVApparatusStockOut
                                                    {
                                                        i = i,
                                                        inventName = name,
                                                        qty = qty,
                                                        size = size,
                                                    });
                                                }
                                            }
                                        }

                                        i++;

                                    }
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

        private void PackIconMaterial_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new AddSubject());
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubjCode.Text) || string.IsNullOrEmpty(txtSubjName.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (lvApparatus.Items.Count == 0)
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
                            int ordinal = 0;
                            string prodCode = null;
                            using(SqlCeCommand cmd = new SqlCeCommand("SELECT prodCode from ApparatusInventory where name = @inventName and size = @size",conn))
                            {
                                cmd.Parameters.AddWithValue("@inventName", row.inventName);
                                cmd.Parameters.AddWithValue("@size", row.size);
                                DbDataReader result = cmd.ExecuteResultSet(ResultSetOptions.Scrollable);
                                if (result.Read())
                                {
                                    ordinal = result.GetOrdinal("prodCode");
                                    prodCode = Convert.ToString(result.GetValue(ordinal));
                                }

                            }
                            using (SqlCeCommand cmd = new SqlCeCommand("INSERT into Subjects (subjCode, subjName, prodCode, qty) VALUES (@subjCode, @subjName, @prodCode, @qty)", conn))
                            {
                                cmd.Parameters.AddWithValue("@subjCode", subjCode);
                                cmd.Parameters.AddWithValue("@subjName", subjName);
                                cmd.Parameters.AddWithValue("@prodCode", prodCode);
                                cmd.Parameters.AddWithValue("@qty", row.qty);
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    check = true;
                                }
                                catch (SqlCeException ex)
                                {
                                    MessageBox.Show("Error! Log has been updated with the error. " + ex);
                                }
                            }
                        }
                        if (check == true)
                        {
                            MessageBox.Show("Subject Added!");
                            lvApparatus.Items.Clear();
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
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(1) from Subjects where subjCode = @subjCode", conn))
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
                                using (SqlCeCommand cmd1 = new SqlCeCommand("INSERT into ArchivedSubjects (subjCode, subjName) select TOP 1 subjCode, subjName from Subjects where subjCode = @subjCode", conn))
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
                                        MessageBox.Show("Subject does not exist1!");
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
            if (string.IsNullOrEmpty(cmbInventName.Text) || string.IsNullOrEmpty(txtQty.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from ApparatusInventory where name = @inventName and size = @size", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    cmd.Parameters.AddWithValue("@size", cmbSize.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        reader.Read();


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

                        lvApparatus.Items.Add(new LVApparatusStockOut
                        {
                            i = i,
                            inventName = cmbInventName.Text,
                            qty = Convert.ToInt32(txtQty.Text),
                            size = size,
                        });
                        i++;
                        emptyAppa();

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
            cmbSize.SelectedIndex = -1;
            cmbSize.Items.Clear();
            txtQty.Text = null;
        }

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtInventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmbSize.Items.Clear();
            fillSize();
        }

        private void fillInventory()
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
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT size from ApparatusInventory where name = @inventName", conn))
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

    }
}
