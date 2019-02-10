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
using System.Data.SqlClient;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using NLog;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for ApparatusStockOut.xaml
    /// </summary>
    public partial class ApparatusStockOut : Page
    {
        int i = 1;
        List<StudentInfo> studInfo = new List<StudentInfo>();
        ObservableCollection<LVApparatusStockOut> stockOut = new ObservableCollection<LVApparatusStockOut>();
        string user = Environment.UserName;
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public ApparatusStockOut()
        {
            InitializeComponent();
            txtDate.Text = DateTime.Now.ToString("dd MMMM yyyy");
            stack.DataContext = new ExpanderListViewModel();
            lvAppaStockOut.ItemsSource = stockOut;
            fillInventory();
            fillSubjects();
        }

        private void fillInventory()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT name from ApparatusInventory", conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        cmbInventName.Items.Clear();
                        while (reader.Read())
                        {
                            string appaName = reader["name"].ToString();
                            cmbInventName.Items.Add(appaName);
                        }
                    }
                }
            }
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
        private void fillSize()
        {
            if (!string.IsNullOrEmpty(cmbInventName.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT size from ApparatusInventory where name = @inventName and size IS NOT NULL", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbSize.Items.Clear();
                            while (reader.Read())
                            {
                                int sizeIndex = reader.GetOrdinal("size");
                                string size = Convert.ToString(reader.GetValue(sizeIndex));
                                cmbSize.Items.Add(size);
                            }
                        }
                    }
                }
            }
        }
        private void fillManufacturer()
        {
            if (!string.IsNullOrEmpty(cmbSize.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT manuf from ApparatusInventory where name = @inventName and size = @size", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    cmd.Parameters.AddWithValue("@size", cmbSize.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            cmbManuf.Items.Clear();
                            while (reader.Read())
                            {
                                int manufIndex = reader.GetOrdinal("manuf");
                                string manuf = Convert.ToString(reader.GetValue(manufIndex));

                                cmbManuf.Items.Add(manuf);
                            }
                        }
                    }
                }
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                cmbManuf.Items.Clear();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT manuf from ApparatusInventory where name = @inventName", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int manufIndex = reader.GetOrdinal("manuf");
                                string manuf = Convert.ToString(reader.GetValue(manufIndex));
                                cmbManuf.Items.Add(manuf);
                            }
                        }
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbInventName.Text) || string.IsNullOrEmpty(txtQty.Text) || string.IsNullOrEmpty(cmbManuf.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (cmbInventName.Text.Length > 0 && cmbSize.Items.Count > 0 && string.IsNullOrEmpty(cmbSize.Text))
            {
                MessageBox.Show("Please select size!");
                cmbSize.Focus();
            }
            else if (!string.IsNullOrEmpty(cmbSize.Text) && !string.IsNullOrEmpty(cmbInventName.Text) && !string.IsNullOrEmpty(cmbManuf.Text) && !string.IsNullOrEmpty(txtQty.Text))
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from ApparatusInventory where name = @inventName and manuf = @manuf and size = @size", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    cmd.Parameters.AddWithValue("@manuf", cmbManuf.Text);
                    cmd.Parameters.AddWithValue("@size", cmbSize.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        reader.Read();
                        int qtyIndex = reader.GetOrdinal("qty");
                        int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                        int manufIndex = reader.GetOrdinal("manuf");
                        string manuf = Convert.ToString(reader.GetValue(manufIndex));

                        int sizeIndex = reader.GetOrdinal("size");
                        string size = Convert.ToString(reader.GetValue(sizeIndex));

                        int reqQty = Convert.ToInt32(txtQty.Text);
                        if (reqQty > qty)
                        {
                            MessageBox.Show("Requested quantity cannot be greater than the available quantity!");
                            return;
                        }
                        else
                        {
                            var found = stockOut.FirstOrDefault(x => (x.inventName == cmbInventName.Text) && (x.manuf == cmbManuf.Text) && ((x.size == cmbSize.Text) || (x.size == null)));
                            if (found != null)
                            {
                                if (found.qty + reqQty > qty)
                                {
                                    MessageBox.Show("Requested quantity cannot be greater than the available quantity!");
                                    return;
                                }
                                else
                                {
                                    found.qty = found.qty + reqQty;
                                }
                            }
                            else
                            {
                                stockOut.Add(new LVApparatusStockOut
                                {
                                    i = i,
                                    inventName = cmbInventName.Text,
                                    qty = reqQty,
                                    manuf = manuf,
                                    size = size
                                });
                                i++;
                            }
                            emptyFields();
                        }

                    }
                }
            }
            else
            {
                SqlCeConnection conn = DBUtils.GetDBConnection();
                conn.Open();
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from ApparatusInventory where name = @inventName and manuf = @manuf", conn))
                {
                    cmd.Parameters.AddWithValue("@inventName", cmbInventName.Text);
                    cmd.Parameters.AddWithValue("@manuf", cmbManuf.Text);
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        reader.Read();
                        int qtyIndex = reader.GetOrdinal("qty");
                        int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                        int manufIndex = reader.GetOrdinal("manuf");
                        string manuf = Convert.ToString(reader.GetValue(manufIndex));

                        string size = null;

                        int reqQty = Convert.ToInt32(txtQty.Text);
                        if (reqQty > qty)
                        {
                            MessageBox.Show("Requested quantity cannot be greater than the available quantity!");
                            return;
                        }
                        else
                        {
                            var found = stockOut.FirstOrDefault(x => (x.inventName == cmbInventName.Text) && (x.manuf == cmbManuf.Text) && (x.size == cmbSize.Text));
                            if (found != null)
                            {
                                if (found.qty + reqQty > qty)
                                {
                                    MessageBox.Show("Requested quantity cannot be greater than the available quantity!");
                                    return;
                                }
                                else
                                {
                                    found.qty = found.qty + reqQty;
                                }
                            }
                            else
                            {
                                stockOut.Add(new LVApparatusStockOut
                                {
                                    i = i,
                                    inventName = cmbInventName.Text,
                                    qty = reqQty,
                                    manuf = manuf,
                                    size = size
                                });
                                i++;
                            }
                            emptyFields();
                        }

                    }
                }
            }
        }
        private void btnStockOut_Click(object sender, RoutedEventArgs e)
        {
            if (lvAppaStockOut.Items.Count == 0)
            {
                MessageBox.Show("There are no apparatus(es) to be stock out");
            }
            else if (string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtGroup.Text) || string.IsNullOrEmpty(cmbSubject.Text) || string.IsNullOrEmpty(txtExperiment.Text) || string.IsNullOrEmpty(txtLocker.Text) || string.IsNullOrEmpty(txtDateExp.Text))
            {
                MessageBox.Show("One or more fields are empty!");
            }
            else if (string.IsNullOrEmpty(txtStud1.Text) || String.IsNullOrEmpty(txtName1.Text))
            {
                MessageBox.Show("Student Info fields are empty!");
            }
            else
            {

                string sMessageBoxText = "Are all fields correct?";
                string sCaption = "Stock Out";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:

                        SqlCeConnection conn = DBUtils.GetDBConnection();
                        conn.Open();
                        bool check = false;

                        studInfo.Add(new StudentInfo
                        {
                            studName = txtName1.Text,
                            studNo = txtStud1.Text
                        });

                        if (string.IsNullOrEmpty(txtStud2.Text) && txtName2.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtStud2.Focus();
                            return;
                        }
                        else if (string.IsNullOrEmpty(txtName2.Text) && txtStud2.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtName2.Focus();
                            return;
                        }
                        else if (txtName2.Text.Length > 0 && txtStud2.Text.Length > 0)
                        {
                            studInfo.Add(new StudentInfo
                            {
                                studName = txtName2.Text,
                                studNo = txtStud2.Text
                            });
                        }

                        if (string.IsNullOrEmpty(txtStud3.Text) && txtName3.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtStud3.Focus();
                            return;
                        }
                        else if (string.IsNullOrEmpty(txtName3.Text) && txtStud3.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtName3.Focus();
                            return;
                        }
                        else if (txtName3.Text.Length > 0 && txtStud3.Text.Length > 0)
                        {
                            studInfo.Add(new StudentInfo
                            {
                                studName = txtName3.Text,
                                studNo = txtStud3.Text
                            });
                        }

                        if (string.IsNullOrEmpty(txtStud4.Text) && txtName4.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtStud4.Focus();
                            return;
                        }
                        else if (string.IsNullOrEmpty(txtName4.Text) && txtStud4.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtName4.Focus();
                            return;
                        }
                        else if (txtName4.Text.Length > 0 && txtStud4.Text.Length > 0)
                        {
                            studInfo.Add(new StudentInfo
                            {
                                studName = txtName4.Text,
                                studNo = txtStud4.Text
                            });
                        }


                        string date = txtDate.Text.Replace("/", "-");
                        string filename = @"C:\Users\" + user + @"\Desktop\[" + date + "][" + cmbSubject.Text + "][" + txtExperiment.Text + "][" + txtGroup.Text + "][" + txtLocker.Text + "].docx";
                        filename = filename.Replace(" ", "-");


                        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filename, WordprocessingDocumentType.Document)){

                            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                            mainPart.Document = new Document();
                            Body body = new Body();

                            PageMargin pageMargin = new PageMargin() { Top = 720, Right = 720U, Bottom = 720 };

                            SectionProperties sectionProps = new SectionProperties();
                            sectionProps.Append(pageMargin);
                            body.Append(sectionProps);


                            DocumentFormat.OpenXml.Wordprocessing.Paragraph para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
                            DocumentFormat.OpenXml.Wordprocessing.Run run = new DocumentFormat.OpenXml.Wordprocessing.Run();

                            Text text = new Text("The Quick Brown Fox Jumps Over The Lazy Dog");

                            run.Append(text);
                            para.Append(run);
                            body.Append(para);
                            mainPart.Document.Append(body );
                        }

                        Process.Start("WINWORD.EXE", filename);


                        //foreach (var row in stockOut)
                        //{
                        //    using (SqlCeCommand cmd = new SqlCeCommand("UPDATE ApparatusInventory set qty = qty - @qty where name = @inventName and  (size IS null or size = @size) and manuf = @manuf", conn))
                        //    {
                        //        cmd.Parameters.AddWithValue("@qty", row.qty);
                        //        cmd.Parameters.AddWithValue("@inventName", row.inventName);
                        //        cmd.Parameters.AddWithValue("@manuf", row.manuf);
                        //        if (!string.IsNullOrEmpty(row.size))
                        //        {
                        //            cmd.Parameters.AddWithValue("@size", row.size);
                        //        }
                        //        else
                        //        {
                        //            row.size = "";
                        //            cmd.Parameters.AddWithValue("@size", row.size);
                        //        }
                        //        try
                        //        {
                        //            cmd.ExecuteNonQuery();
                        //            check = true;
                        //            int ordinal = 0;
                        //            string prodCode = null;
                        //            using (SqlCeCommand cmd2 = new SqlCeCommand("SELECT prodCode from ApparatusInventory where name = @inventName and manuf = @manuf", conn))
                        //            {
                        //                cmd2.Parameters.AddWithValue("@inventName", row.inventName);
                        //                cmd2.Parameters.AddWithValue("@manuf", row.manuf);
                        //                DbDataReader result = cmd2.ExecuteResultSet(ResultSetOptions.Scrollable);
                        //                if (result.Read())
                        //                {
                        //                    ordinal = result.GetOrdinal("prodCode");
                        //                    prodCode = Convert.ToString(result.GetValue(ordinal));
                        //                }

                        //            }
                        //            foreach (var student in studInfo)
                        //            {
                        //                using (SqlCeCommand cmd1 = new SqlCeCommand("INSERT into BorrowerList (dateReq, dateExp, studentNo, fullName, groupID, lockNo ,subject, expName ,prodCode, qty, breakage) VALUES (@dateReq, @dateExp, @studentNo, @fullName, @groupID, @lockNo, @subject, @expName ,@prodCode, @qty, 0)", conn))
                        //                {
                        //                    cmd1.Parameters.AddWithValue("@dateReq", txtDate.Text);
                        //                    cmd1.Parameters.AddWithValue("@dateExp", txtDateExp.Text);
                        //                    cmd1.Parameters.AddWithValue("@studentNo", student.studNo);
                        //                    cmd1.Parameters.AddWithValue("@fullName", student.studName);
                        //                    cmd1.Parameters.AddWithValue("@groupID", txtGroup.Text);
                        //                    cmd1.Parameters.AddWithValue("@subject", cmbSubject.Text);
                        //                    cmd1.Parameters.AddWithValue("@lockNo", txtLocker.Text);
                        //                    cmd1.Parameters.AddWithValue("@expName", txtExperiment.Text);
                        //                    cmd1.Parameters.AddWithValue("@prodCode", prodCode);
                        //                    cmd1.Parameters.AddWithValue("@qty", row.qty);
                        //                    try
                        //                    {
                        //                        cmd1.ExecuteNonQuery();
                        //                    }
                        //                    catch (SqlCeException ex)
                        //                    {
                        //                        MessageBox.Show("Error! Log has been updated with the error.");
                        //                        Log = LogManager.GetLogger("*");
                        //                        Log.Error(ex, "Query Error");
                        //                    }
                        //                }
                        //            }

                        //        }
                        //        catch (SqlCeException ex)
                        //        {
                        //            MessageBox.Show("Error! Log has been updated with the error.");
                        //            Log = LogManager.GetLogger("*");
                        //            Log.Error(ex, "Query Error");
                        //        }


                        //    }

                        //}

                        if (check == true)
                        {
                            MessageBox.Show("Stock Out Successfully");
                            Log = LogManager.GetLogger("generateBorrowerForm");
                            string newLine = System.Environment.NewLine;
                            Log.Info("A Borrower form has been generated with the following details: " + newLine +
                                "Date Requested: " + txtDate.Text + newLine +
                                "Date of Experiment" + txtDateExp.Text + newLine +
                                "Subject: " + cmbSubject.Text + newLine +
                                "Experiment Title: " + txtExperiment.Text + newLine +
                                "Locker No.: " + txtLocker.Text + newLine +
                                "Group No: " + txtGroup.Text
                                );
                        }


                        studInfo.Clear();
                        stockOut.Clear();
                        i = 1;
                        emptyFields();
                        break;
                    case MessageBoxResult.No: break;

                }

            }
        }


        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void emptyFields()
        {
            cmbInventName.SelectedIndex = -1;
            cmbManuf.SelectedIndex = -1;
            cmbManuf.Items.Clear();
            cmbSize.SelectedIndex = -1;
            cmbSize.Items.Clear();
            txtQty.Text = null;

            txtDate.Text = DateTime.Now.ToString("dd MMMM yyyy");
            txtGroup.Text = null;
            txtDateExp.Text = null;
            txtExperiment.Text = null;
            txtLocker.Text = null;
            cmbSubject.SelectedIndex = -1;

            txtStud1.Text = null;
            txtStud2.Text = null;
            txtStud3.Text = null;
            txtStud4.Text = null;
            txtName1.Text = null;
            txtName2.Text = null;
            txtName3.Text = null;
            txtName4.Text = null;

        }

        private void txtInventName_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmbSize.Items.Clear();
            cmbManuf.Items.Clear();
            fillSize();
            if (cmbSize.Items.Count == 0)
            {
                fillManufacturer();
            }
        }

        private void txtSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            fillManufacturer();
        }



        private void PackIconMaterial_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            emptyFields();

            i = 1;
            studInfo.Clear();
            stockOut.Clear();
        }

        private void txtDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime dateToday = DateTime.Now.Date;
            DateTime? dateReq = txtDate.SelectedDate;
            DateTime? dateExp = txtDateExp.SelectedDate;

            if (dateReq < dateToday)
            {
                MessageBox.Show("Date request should be less than date today!");
                txtDate.SelectedDate = dateToday;
            }
            if (dateExp < dateToday)
            {
                MessageBox.Show("Date of experiment should be less than date today!");
                txtDateExp.SelectedDate = dateToday;

            }
        }

        private float InchesToPoints(float fInches)
        {
            return fInches * 72.0f;
        }

        private string returnCount(int underlineCount, string text)
        {
            string underline = "";
            for (i = 0; i < underlineCount - text.Length; i++)
            {
                underline = underline.Insert(i, "_");
            }
            return underline;
        }
    }

}
