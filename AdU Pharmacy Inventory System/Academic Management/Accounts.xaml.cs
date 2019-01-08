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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts : Page
    {
        public Accounts()
        {
            InitializeComponent();
        }

        public string passwordStatus;
        public string PasswordStatus
        {
            get { return passwordStatus; }
            set
            {
                passwordStatus = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        private void searchUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                MessageBox.Show("Username field is empty!");
                txtUser.Focus();
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from Accounts where username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUser.Text);
                    int userCount;
                    userCount = (int)cmd.ExecuteScalar();
                    if (userCount > 0)
                    {
                        using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT * from Accounts where username = @username", conn))
                        {
                            cmd1.Parameters.AddWithValue("@username", txtUser.Text);
                            using (SqlCeDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                if (reader.HasRows)
                                {
                                    reader.Read();

                                    int firstNameIndex = reader.GetOrdinal("firstName");
                                    string firstName = Convert.ToString(reader.GetValue(firstNameIndex));

                                    int lastNameIndex = reader.GetOrdinal("lastName");
                                    string lastName = Convert.ToString(reader.GetValue(lastNameIndex));

                                    int securityQuestionIndex = reader.GetOrdinal("securityQuestion");
                                    string securityQuestion = Convert.ToString(reader.GetValue(securityQuestionIndex));

                                    txtFirstName.Text = firstName;
                                    txtLastName.Text = lastName;
                                    cmbQuestion.Text = securityQuestion;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("User does not exist!");
                    }
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Regex reg = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$");
            bool result = reg.IsMatch(txtPass.Password.ToString());

            if (result)
            {
                if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(cmbQuestion.Text) || string.IsNullOrEmpty(txtAns.Password) || string.IsNullOrEmpty(txtConfirmPass.Password) || string.IsNullOrEmpty(txtPass.Password) || string.IsNullOrEmpty(txtPass.Password))
                {
                    MessageBox.Show("One or more fields are empty!");
                }
                else
                {
                    if (txtPass.Password.Equals(txtConfirmPass.Password))
                    {
                        SqlCeConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from Accounts where username = @username", conn))
                        {
                            cmd.Parameters.AddWithValue("@username", txtUser.Text);
                            int userCount;
                            userCount = (int)cmd.ExecuteScalar();
                            if (userCount > 0)
                            {
                                MessageBox.Show("User already exist!");
                            }
                            else
                            {
                                string sMessageBoxText = "Are all fields correct?";
                                string sCaption = "Add Account";
                                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                                switch (dr)
                                {
                                    case MessageBoxResult.Yes:
                                        using (SqlCeCommand cmd1 = new SqlCeCommand("INSERT into Accounts (firstName, lastName, username, password, securityQuestion, answer, tries, userLevel) VALUES (@firstName, @lastName, @username, @password, @securityQuestion, @answer, 0, @userLevel)", conn))
                                        {
                                            cmd1.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                                            cmd1.Parameters.AddWithValue("@lastName", txtLastName.Text);
                                            cmd1.Parameters.AddWithValue("@username", txtUser.Text);
                                            cmd1.Parameters.AddWithValue("@password", txtPass.Password);
                                            cmd1.Parameters.AddWithValue("@securityQuestion", cmbQuestion.Text);
                                            cmd1.Parameters.AddWithValue("@answer", txtAns.Password);
                                            if (cmbUserLevel.Text.Equals("Administrator"))
                                                cmd1.Parameters.AddWithValue("@userLevel", 0);
                                            else
                                                cmd1.Parameters.AddWithValue("@userLevel", 1);

                                            try
                                            {
                                                cmd1.ExecuteNonQuery();
                                                MessageBox.Show("Registered successfully");
                                                emptyFields();

                                            }
                                            catch (SqlCeException ex)
                                            {
                                                MessageBox.Show("Error! Log has been updated with the error!");
                                            }
                                        }
                                        break;
                                    case MessageBoxResult.No:
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Your password and confirmation password do not match.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Password is Invalid");
            }
            
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                MessageBox.Show("Username field is empty!");
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("Select COUNT(1) from Accounts where username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUser.Text);
                    int userCount;
                    userCount = (int)cmd.ExecuteScalar();
                    if (userCount > 0)
                    {
                        string sMessageBoxText = "Do you want to delete the account?";
                        string sCaption = "Delete Account";
                        MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                        MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                        MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                        switch (dr)
                        {
                            case MessageBoxResult.Yes:
                                using (SqlCeCommand cmd1 = new SqlCeCommand("INSERT into ArchivedAccounts (firstName, lastName, username, password, securityQuestion, answer) select firstName, lastName, username, password, securityQuestion, answer from Accounts where username = @username", conn))
                                {
                                    cmd1.Parameters.AddWithValue("@username", txtUser.Text);
                                    try
                                    {
                                        cmd1.ExecuteNonQuery();
                                        using (SqlCeCommand command = new SqlCeCommand("DELETE from Accounts where username= @username", conn))
                                        {
                                            command.Parameters.AddWithValue("@username", txtUser.Text);
                                            int query = command.ExecuteNonQuery();
                                            MessageBox.Show("Account has been deleted!");
                                            emptyFields();
                                        }
                                    }
                                    catch(SqlCeException ex)
                                    {
                                        MessageBox.Show("Error! Log has been updated with the error!");
                                    }
                                }
                                break;
                            case MessageBoxResult.No:
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("User does not exist!");

                    }
                }
            }
        }

        private void emptyFields()
        {
            txtFirstName.Text = null;
            txtLastName.Text = null;
            txtUser.Text = null;
            txtPass.Password = null;
            txtConfirmPass.Password = null;
            cmbQuestion.SelectedIndex = -1;
            txtAns.Password = null;
        }

        private void PackIconMaterial_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();
        }

        private void txtPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //Password must contain at least 8 characters, 1 uppercase, 1 numeric
            Regex reg = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$");
            bool result = reg.IsMatch(txtPass.Password.ToString());

            if (result)
            {
                passwordStatus = "Password is Valid";
                lblPassword.Content = passwordStatus;
                lblPassword.Foreground = Brushes.Green;
            }
            else
            {
                passwordStatus = "Password is Invalid";
                lblPassword.Content = passwordStatus;
                lblPassword.Foreground = Brushes.Red;
            }
            if (string.IsNullOrEmpty(txtPass.Password))
            {
                lblPassword.Content = null;
            }
        }
    }
}
