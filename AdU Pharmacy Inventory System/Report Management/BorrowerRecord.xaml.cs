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
using System.Data.Common;
using NLog;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for BorrowerRecord.xaml
    /// </summary>
    public partial class BorrowerRecord : Page
    {
        ObservableCollection<LVBorrower> borrowers = new ObservableCollection<LVBorrower>();
        List<StudentInfo> studInfo = new List<StudentInfo>();
        private static Logger Log = LogManager.GetCurrentClassLogger();
        string studentNo;
        public BorrowerRecord(string studentNo, string dateReq, string dateExp, string subj, int grpID, string experiment, string lockNo)
        {
            InitializeComponent();
            stack.DataContext = new ExpanderListViewModel();
            this.studentNo = studentNo;
            txtDateReq.Text = dateReq;
            txtDateExp.Text = dateExp;
            txtSubj.Text = subj;
            txtLockNo.Text = lockNo;
            txtExpName.Text = experiment;
            txtGrpID.Text = Convert.ToString(grpID);
            dgRecords.ItemsSource = borrowers;
            updateRecords();
            studentList();
        }

        private void updateRecords()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            borrowers.Clear();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT bl.prodCode, ai.name, ai.manuf, bl.qty, bl.breakage, ai.size from BorrowerList bl INNER JOIN ApparatusInventory ai on bl.prodCode = ai.prodCode where bl.studentNo = @studentNo and bl.groupID = @grpID and bl.lockNo = @lockNo and bl.subject = @subj and bl.expName = @experiment and bl.dateReq = @dateReq and bl.dateExp = @dateExp", conn))
            {
                cmd.Parameters.AddWithValue("@studentNo", studentNo);
                cmd.Parameters.AddWithValue("@subj", txtSubj.Text);
                cmd.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                cmd.Parameters.AddWithValue("@experiment", txtExpName.Text);
                cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                cmd.Parameters.AddWithValue("@dateReq", txtDateReq.Text);
                cmd.Parameters.AddWithValue("@dateExp", txtDateExp.Text);
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

                            borrowers.Add(new LVBorrower
                            {
                                breakage = breakage,
                                prodName = name,
                                manuf = manuf,
                                qty = qty,
                                size = size
                            });
                        }
                    }
                    else
                    {
                        this.NavigationService.Navigate(new BorrowersList());
                    }
                }
            }
        }
        private void studentList()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT studentNo, fullName from BorrowerList where groupID = @groupID and subject = @subject and expName = @expName and DATEDIFF(day, dateReq, @dateReq) = 0 and DATEDIFF(day, dateExp, @dateExp) = 0 and lockNo = @lockNo", conn))
            {
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
                    foreach (var check in borrowers)
                    {
                        string prodCode = "";
                        using (SqlCeCommand cmd = new SqlCeCommand("SELECT prodCode from ApparatusInventory where name = @appaName and manuf = @manuf and size = @size", conn))
                        {
                            cmd.Parameters.AddWithValue("@appaName", check.prodName);
                            cmd.Parameters.AddWithValue("@manuf", check.manuf);
                            cmd.Parameters.AddWithValue("@size", check.size);
                            using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                            {
                                while (reader.Read())
                                {
                                    int prodCodeIndex = reader.GetOrdinal("prodCode");
                                    prodCode = Convert.ToString(reader.GetValue(prodCodeIndex));

                                }
                            }
                        }
                        int qty = 0;
                        using (SqlCeCommand cmd = new SqlCeCommand("SELECT qty from BorrowerList where DATEDIFF(day, dateReq, @dateReq) = 0 and DATEDIFF(day, dateExp, @dateExp) = 0 and groupID = @grpID and lockNo = @lockNo and subject = @subj and expName = @experiment and prodCode = @prodCode", conn))
                        {
                            cmd.Parameters.AddWithValue("@subj", txtSubj.Text);
                            cmd.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                            cmd.Parameters.AddWithValue("@experiment", txtExpName.Text);
                            cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                            cmd.Parameters.AddWithValue("@dateReq", txtDateReq.Text);
                            cmd.Parameters.AddWithValue("@dateExp", txtDateExp.Text);
                            cmd.Parameters.AddWithValue("@prodCode", prodCode);
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
                            if (check.breakage == true)
                            {
                                if (check.qty < qty) //IF THERE ARE SOME BREAKAGES
                                {
                                    qty -= check.qty; //working apparatus
                                    using (SqlCeCommand cmd = new SqlCeCommand("UPDATE BorrowerList set breakage = 1, qty = qty - @qty where dateReq = @dateReq and dateExp = @dateExp and groupID = @grpID and lockNo = @lockNo and subject = @subj and expName = @experiment and prodCode = @prodCode", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@dateReq", txtDateReq.Text);
                                        cmd.Parameters.AddWithValue("@dateExp", txtDateExp.Text);
                                        cmd.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                                        cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                                        cmd.Parameters.AddWithValue("@subj", txtSubj.Text);
                                        cmd.Parameters.AddWithValue("@experiment", txtExpName.Text);
                                        cmd.Parameters.AddWithValue("@manuf", check.manuf);
                                        cmd.Parameters.AddWithValue("@prodCode", prodCode);
                                        cmd.Parameters.AddWithValue("@qty", qty);
                                        try
                                        {
                                            int count = cmd.ExecuteNonQuery();
                                            MessageBox.Show(count.ToString());
                                            if (count > 0)
                                            {
                                                using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE ApparatusInventory set qty = qty + @qty where prodCode = @prodCode", conn))
                                                {
                                                    cmd1.Parameters.AddWithValue("@prodCode", prodCode);
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
                                else //IF ALL QUANTITY OF THE SPECIFIC BORROWED APPARATUS ARE BROKEN
                                {
                                    using (SqlCeCommand cmd = new SqlCeCommand("UPDATE BorrowerList set breakage = 1 where dateReq = @dateReq and dateExp = @dateExp and groupID = @grpID and lockNo = @lockNo and subject = @subj and expName = @experiment and prodCode = @prodCode", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@dateReq", txtDateReq.Text);
                                        cmd.Parameters.AddWithValue("@dateExp", txtDateExp.Text);
                                        cmd.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                                        cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                                        cmd.Parameters.AddWithValue("@subj", txtSubj.Text);
                                        cmd.Parameters.AddWithValue("@experiment", txtExpName.Text);
                                        cmd.Parameters.AddWithValue("@prodCode", prodCode);
                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                            MessageBox.Show("Record has been updated!");
                                        }
                                        catch (SqlCeException ex)
                                        {
                                            MessageBox.Show("Error! Log has been updated with the error. " + ex);
                                            return;

                                        }
                                    }
                                }

                            }
                            else //IF THERE ARE NONE BROKEN
                            {

                                using (SqlCeCommand cmd = new SqlCeCommand("INSERT into ArchiveBorrowerList SELECT dateReq, dateExp, studentNo, fullName, groupID, lockNo, subject, expName, prodCode, qty from BorrowerList where DATEDIFF(day, dateReq, @dateReq) = 0 and DATEDIFF(day, dateExp, @dateExp) = 0 and groupID = @grpID and lockNo = @lockNo and subject = @subj and expName = @experiment and prodCode = @prodCode", conn))
                                {
                                    cmd.Parameters.AddWithValue("@dateReq", txtDateReq.Text);
                                    cmd.Parameters.AddWithValue("@dateExp", txtDateExp.Text);
                                    cmd.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                                    cmd.Parameters.AddWithValue("@lockNo", txtLockNo.Text);
                                    cmd.Parameters.AddWithValue("@subj", txtSubj.Text);
                                    cmd.Parameters.AddWithValue("@experiment", txtExpName.Text);
                                    cmd.Parameters.AddWithValue("@prodCode", prodCode);
                                    try
                                    {
                                        int count = cmd.ExecuteNonQuery();

                                        if (count > 0)
                                        {
                                            using (SqlCeCommand cmd1 = new SqlCeCommand("UPDATE ApparatusInventory set qty = qty + @qty where prodCode = @prodCode", conn))
                                            {
                                                cmd1.Parameters.AddWithValue("@prodCode", prodCode);
                                                cmd1.Parameters.AddWithValue("@qty", check.qty);
                                                try
                                                {
                                                    cmd1.ExecuteNonQuery();

                                                    using (SqlCeCommand cmd2 = new SqlCeCommand("DELETE from BorrowerList where groupID = @grpID and subject = @subjName and expName = @experiment and DATEDIFF(day, dateReq, @dateReq) = 0 and DATEDIFF(day, dateExp, @dateExp) = 0 and prodCode = @prodCode", conn))
                                                    {
                                                        cmd2.Parameters.AddWithValue("@grpID", txtGrpID.Text);
                                                        cmd2.Parameters.AddWithValue("@subjName", txtSubj.Text);
                                                        cmd2.Parameters.AddWithValue("@experiment", txtExpName.Text);
                                                        cmd2.Parameters.AddWithValue("@dateReq", txtDateReq.Text);
                                                        cmd2.Parameters.AddWithValue("@dateExp", txtDateExp.Text);
                                                        cmd2.Parameters.AddWithValue("@prodCode", prodCode);

                                                        try
                                                        {
                                                            count = cmd2.ExecuteNonQuery();
                                                            Log = LogManager.GetLogger("borrowerFormRecord");
                                                            string newLine = System.Environment.NewLine;
                                                            Log.Info("A Borrower form has been returned with the following information: " + newLine +
                                                                "Date Requested: " + txtDateReq.Text + newLine +
                                                                "Date of Experiment" + txtDateExp.Text + newLine +
                                                                "Subject: " + txtSubj.Text + newLine +
                                                                "Experiment Title: " + txtExpName.Text + newLine +
                                                                "Locker No.: " + txtLockNo.Text + newLine +
                                                                "Group No: " + txtGrpID.Text
                                                                );
                                                        }
                                                        catch (SqlCeException ex)
                                                        {
                                                            MessageBox.Show("Error! Log has been updated with the error.");
                                                            Log = LogManager.GetLogger("*");
                                                            Log.Error(ex, "Query Error");
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

        private void imgBack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new BorrowersList());
        }

    }
}
