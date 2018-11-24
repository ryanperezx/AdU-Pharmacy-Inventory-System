using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Data.Common;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for Add_Chemicals.xaml
    /// </summary>
    public partial class Add_Chemicals : Page
    {
        bool process = false;
        public Add_Chemicals()
        {
            InitializeComponent();
        }


        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtChem.Text) || string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtQty.Text) || string.IsNullOrEmpty(txtReceipt.Text) || string.IsNullOrEmpty(txtReceivedFrom.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using(SqlCeCommand cmd = new SqlCeCommand("INSERT into ChemicalInventory (date, name, qty, receivedFrom, receipt) VALUES (@date, @name, @qty, @receivedFrom, @receipt)", conn))
                {
                    cmd.Parameters.AddWithValue("@date", txtDate.Text);
                    cmd.Parameters.AddWithValue("@name", txtChem.Text);
                    cmd.Parameters.AddWithValue("@qty", txtQty.Text);
                    cmd.Parameters.AddWithValue("@receivedFrom", txtReceivedFrom.Text);
                    cmd.Parameters.AddWithValue("@receipt", txtReceipt.Text);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Added Successfully");
                        emptyFields();
                    }
                    catch(SqlCeException ex)
                    {
                        MessageBox.Show("Error! Log has been updated with the error!");
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void emptyFields()
        {
            txtDate.Text = null;
            txtChem.Text = null;
            txtQty.Text = null;
            txtReceivedFrom.Text = null;
            txtReceipt.Text = null;
        }

        private void Label_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new Add_Chemicals());
        }

    }
}
