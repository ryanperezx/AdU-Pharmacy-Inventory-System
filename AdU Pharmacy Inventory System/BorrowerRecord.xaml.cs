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
using System.Data.SqlServerCe;
using System.Collections.ObjectModel;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for BorrowerRecord.xaml
    /// </summary>
    public partial class BorrowerRecord : Page
    {
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<LVBorrower> borrowers = new ObservableCollection<LVBorrower>();
        public BorrowerRecord(string fullName, string dateReq, string dateExp, string studno, string subj, int grpID, string experiment, string lockNo)
        {
            InitializeComponent();
            txtDateReq.Text = dateReq;
            txtDateExp.Text = dateExp;
            txtStudNo.Text = studno;
            txtSubj.Text = subj;
            txtFullName.Text = fullName;
            txtLockNo.Text = lockNo;
            txtExpName.Text = experiment;
            txtGrpID.Text = Convert.ToString(grpID);
            view.Source = borrowers;
            lvRecords.DataContext = view;
            updateRecords();
        }

        private void updateRecords()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT bl.prodCode, ai.name, bl.manuf, bl.qty from BorrowerList bl INNER JOIN ApparatusInventory ai on bl.prodCode = ai.prodCode where bl.studentNo = @studentNo and bl.groupID = @grpID and bl.subject = @subj and bl.expName = @experiment", conn))
            {
                cmd.Parameters.AddWithValue("@studentNo", txtStudNo.Text);
                cmd.Parameters.AddWithValue("@subj", txtSubj.Text);
                cmd.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                cmd.Parameters.AddWithValue("@experiment", txtExpName.Text);
                lvRecords.Items.Clear();
                using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    while (reader.Read())
                    {
                        int nameIndex = reader.GetOrdinal("name");
                        string name = Convert.ToString(reader.GetValue(nameIndex));

                        int manufIndex = reader.GetOrdinal("manuf");
                        string manuf = Convert.ToString(reader.GetValue(manufIndex));

                        int qtyIndex = reader.GetOrdinal("qty");
                        int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                        borrowers.Add(new LVBorrower
                        {
                            prodName = name,
                            manuf = manuf,
                            qty = qty
                        });
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("INSERT into ArchiveBorrowerList SELECT dateReq, dateExp, studentNo, fullName, groupID, lockNo, subject, expName, prodCode, manuf, qty from BorrowerList where studentNo = @studNo and groupID = @grpID and subject = @subjName and expName = @experiment", conn))
            {
                cmd.Parameters.AddWithValue("@studNo", txtStudNo.Text);
                cmd.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                cmd.Parameters.AddWithValue("@subjName", txtSubj.Text);
                cmd.Parameters.AddWithValue("@experiment", txtExpName.Text);
                try
                {
                    cmd.ExecuteNonQuery();
                    bool check = false;
                    foreach (var item in borrowers)
                    {
                        using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE ai set ai.qty = ai.qty + @qty from ApparatusInventory ai INNER JOIN BorrowerList bl on bl.prodCode = ai.prodCode where bl.prodName = @prodName and bl.manuf = @manuf", conn))
                        {
                            cmd1.Parameters.AddWithValue("@manuf", item.manuf);
                            cmd1.Parameters.AddWithValue("@prodName", item.prodName);
                            cmd1.Parameters.AddWithValue("@qty", item.qty);

                            try
                            {
                                using (SqlCeCommand command = new SqlCeCommand("DELETE from BorrowerList where studentNo = @studNo and groupID = @grpID and subject = @subjName and expName = @experiment", conn))
                                {
                                    command.Parameters.AddWithValue("@studNo", txtStudNo.Text);
                                    command.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                                    command.Parameters.AddWithValue("@subjName", txtSubj.Text);
                                    command.Parameters.AddWithValue("@experiment", txtExpName.Text);
                                    command.ExecuteNonQuery();
                                    check = true;
                                }
                            }
                            catch(SqlCeException ex)
                            {
                                MessageBox.Show("Error! Log has been updated with the error!" + ex);
                            }

                        }
                    }
                    if(check == true)
                    {
                        MessageBox.Show("Record has been deleted!");
                        this.NavigationService.Navigate(new BorrowersList());
                    }
                }
                catch (SqlCeException ex)
                {
                    MessageBox.Show("Error! Log has been updated with the error!" + ex);
                }

            }
        }

        private void imgBack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new BorrowersList());
        }
    }
}
