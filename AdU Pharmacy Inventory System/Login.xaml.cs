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
using System.Data.SqlServerCe;
using System.Data.Common;
using System.Diagnostics;
using NLog;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : MetroWindow
    {
        string user;
        public static int userLevel;
        private static Logger Log = LogManager.GetCurrentClassLogger();


        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("One or more fields are empty!");
                return;
            }
            else
            {

                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                Nullable<int> loginAttempts;
                using (SqlCeCommand cmd = new SqlCeCommand("Select tries FROM Accounts WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    loginAttempts = Convert.ToInt32(cmd.ExecuteScalar());
                }
                if (loginAttempts < 5)
                {
                    string un = txtUsername.Text;
                    string pw = txtPassword.Password;

                    using (SqlCeCommand cmd = new SqlCeCommand("Select * from Accounts where username = @username AND password = @password", conn))
                    {
                        cmd.Parameters.AddWithValue("@username", un);
                        cmd.Parameters.AddWithValue("@password", pw);
                        SqlCeDataReader dr = cmd.ExecuteResultSet(ResultSetOptions.Scrollable);

                        if (dr.Read())
                        {
                            string lName, fName, mName;
                            lName = dr.GetString(2);
                            fName = dr.GetString(3);
                            mName = dr.GetString(4);
                            int userLevelIndex = dr.GetOrdinal("userLevel");
                            userLevel = dr.GetInt32(userLevelIndex);
                            Debug.WriteLine(userLevel);

                            using (SqlCeCommand cmd2 = new SqlCeCommand("UPDATE Accounts SET tries = 0", conn))
                            {
                                dr.Close();
                                dr.Dispose();
                                cmd2.ExecuteNonQuery();
                                MessageBox.Show("Login Successful");
                                Log = LogManager.GetLogger("userLogin");
                                Log.Info(" Account Name: " + txtUser.Text + " has logged in.");
                            }

                        }

                        else
                        {
                            using (SqlCeCommand cmd2 = new SqlCeCommand("Select username from Accounts where username = @username", conn))
                            {
                                cmd2.Parameters.AddWithValue("@username", un);
                                dr.Close();
                                dr.Dispose();
                                dr = cmd2.ExecuteReader();
                                int ordinal = 0;
                                string value = "";

                                if (dr.Read())
                                {
                                    ordinal = dr.GetOrdinal("username");
                                    value = dr.GetString(ordinal);
                                    if (value.Equals(un))
                                    {
                                        using (SqlCeCommand cmd3 = new SqlCeCommand("UPDATE Accounts SET tries = tries + 1 WHERE username = @username", conn))
                                        {
                                            cmd3.Parameters.AddWithValue("@username", un);
                                            dr.Close();
                                            dr.Dispose();
                                            cmd3.ExecuteNonQuery();
                                            cmd3.Dispose();
                                        }
                                    }
                                }
                            }
                            MessageBox.Show("Username or Password is invalid");
                            return;
                        }
                    }
                    Hide();
                    new MainWindow(txtUsername.Text).ShowDialog();
                    ShowDialog();
                    txtPassword.Password = null;
                    txtUsername.Text = null;
                }
                else
                {
                    user = txtUsername.Text;
                    string sMessageBoxText = "Due to multiple login attempts, your account has been locked. \nPlease unlock it to continue.";
                    string sCaption = "Account Recovery";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            Hide();
                            new ForgotPassword(txtUsername.Text).ShowDialog();
                            ShowDialog();
                            break;

                        case MessageBoxResult.No: break;
                    }
                }
            }
            */
            string user = "admin";
            Hide();
            new MainWindow(user).ShowDialog();
            ShowDialog();
        }

        private void lblForgot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("No username input");
                txtUsername.Focus();
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from Accounts where username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    int userCount;
                    userCount = (int)cmd.ExecuteScalar();
                    if (userCount > 0)
                    {
                        Hide();
                        new ForgotPassword(txtUsername.Text).ShowDialog();
                        ShowDialog();
                        txtPassword.Password = null;
                        txtUsername.Text = null;
                    }
                    else
                    {
                        MessageBox.Show("User does not exist!");

                    }
                }
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
