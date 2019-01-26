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
using NLog;
namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for IssuanceRecord.xaml
    /// </summary>
    public partial class IssuanceRecord : Page
    {
        ObservableCollection<LVIssuance> request = new ObservableCollection<LVIssuance>();
        List<StudentInfo> studInfo = new List<StudentInfo>();

        private static Logger Log = LogManager.GetCurrentClassLogger();

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
            request.Clear();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT il.prodCode, ai.name, ai.manuf, il.qty, il.breakage, ai.size from IssuanceList il INNER JOIN ApparatusInventory ai on il.prodCode = ai.prodCode where il.section = @sect and il.lockNo = @lockNo and il.subject = @subj and il.issuedBy = @issuedBy and il.sched = @sched", conn))
            {
                cmd.Parameters.AddWithValue("@subj", txtSubject.Text);
                cmd.Parameters.AddWithValue("@sched", txtSched.Text);
                cmd.Parameters.AddWithValue("@sect", txtSection.Text);
                cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                cmd.Parameters.AddWithValue("@issuedBy", txtIssued.Text);
                using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int prodCodeIndex = reader.GetOrdinal("prodCode");
                            string prodCode = Convert.ToString(reader.GetValue(prodCodeIndex));

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
                                size = size,
                                prodCode = prodCode
                            });
                        }
                    }
                    else
                    {
                        this.NavigationService.Navigate(new IssuanceList());
                    }
                }
            }
        }

        private void imgBack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new IssuanceList());
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Are all fields checked?";
            string sCaption = "Return process";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            switch (dr)
            {
                case MessageBoxResult.Yes:
                    SqlCeConnection conn = DBUtils.GetDBConnection();
                    conn.Open();
                    foreach (var check in request)
                    {
                        int qty = 0;

                        using (SqlCeCommand cmd = new SqlCeCommand("SELECT qty from IssuanceList where lockNo = @lockNo and subject = @subject and prodCode = @prodCode and section = @section and sched = @sched", conn))
                        {
                            cmd.Parameters.AddWithValue("@subject", txtSubject.Text);
                            cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                            cmd.Parameters.AddWithValue("@issuedBy", txtIssued.Text);
                            cmd.Parameters.AddWithValue("@prodCode", check.prodCode);
                            cmd.Parameters.AddWithValue("@sched", txtSched.Text);
                            cmd.Parameters.AddWithValue("@section", txtSection.Text);
                            using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        int qtyIndex = reader.GetOrdinal("qty");
                                        qty = Convert.ToInt32(reader.GetValue(qtyIndex));
                                    }
                                }
                            }
                        }

                        if (check.qty > qty)
                        {
                            MessageBox.Show("Quantity of the product is greater than the old quantity value! Please change the value to be able to proceed");
                            break;
                        }
                        else
                        {
                            if(check.breakage == true)
                            {
                                if(check.qty < qty)
                                {
                                    qty -= check.qty; //working apparatus
                                    using (SqlCeCommand cmd = new SqlCeCommand("UPDATE IssuanceList set breakage = 1, qty = qty - @qty where where section = @sect and sched = @sched and lockNo = @lockNo and subject = @subject and prodCode = @prodCode", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@subject", txtSubject.Text);
                                        cmd.Parameters.AddWithValue("@sched", txtSched.Text);
                                        cmd.Parameters.AddWithValue("@sect", txtSection.Text);
                                        cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                                        cmd.Parameters.AddWithValue("@issuedBy", txtIssued.Text);
                                        cmd.Parameters.AddWithValue("@prodCode", check.prodCode);
                                        cmd.Parameters.AddWithValue("@qty", qty);
                                        try
                                        {
                                            int count = cmd.ExecuteNonQuery();
                                            MessageBox.Show(count.ToString());
                                            if (count > 0)
                                            {
                                                using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE ApparatusInventory set qty = qty + @qty where prodCode = @prodCode", conn))
                                                {
                                                    cmd1.Parameters.AddWithValue("@prodCode", check.prodCode);
                                                    cmd1.Parameters.AddWithValue("@qty", qty);
                                                    try
                                                    {
                                                        cmd1.ExecuteNonQuery();
                                                    }
                                                    catch (SqlCeException ex)
                                                    {
                                                        MessageBox.Show("Error! Log has been updated with the error.");
                                                        Log = LogManager.GetLogger("*");
                                                        Log.Error(ex, "Query Error");
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        catch (SqlCeException ex)
                                        {
                                            MessageBox.Show("Error! Log has been updated with the error.");
                                            Log = LogManager.GetLogger("*");
                                            Log.Error(ex, "Query Error");
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    using (SqlCeCommand cmd = new SqlCeCommand("UPDATE IssuanceList set breakage = 1 where section = @sect and sched = @sched and lockNo = @lockNo and subject = @subject and prodCode = @prodCode", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@subject", txtSubject.Text);
                                        cmd.Parameters.AddWithValue("@sched", txtSched.Text);
                                        cmd.Parameters.AddWithValue("@sect", txtSection.Text);
                                        cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                                        cmd.Parameters.AddWithValue("@issuedBy", txtIssued.Text);
                                        cmd.Parameters.AddWithValue("@prodCode", check.prodCode);
                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            MessageBox.Show("Record has been updated!");
                                        }
                                        catch (SqlCeException ex)
                                        {
                                            MessageBox.Show("Error! Log has been updated with the error.");
                                            Log = LogManager.GetLogger("*");
                                            Log.Error(ex, "Query Error");
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (SqlCeCommand cmd = new SqlCeCommand("INSERT into ArchiveIssuanceList SELECT lockNo, prof, sched, subject, section, issuedDate, issuedBy, fullName, studentNo, prodCode, qty from IssuanceList where lockNo = @lockNo and sched = @sched and subject = @subject and section = @sect and issuedBy = @issuedBy", conn))
                                {
                                    cmd.Parameters.AddWithValue("@subject", txtSubject.Text);
                                    cmd.Parameters.AddWithValue("@sched", txtSched.Text);
                                    cmd.Parameters.AddWithValue("@sect", txtSection.Text);
                                    cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                                    cmd.Parameters.AddWithValue("@issuedBy", txtIssued.Text);

                                    try
                                    {
                                        int count = cmd.ExecuteNonQuery();

                                        if (count > 0)
                                        {
                                            using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE ApparatusInventory set qty = qty + @qty where prodCode = @prodCode", conn))
                                            {
                                                cmd1.Parameters.AddWithValue("@prodCode", check.prodCode);
                                                cmd1.Parameters.AddWithValue("@qty", check.qty);
                                                try
                                                {
                                                    cmd1.ExecuteNonQuery();

                                                    using (SqlCeCommand cmd2 = new SqlCeCommand("DELETE from IssuanceList where lockNo = @lockNo and subject = @subject and section = @sect and sched = @sched and issuedBy = @issuedBy and prodCode = @prodCode", conn))
                                                    {
                                                        cmd2.Parameters.AddWithValue("@subject", txtSubject.Text);
                                                        cmd2.Parameters.AddWithValue("@sched", txtSched.Text);
                                                        cmd2.Parameters.AddWithValue("@sect", txtSection.Text);
                                                        cmd2.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                                                        cmd2.Parameters.AddWithValue("@issuedBy", txtIssued.Text);
                                                        cmd2.Parameters.AddWithValue("@prodCode", check.prodCode);

                                                        try
                                                        {
                                                            cmd2.ExecuteNonQuery();
                                                            Log = LogManager.GetLogger("issuanceFormRecord");
                                                            string newLine = System.Environment.NewLine;
                                                            Log.Info("A Issuance form has been returned with the ff information: " + newLine +
                                                                "Date Requested: " + txtDate.Text + newLine +
                                                                "Subject: " + txtSubject.Text + newLine +
                                                                "Section: " + txtSection.Text + newLine +
                                                                "Schedule: " + txtSched.Text + newLine +
                                                                "Locker No.: " + txtLockNo.Text + newLine +
                                                                "Issued by: " + txtIssued.Text
                                                                );
                                                        }
                                                        catch (SqlCeException ex)
                                                        {
                                                            MessageBox.Show("Error! Log has been updated with the error. " + ex);
                                                        }
                                                    }
                                                }
                                                catch (SqlCeException ex)
                                                {
                                                    MessageBox.Show("Error! Log has been updated with the error. " + ex);
                                                    return;

                                                }
                                            }
                                        }
                                    }
                                    catch (SqlCeException ex)
                                    {
                                        MessageBox.Show("Error! Log has been updated with the error. " + ex);
                                        return;
                                    }

                                }
                            }
                        }
                    }
                    updateRecords();
                    break;

                case MessageBoxResult.No:
                    break;
            }
        }
    }
}
