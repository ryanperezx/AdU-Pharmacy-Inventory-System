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
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT dateReq, dateExp, studentNo, fullName, groupID, lockNo, subject, expName from BorrowerList", conn))
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

                            int studentNoIndex = reader.GetOrdinal("studentNo");
                            string studentNo = Convert.ToString(reader.GetValue(studentNoIndex));

                            int fullNameIndex = reader.GetOrdinal("fullName");
                            string fullName = Convert.ToString(reader.GetValue(fullNameIndex));

                            int subjIndex = reader.GetOrdinal("subject");
                            string subj = Convert.ToString(reader.GetValue(subjIndex));

                            int lockNoIndex = reader.GetOrdinal("lockNo");
                            int lockNo = Convert.ToInt32(reader.GetValue(lockNoIndex));

                            int expNameIndex = reader.GetOrdinal("expName");
                            string expName = Convert.ToString(reader.GetValue(expNameIndex));

                            int grpIDIndex = reader.GetOrdinal("groupID");
                            int grpID = Convert.ToInt32(reader.GetValue(grpIDIndex));


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

        private void lvList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LVBorrower borrower = lvList.SelectedItem as LVBorrower;
            this.NavigationService.Navigate(new BorrowerRecord(borrower.fullName, borrower.dateReq, borrower.dateReq, borrower.studentNo, borrower.subj, borrower.grpID, borrower.experiment, borrower.lockNo));
        }
    }
}
