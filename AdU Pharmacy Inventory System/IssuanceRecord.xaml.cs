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
using System.Data.SqlServerCe;
using System.Collections.ObjectModel;
using System.Data.Common;
namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for IssuanceRecord.xaml
    /// </summary>
    public partial class IssuanceRecord : Page
    {
        ObservableCollection<LVIssuance> request = new ObservableCollection<LVIssuance>();
        List<StudentInfo> studInfo = new List<StudentInfo>();

        public IssuanceRecord(string lockNo, string section, string sched, string issuedDate, string issuedBy)
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            fillStudents();
            updateRecords();
        }

        private void fillStudents()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT studentNo, fullName from IssuanceList where subject = @subject and lockNo = @lockNo", conn))
            {
                /*
                cmd.Parameters.AddWithValue("@groupID", txtGrpID.Text);
                cmd.Parameters.AddWithValue("@subject", txtSubj.Text);
                cmd.Parameters.AddWithValue("@expName", txtExpName.Text);
                cmd.Parameters.AddWithValue("@dateReq", txtDateReq.Text);
                cmd.Parameters.AddWithValue("@dateExp", txtDateExp.Text);
                cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                using (DbDataReader rd = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    while (rd.Read())
                    {
                        int studentNoIndex = rd.GetOrdinal("studentNo");
                        int studentNo = Convert.ToInt32(rd.GetValue(studentNoIndex));

                        int fullNameIndex = rd.GetOrdinal("fullName");
                        string fullName = Convert.ToString(rd.GetValue(fullNameIndex));

                        studInfo.Add(new StudentInfo
                        {
                            studName = fullName,
                            studNo = studentNo
                        });
                    }
                }
               */
            }
        }

        private void updateRecords()
        {

        }

        private void imgBack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new IssuanceList());
        }
    }
}
