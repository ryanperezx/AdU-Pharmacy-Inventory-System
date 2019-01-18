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



        public IssuanceRecord(string lockNo, string section, string sched, string issuedDate, string issuedBy, string subject)
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            fillStudents();
            updateRecords();
            txtLockNo.Text = lockNo;
            txtSection.Text = section;
            txtSched.Text = sched;
            txtDate.Text = issuedDate;
            txtIssued.Text = issuedBy;
            txtSubject.Text = subject;
            fillStudents();
            updateRecords();
            dgRecords.ItemsSource = request;
        }

        private void fillStudents()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT studentNo, fullName from IssuanceList where subject = @subject and lockNo = @lockNo and section = @section and sched = @sched and issuedBy = @issuedBy", conn))
            {
                
                cmd.Parameters.AddWithValue("@section", txtSection.Text);
                cmd.Parameters.AddWithValue("@subject", txtSubject.Text);
                cmd.Parameters.AddWithValue("@sched", txtSched.Text);
                cmd.Parameters.AddWithValue("@issuedBy", txtIssued.Text);
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

                foreach (var student in studInfo)
                {
                    if (string.IsNullOrEmpty(txtStud1.Text))
                    {
                        txtStud1.Text = Convert.ToString(student.studNo);
                        txtName1.Text = student.studName;
                    }
                    else if (string.IsNullOrEmpty(txtStud2.Text))
                    {
                        txtStud2.Text = Convert.ToString(student.studNo);
                        txtName2.Text = student.studName;
                    }
                    else if (string.IsNullOrEmpty(txtStud3.Text))
                    {
                        txtStud3.Text = Convert.ToString(student.studNo);
                        txtName3.Text = student.studName;
                    }
                    else if (string.IsNullOrEmpty(txtStud4.Text))
                    {
                        txtStud4.Text = Convert.ToString(student.studNo);
                        txtName4.Text = student.studName;
                    }
                    else if (string.IsNullOrEmpty(txtStud5.Text))
                    {
                        txtStud5.Text = Convert.ToString(student.studNo);
                        txtName5.Text = student.studName;
                    }
                }
            }
        }

        private void updateRecords()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT il.prodCode, ai.name, ai.manuf, il.qty, il.breakage, ai.size from IssuanceList il INNER JOIN ApparatusInventory ai on il.prodCode = ai.prodCode where il.section = @sect and il.lockNo = @lockNo and il.subject = @subj and il.issuedBy = @issuedBy and il.sched = @sched", conn))
            {
                cmd.Parameters.AddWithValue("@subj", txtSubject.Text);
                cmd.Parameters.AddWithValue("@sched", txtSched.Text);
                cmd.Parameters.AddWithValue("@sect", txtSection.Text);
                cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                cmd.Parameters.AddWithValue("@issuedBy", txtIssued.Text);
                request.Clear();
                using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int nameIndex = reader.GetOrdinal("name");
                            string name = Convert.ToString(reader.GetValue(nameIndex));

                            int manufIndex = reader.GetOrdinal("manuf");
                            string manuf = Convert.ToString(reader.GetValue(manufIndex));

                            int qtyIndex = reader.GetOrdinal("qty");
                            int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                            int sizeIndex = reader.GetOrdinal("size");
                            string size = Convert.ToString(reader.GetValue(sizeIndex));

                            int breakageIndex = reader.GetOrdinal("breakage");
                            bool breakage = Convert.ToBoolean(reader.GetValue(breakageIndex));

                            request.Add(new LVIssuance
                            {
                                breakage = breakage,
                                inventName = name,
                                manuf = manuf,
                                qty = qty,
                                size = size
                            });
                        }
                    }
                    else
                    {
                        //IF NO RECORDS ANYMORE DELETE RECORD
                    }
                }
            }
        }

        private void imgBack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new IssuanceList());
        }
    }
}
