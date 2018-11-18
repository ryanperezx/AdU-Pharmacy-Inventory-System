﻿using System;
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
                                    using (SqlCeCommand cmd1 = new SqlCeCommand("INSERT into Accounts (firstName, lastName, username, password, securityQuestion, answer, tries) VALUES (@firstName, @lastName, @username, @password, @securityQuestion, @answer, 0)", conn))
                                    {
                                        cmd1.Parameters.AddWithValue("@firstName", txtFirstName.Text);
                                        cmd1.Parameters.AddWithValue("@lastName", txtLastName.Text);
                                        cmd1.Parameters.AddWithValue("@username", txtUser.Text);
                                        cmd1.Parameters.AddWithValue("@password", txtPass.Password);
                                        cmd1.Parameters.AddWithValue("@securityQuestion", cmbQuestion.Text);
                                        cmd1.Parameters.AddWithValue("@answer", txtAns.Password);
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
    }
}