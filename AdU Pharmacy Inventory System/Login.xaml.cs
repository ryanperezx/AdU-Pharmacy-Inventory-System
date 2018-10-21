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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : MetroWindow
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            new MainWindow().ShowDialog();
            ShowDialog();
        }

        private void lblForgot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (txtUsername.Text == "")
            {
                MessageBox.Show("No User ID input");
            }
            else
            {
                txtUsername.Text = "";
                txtPassword.Password = "";
                Hide();
                new ForgotPassword().ShowDialog();
                ShowDialog();
            }
        }

        private void txtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUsername.FontStyle = FontStyles.Normal;
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.FontStyle = FontStyles.Normal;
        }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                txtUsername.FontStyle = FontStyles.Italic;
            }
            else
            {
                txtUsername.FontStyle = FontStyles.Normal;

            }
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                txtPassword.FontStyle = FontStyles.Italic;
            }
            else
            {
                txtUsername.FontStyle = FontStyles.Normal;
            }
        }
    }
}
