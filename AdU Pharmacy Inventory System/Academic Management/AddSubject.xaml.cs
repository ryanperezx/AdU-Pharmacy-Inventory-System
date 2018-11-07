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


namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for AddSubject.xaml
    /// </summary>
    public partial class AddSubject : Page
    {
        public AddSubject()
        {
            InitializeComponent();
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
                        using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT * from Subjects where subjCode = @subjCode",conn))
                        {
                            cmd1.Parameters.AddWithValue("@subjCode", subjCode);
                            using(SqlCeDataReader reader = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
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
            if(string.IsNullOrEmpty(txtSubjCode.Text) || string.IsNullOrEmpty(txtSubjName.Text))
            {
                MessageBox.Show("One or more fields are empty!");
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

                        using (SqlCeCommand cmd = new SqlCeCommand("INSERT into Subjects (subjCode, subjName) VALUES (@subjCode, @subjName)", conn))
                        {
                            cmd.Parameters.AddWithValue("@subjCode", subjCode);
                            cmd.Parameters.AddWithValue("@subjName", subjName);

                            try
                            {
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Subject Added!");
                                emptyTextbox();
                            }
                            catch (SqlCeException ex)
                            {
                                MessageBox.Show("Error! Log has been updated with the error.");
                            }
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
                string sMessageBoxText = "Do you want to delete the subject?";
                string sCaption = "Delete Subject";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        SqlCeConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        using (SqlCeCommand command = new SqlCeCommand("DELETE from Subjects where subjCode= @subjCode", conn))
                        {
                            command.Parameters.AddWithValue("@subjCode", txtSubjCode.Text);
                            int result = command.ExecuteNonQuery();
                            if (result == 1)
                            {
                                MessageBox.Show("Subject has been deleted!");
                            }
                            else
                            {
                                MessageBox.Show("Subject does not exist!");
                                return;
                            }
                        }
                        emptyTextbox();
                        break;

                    case MessageBoxResult.No: break;
                }
            }
            }

        private void emptyTextbox()
        {
            txtSubjCode.Text = null;
            txtSubjName.Text = null;
        }
    }
}
