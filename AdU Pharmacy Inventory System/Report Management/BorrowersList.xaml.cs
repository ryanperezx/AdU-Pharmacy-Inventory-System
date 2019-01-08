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
using System.Data.Common;
using System.Data.SqlClient;


namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for BorrowersList.xaml
    /// </summary>
    public partial class BorrowersList : Page
    {
        int i = 1;
        public BorrowersList()
        {
            InitializeComponent();
            date.Text = DateTime.Now.ToString("dd MMMM yyyy");
            fillList();
        }

        private void fillList()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT dateReq, dateExp, groupID, lockNo, subject, expName from BorrowerList", conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        lvList.Items.Clear();
                        while (reader.Read())
                        {
                            int dateReqIndex = reader.GetOrdinal("dateReq");
                            DateTime myDate = reader.GetDateTime(dateReqIndex);
                            string dateReq = myDate.ToString("MM/dd/yyyy");

                            int dateExpIndex = reader.GetOrdinal("dateExp");
                            myDate = reader.GetDateTime(dateExpIndex);
                            string dateExp = myDate.ToString("MM/dd/yyyy");

                            int subjIndex = reader.GetOrdinal("subject");
                            string subj = Convert.ToString(reader.GetValue(subjIndex));

                            int lockNoIndex = reader.GetOrdinal("lockNo");
                            int lockNo = Convert.ToInt32(reader.GetValue(lockNoIndex));

                            int expNameIndex = reader.GetOrdinal("expName");
                            string expName = Convert.ToString(reader.GetValue(expNameIndex));

                            int grpIDIndex = reader.GetOrdinal("groupID");
                            int grpID = Convert.ToInt32(reader.GetValue(grpIDIndex));

                            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT TOP 1 studentNo, fullName from BorrowerList where groupID = @groupID and subject = @subject and expName = @expName and DATEDIFF(day, dateReq, @dateReq) = 0 and DATEDIFF(day, dateExp, @dateExp) = 0 and lockNo = @lockNo", conn))
                            {
                                cmd1.Parameters.AddWithValue("@groupID", grpID);
                                cmd1.Parameters.AddWithValue("@subject", subj);
                                cmd1.Parameters.AddWithValue("@expName", expName);
                                cmd1.Parameters.AddWithValue("@dateReq", dateReq);
                                cmd1.Parameters.AddWithValue("@dateExp", dateExp);
                                cmd1.Parameters.AddWithValue("@lockNo", lockNo);
                                using (DbDataReader rd = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    rd.Read();
                                    int studentNoIndex = rd.GetOrdinal("studentNo");
                                    string studentNo = Convert.ToString(rd.GetValue(studentNoIndex));

                                    int fullNameIndex = rd.GetOrdinal("fullName");
                                    string fullName = Convert.ToString(rd.GetValue(fullNameIndex));

                                    lvList.Items.Add(new LVBorrower
                                    {
                                        dateReq = dateReq,
                                        dateExp = dateExp,
                                        studentNo = studentNo,
                                        fullName = fullName,
                                        lockNo = lockNo.ToString(),
                                        subj = subj,
                                        grpID = grpID,
                                        experiment = expName,
                                    });
                                    i++;

                                }
                            }

                        }
                    }
                }
            }
        }

        private void lvList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LVBorrower borrower = lvList.SelectedItem as LVBorrower;
            this.NavigationService.Navigate(new BorrowerRecord(borrower.studentNo, borrower.dateReq, borrower.dateExp, borrower.subj, borrower.grpID, borrower.experiment, borrower.lockNo));
        }
    }
}
