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
using NLog;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : MetroWindow
    {
        string user, question, answer;
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public ForgotPassword(string username)
        {
            InitializeComponent();
            user = username;
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT securityQuestion, answer FROM Accounts WHERE username = @username", conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                SqlCeDataReader dr = cmd.ExecuteResultSet(ResultSetOptions.Scrollable);
                if (dr.Read())
                {
                    int securityQuestionIndex = dr.GetOrdinal("securityQuestion");
                    question = Convert.ToString(dr.GetValue(securityQuestionIndex));

                    int answerIndex = dr.GetOrdinal("answer");
                    answer = Convert.ToString(dr.GetValue(answerIndex));

                    securityQuestion.Content = question;
                }
            }
        }

        private void btnConfirmAns_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAnswer.Text))
            {
                MessageBox.Show("Answer field is empty!");
                txtAnswer.Focus();
            }
            else {
                if (txtAnswer.Text.Equals(answer))
                {
                    gridPassword.IsEnabled = true;
                    gridPassword.Visibility = Visibility.Visible;

                    gridSecurity.IsEnabled = false;
                    gridSecurity.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Security answer is wrong");
                }
            }
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNewPass.Password) || string.IsNullOrEmpty(txtConfirmPass.Password))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else
            {
                if (txtNewPass.Password.Equals(txtConfirmPass.Password))
                {
                    string sMessageBoxText = "Are all fields checked?";
                    string sCaption = "Confirm Change Password";
                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                    switch (dr)
                    {
                        case MessageBoxResult.Yes:
                            SqlCeConnection conn = DBUtils.GetDBConnection();
                            conn.Open();
                            using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE Accounts SET password = @password, tries = @tries WHERE username = @username", conn))
                            {
                                cmd1.Parameters.AddWithValue("@username", user);
                                cmd1.Parameters.AddWithValue("@password", txtNewPass.Password);
                                cmd1.Parameters.AddWithValue("@tries", 0);
                                cmd1.ExecuteNonQuery();
                                MessageBox.Show("Password has been changed.");

                                Log = LogManager.GetLogger("AccountLog");
                                Log.Info("Account: " + user + " changed their password");
                            }
                            this.DialogResult = false;

                            break;


                        case MessageBoxResult.No: break;
                    }

                }
                else
                {
                    MessageBox.Show("New password and confirmation password do not match.");
                }
            }
        }
    }
}
