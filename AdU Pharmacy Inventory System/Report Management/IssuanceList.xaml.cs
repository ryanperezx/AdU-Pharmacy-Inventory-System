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
using System.Data.Common;
using System.Collections.ObjectModel;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for IssuanceList.xaml
    /// </summary>
    public partial class IssuanceList : Page
    {
        ObservableCollection<LVIssuance> list = new ObservableCollection<LVIssuance>();

        public IssuanceList()
        {
            InitializeComponent();
            fillSubjects();
        }

        private void fillSubjects()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT subjName from Subjects", conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        cmbSubject.Items.Clear();
                        while (reader.Read())
                        {
                            string subject = reader["subjName"].ToString();
                            cmbSubject.Items.Add(subject);
                        }
                    }
                }
            }
        }

        private ObservableCollection<LVIssuance> LoadCollectionData()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT lockNo, prof, sched, section, issuedDate, issuedBy from IssuanceList where subject = @subjName", conn))
            {
                cmd.Parameters.AddWithValue("@subjName", cmbSubject.Text);
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        list.Clear();
                        while (reader.Read())
                        {
                            int lockNoIndex = reader.GetOrdinal("lockNo");
                            string lockNo = Convert.ToString(reader.GetValue(lockNoIndex));

                            int profIndex = reader.GetOrdinal("prof");
                            string prof = Convert.ToString(reader.GetValue(profIndex));

                            int schedIndex = reader.GetOrdinal("sched");
                            string sched = Convert.ToString(reader.GetValue(schedIndex));

                            int sectionIndex = reader.GetOrdinal("section");
                            string section = Convert.ToString(reader.GetValue(sectionIndex));

                            int issuedDateIndex = reader.GetOrdinal("issuedDate");
                            DateTime temp = reader.GetDateTime(issuedDateIndex);
                            string issuedDate = temp.ToString("MM/dd/yyyy");

                            int issuedByIndex = reader.GetOrdinal("issuedBy");
                            string issuedBy = Convert.ToString(reader.GetValue(issuedByIndex));

                            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT TOP 1 studentNo, fullName from IssuanceList where section = @section and subject = @subject and prof = @prof and lockNo = @lockNo and DATEDIFF(day, issuedDate, @issuedDate) = 0 and sched = @sched", conn))
                            {
                                cmd1.Parameters.AddWithValue("@prof", prof);
                                cmd1.Parameters.AddWithValue("@lockNo", lockNo);
                                cmd1.Parameters.AddWithValue("@subject", cmbSubject.Text);
                                cmd1.Parameters.AddWithValue("@issuedDate", issuedDate);
                                cmd1.Parameters.AddWithValue("@section", section);
                                cmd1.Parameters.AddWithValue("@sched", sched);
                                using (DbDataReader rd = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    rd.Read();
                                    int studentNoIndex = rd.GetOrdinal("studentNo");
                                    string studentNo = Convert.ToString(rd.GetValue(studentNoIndex));

                                    int fullNameIndex = rd.GetOrdinal("fullName");
                                    string fullName = Convert.ToString(rd.GetValue(fullNameIndex));

                                    list.Add(new LVIssuance
                                    {
                                        lockNo = lockNo,
                                        sect = section,
                                        sched = sched,
                                        issuedDate = issuedDate,
                                        issuedBy = issuedBy,
                                        fullName = fullName,
                                        studentNo = studentNo
                                    });
                                }
                            }
                        }
                    }
                }
            }
                return list;
        }

        private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LVIssuance student = dgList.SelectedItem as LVIssuance;
            this.NavigationService.Navigate(new IssuanceRecord(student.lockNo, student.sect, student.sched, student.issuedDate, student.issuedBy, cmbSubject.Text));
        }

        private void txtSubject_TextChanged(object sender, TextChangedEventArgs e)
        {
            dgList.ItemsSource = LoadCollectionData();
        }
    }
}
