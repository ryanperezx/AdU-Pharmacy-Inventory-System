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
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : MetroWindow
    {
        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void btnConfirmAns_Click(object sender, RoutedEventArgs e)
        {
            gridPassword.IsEnabled = true;
            gridPassword.Visibility = Visibility.Visible;

            gridSecurity.IsEnabled = false;
            gridSecurity.Visibility = Visibility.Collapsed;
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            gridSecurity.IsEnabled = true;
            gridSecurity.Visibility = Visibility.Visible;

            gridPassword.IsEnabled = false;
            gridPassword.Visibility = Visibility.Collapsed;
        }
    }
}
