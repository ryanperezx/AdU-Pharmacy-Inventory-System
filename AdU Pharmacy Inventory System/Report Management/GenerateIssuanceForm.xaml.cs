using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlServerCe;
using System.Data.Common;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Xceed.Words.NET;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for GenerateIssuanceForm.xaml
    /// </summary>
    public partial class GenerateIssuanceForm : System.Windows.Controls.Page
    {
        ObservableCollection<LVIssuance> items = new ObservableCollection<LVIssuance>();
        List<StudentInfo> studInfo = new List<StudentInfo>();

        int i = 1;
        public GenerateIssuanceForm(string fullName)
        {
            InitializeComponent();
            fillSubjects();
            txtIssued.Text = fullName;
            stack.DataContext = new ExpanderListViewModel();

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
                        cmbSubj.Items.Clear();
                        while (reader.Read())
                        {
                            string subjName = reader["subjName"].ToString();
                            cmbSubj.Items.Add(subjName);
                        }
                    }
                }
            }
        }
        private void txtSubject_TextChanged(object sender, TextChangedEventArgs e)
        {
            dgSubject.ItemsSource = LoadCollectionData();
        }

        private ObservableCollection<LVIssuance> LoadCollectionData()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            string name = "", size = "", manuf = "";
            conn.Open();
            items.Clear();

            using (SqlCeCommand cmd = new SqlCeCommand("SELECT sub.qty, sub.prodCode, ai.name, ai.size from Subjects sub INNER JOIN ApparatusInventory ai on sub.prodCode = ai.prodCode where sub.subjName = @subjName", conn))
            {
                cmd.Parameters.AddWithValue("@subjName", txtSubject.Text);
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int prodCodeIndex = reader.GetOrdinal("prodCode");
                            string prodCode = Convert.ToString(reader.GetValue(prodCodeIndex));

                            int qtyIndex = reader.GetOrdinal("qty");
                            int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                            int nameIndex = reader.GetOrdinal("name");
                            name = Convert.ToString(reader.GetValue(nameIndex));

                            int sizeIndex = reader.GetOrdinal("size");
                            size = Convert.ToString(reader.GetValue(sizeIndex));


                            List<string> manufacturer = new List<string>();
                            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT manuf from ApparatusInventory where name = @name", conn))
                            {
                                cmd1.Parameters.AddWithValue("@name", name);
                                using (DbDataReader dr = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    if (dr.HasRows)
                                    {
                                        while (dr.Read())
                                        {
                                            int manufIndex = dr.GetOrdinal("manuf");
                                            manuf = Convert.ToString(dr.GetValue(manufIndex));

                                            manufacturer.Add(manuf);
                                        }
                                    }
                                }
                            }
                            int countQty;

                            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT qty, remarks from ApparatusInventory where prodCode = @prodCode", conn))
                            {
                                cmd1.Parameters.AddWithValue("@prodCode", prodCode);
                                using (DbDataReader dr = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    if (dr.HasRows)
                                    {
                                        while (dr.Read())
                                        {
                                            int countQtyIndex = dr.GetOrdinal("qty");
                                            countQty = Convert.ToInt32(dr.GetValue(countQtyIndex));

                                            int remarksIndex = dr.GetOrdinal("remarks");
                                            string remarks = Convert.ToString(dr.GetValue(remarksIndex));
                                            if (!string.IsNullOrEmpty(remarks))
                                            {
                                                remarks = "/" + remarks;
                                            }
                                            if (qty > countQty)
                                            {
                                                MessageBox.Show("Item: " + name + "has low stocks, please stock in as soon as possible!");
                                                items.Add(new LVIssuance()
                                                {
                                                    i = i,
                                                    inventName = name,
                                                    manufList = manufacturer,
                                                    manuf = manuf,
                                                    size = size + remarks,
                                                    qty = countQty,
                                                    prodCode = prodCode
                                                });
                                            }
                                            else
                                            {
                                                items.Add(new LVIssuance()
                                                {
                                                    i = i,
                                                    inventName = name,
                                                    manufList = manufacturer,
                                                    manuf = manuf,
                                                    size = size + remarks,
                                                    qty = qty,
                                                    prodCode = prodCode
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            i++;
                        }
                    }
                }
            }
            return items;
        }

        private void PackIconMaterial_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new GenerateIssuanceForm(txtIssued.Text));
        }

        private void btnGenForm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbSubj.Text) || string.IsNullOrEmpty(txtLock.Text) || string.IsNullOrEmpty(txtIssued.Text) || string.IsNullOrEmpty(txtDate.Text) || string.IsNullOrEmpty(txtProf.Text) || string.IsNullOrEmpty(txtSched.Text) || string.IsNullOrEmpty(txtStud1.Text) || string.IsNullOrEmpty(txtName1.Text))
            {
                MessageBox.Show("Fill in the missing fields");
            }
            else
            {
                string sMessageBoxText = "Are all fields accounted form?";
                string sCaption = "Generate issuance form";
                MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                switch (dr)
                {
                    case MessageBoxResult.Yes:
                        if (string.IsNullOrEmpty(txtStud2.Text) && txtName2.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtStud2.Focus();
                        }
                        else if (string.IsNullOrEmpty(txtName2.Text) && txtStud2.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtName2.Focus();
                        }
                        else if (txtName2.Text.Length > 0 && txtStud2.Text.Length > 0)
                        {
                            studInfo.Add(new StudentInfo
                            {
                                studName = txtName2.Text,
                                studNo = Convert.ToInt32(txtStud2.Text)
                            });
                        }

                        if (string.IsNullOrEmpty(txtStud3.Text) && txtName3.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtStud3.Focus();
                        }
                        else if (string.IsNullOrEmpty(txtName3.Text) && txtStud3.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtName3.Focus();
                        }
                        else if (txtName3.Text.Length > 0 && txtStud3.Text.Length > 0)
                        {
                            studInfo.Add(new StudentInfo
                            {
                                studName = txtName3.Text,
                                studNo = Convert.ToInt32(txtStud3.Text)
                            });
                        }

                        if (string.IsNullOrEmpty(txtStud4.Text) && txtName4.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtStud4.Focus();
                        }
                        else if (string.IsNullOrEmpty(txtName4.Text) && txtStud4.Text.Length > 0)
                        {
                            MessageBox.Show("Please fill up the missing student field!");
                            txtName4.Focus();
                        }
                        else if (txtName4.Text.Length > 0 && txtStud4.Text.Length > 0)
                        {
                            studInfo.Add(new StudentInfo
                            {
                                studName = txtName4.Text,
                                studNo = Convert.ToInt32(txtStud4.Text)
                            });
                        }

                        string user = Environment.UserName;
                        string date = txtDate.Text.Replace("/", "-");
                        string filename = @"C:\Users\" + user + @"\Desktop\[" + date + "][" + txtLock.Text + "][" + cmbSubj.Text + "][" + txtSect.Text + "].docx";
                        filename = filename.Replace(" ", "-");
                        
                        using (DocX document = DocX.Create(filename))
                        {
                            string underline = "";

                            var image = document.AddImage(@"resources/adulogo-blue.png");
                            var picture = image.CreatePicture(50, 200);

                            var p0 = document.InsertParagraph();
                            p0.AppendPicture(picture);

                            var p1 = document.InsertParagraph("ISSUANCE FORM").Bold().FontSize(9)
                            .Alignment = Alignment.right;

                            var p2 = document.InsertParagraph("PHARMACY LABORATORY").Bold().FontSize(7)
                            .UnderlineColor(System.Drawing.Color.Black).SpacingAfter(15).Alignment = Alignment.right;


                            var t0 = document.AddTable(2, 1);
                            t0.Design = TableDesign.None;
                            t0.Alignment = Alignment.right;

                            underline = returnCount(12, txtLock.Text);

                            t0.Rows[0].Cells[0].Paragraphs[0].Bold().Append("________").Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.center;
                            t0.Rows[0].Cells[0].Paragraphs[0].Bold().Append(txtLock.Text + underline).Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.center;
                            underline = "";
                            t0.Rows[1].Cells[0].Paragraphs[0].Append("LOCKER NUMBER").Bold().Alignment = Alignment.center;

                            document.InsertTable(t0);

                            document.InsertParagraph();

                            var t1 = document.AddTable(2, 3);
                            t1.Design = TableDesign.None;
                            t1.AutoFit = AutoFit.Window;

                            foreach (Row row in t1.Rows)
                            {
                                row.Cells[0].Width = 200;
                                row.Cells[1].Width = 200;
                                row.Cells[2].Width = 200;

                            }

                            t1.Rows[1].Cells[0].Paragraphs[0].Append("PROFESSOR").Bold().Alignment = Alignment.center;
                            t1.Rows[0].Cells[0].Paragraphs[0].Append("___").FontSize(10).Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.center;
                            t1.Rows[1].Cells[1].Paragraphs[0].Append("SCHEDULE").FontSize(10).Bold().Alignment = Alignment.center;
                            t1.Rows[0].Cells[1].Paragraphs[0].Append("_____").Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.center;
                            t1.Rows[1].Cells[2].Paragraphs[0].Append("SUBJECT AND SECTION").Bold().Alignment = Alignment.center;
                            t1.Rows[0].Cells[2].Paragraphs[0].Append("__").FontSize(10).Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.center;

                            underline = returnCount(20, txtProf.Text);

                            t1.Rows[0].Cells[0].Paragraphs[0].Append(txtProf.Text + underline).FontSize(10).Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.center;
                            underline = "";

                            underline = returnCount(20, txtSched.Text);

                            t1.Rows[0].Cells[1].Paragraphs[0].Append(txtSched.Text + underline).FontSize(10).Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.center;
                            underline = "";

                            underline = returnCount(22, txtSubject.Text + " " + txtSect.Text);

                            t1.Rows[0].Cells[2].Paragraphs[0].Append(txtSubject.Text + " " + txtSect.Text + underline).FontSize(10).Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.center;
                            underline = "";
                            document.InsertTable(t1);

                            document.InsertParagraph(); //newline

                            var t2 = document.AddTable(26, 6);
                            t2.Design = TableDesign.TableGrid;
                            t2.AutoFit = AutoFit.Window;

                            foreach (Row row in t2.Rows)
                            {
                                row.Cells[0].Width = 30;
                                row.Cells[1].Width = 120;
                                row.Cells[2].Width = 200;
                                row.Cells[3].Width = 40;
                                row.Cells[4].Width = 80;
                                row.Cells[5].Width = 50;
                            }

                            t2.Rows[0].Cells[0].Paragraphs[0].Append("QTY").Bold().Alignment = Alignment.center;
                            t2.Rows[0].Cells[1].Paragraphs[0].Append("APPARATUS").Bold().Alignment = Alignment.center;
                            t2.Rows[0].Cells[2].Paragraphs[0].Append("SIZE / BRAND / REMARKS").Bold().Alignment = Alignment.center;
                            t2.Rows[0].Cells[3].Paragraphs[0].Append("RTN").Bold().Alignment = Alignment.center;
                            t2.Rows[0].Cells[3].Paragraphs[0].AppendLine("CHK").Bold().Alignment = Alignment.center;
                            t2.Rows[0].Cells[4].Paragraphs[0].Append("BREAKAGES").Bold().Alignment = Alignment.center;
                            t2.Rows[0].Cells[5].Paragraphs[0].Append("AMOUNT").Bold().Alignment = Alignment.center;
                            t2.Rows[0].Cells[5].Paragraphs[0].AppendLine("CHARGE").Bold().Alignment = Alignment.center;

                            //INSERTION OF ITEMS REQUESTED TO DATABASE AND WORD FILE
                            SqlCeConnection conn = DBUtils.GetDBConnection();
                            conn.Open();

                            int rowx = 1;
                            foreach (var items in items)
                            {
                                if (string.IsNullOrEmpty(items.manuf))
                                {
                                    MessageBox.Show("One or more manufacturing fields are empty, please fill all out.");
                                    return;
                                }
                                else
                                {
                                    t2.Rows[rowx].Cells[0].Paragraphs[0].Append(Convert.ToString(items.qty)).Bold().Alignment = Alignment.center;
                                    t2.Rows[rowx].Cells[1].Paragraphs[0].Append(items.inventName).Bold().Alignment = Alignment.center;
                                    if (!string.IsNullOrEmpty(items.size))
                                    {
                                        t2.Rows[rowx].Cells[2].Paragraphs[0].Append(items.size + "/" + items.manuf).Bold().Alignment = Alignment.center;
                                    }
                                    else
                                    {
                                        t2.Rows[rowx].Cells[2].Paragraphs[0].Append(items.manuf).Bold().Alignment = Alignment.center;
                                    }

                                    rowx++;
                                    foreach (var student in studInfo)
                                    {
                                        using (SqlCeCommand cmd = new SqlCeCommand("INSERT into IssuanceList (lockNo, prof, sched, subject, section, issuedDate, issuedBy, fullName, studentNo, prodCode, qty, breakage) VALUES (@lockNo, @prof, @sched, @subject, @section, @issuedDate, @issuedBy, @fullName, @studentNo, @prodCode, @qty, 0)", conn))
                                        {
                                            cmd.Parameters.AddWithValue("@lockNo", txtLock.Text);
                                            cmd.Parameters.AddWithValue("@prof", txtProf.Text);
                                            cmd.Parameters.AddWithValue("@sched", txtSched.Text);
                                            cmd.Parameters.AddWithValue("@subject", txtSubject.Text);
                                            cmd.Parameters.AddWithValue("@section", txtSect.Text);
                                            cmd.Parameters.AddWithValue("@issuedDate", txtDate.Text);
                                            cmd.Parameters.AddWithValue("@issuedBy", txtIssued.Text);
                                            cmd.Parameters.AddWithValue("@fullName", student.studName);
                                            cmd.Parameters.AddWithValue("@studentNo", student.studNo);
                                            cmd.Parameters.AddWithValue("@prodCode", items.prodCode);
                                            cmd.Parameters.AddWithValue("@qty", items.qty);
                                            try
                                            {
                                                cmd.ExecuteNonQuery();
                                            }
                                            catch (SqlCeException ex)
                                            {
                                                MessageBox.Show("Error! Log has been updated with the error!");
                                            }
                                        }
                                    }
                                    using (SqlCeCommand cmd = new SqlCeCommand("UPDATE ApparatusInventory set qty = qty - @qty where prodCode = @prodCode", conn))
                                    {
                                        cmd.Parameters.AddWithValue("@qty", items.qty);
                                        cmd.Parameters.AddWithValue("@prodCode", items.prodCode);
                                        try
                                        {
                                            cmd.ExecuteNonQuery();
                                        }
                                        catch (SqlCeException ex)
                                        {
                                            MessageBox.Show("Error! Log has been updated with the error!");
                                        }
                                    }
                                }
                            }
                            document.InsertTable(t2);


                            document.InsertParagraph(); //newline

                            var t3 = document.AddTable(2, 2);
                            t3.Design = TableDesign.None;
                            t3.AutoFit = AutoFit.Window;

                            t3.Rows[0].Cells[0].Paragraphs[0].Append("ISSUED ON :         ").FontSize(9).Bold();
                            t3.Rows[1].Cells[0].Paragraphs[0].Append("ISSUED BY :          ").FontSize(9).Bold().Alignment = Alignment.left;
                            t3.Rows[0].Cells[1].Paragraphs[0].Append("RETURNED ON :_________________________").FontSize(9).Bold().Alignment = Alignment.right;
                            t3.Rows[1].Cells[1].Paragraphs[0].Append("RECEIVED BY   :_________________________").FontSize(9).Bold().Alignment = Alignment.right;

                            underline = returnCount(19, txtDate.Text);

                            t3.Rows[0].Cells[0].Paragraphs[0].Append("__" + txtDate.Text + underline).FontSize(9).Bold().UnderlineStyle(UnderlineStyle.thick).Alignment = Alignment.left;
                            underline = "";

                            underline = returnCount(19, txtIssued.Text);

                            t3.Rows[1].Cells[0].Paragraphs[0].Append("__" + txtIssued.Text + underline).UnderlineStyle(UnderlineStyle.thick).FontSize(9).Bold().Alignment = Alignment.left;
                            underline = "";

                            document.InsertTable(t3);

                            document.InsertParagraph();

                            var p5 = document.InsertParagraph(@"                I/We, the undersigned, acknowledge to have received the apparatus above clean, dry and in good condition. Said articles are to be returned upon the termination of the semester clean, dry and in good condition.").FontSize(9.5).Bold().Alignment = Alignment.both;

                            document.InsertParagraph();

                            var p6 = document.InsertParagraph("FILL THE BLANKS PROPERLY AND IN PRINT").Bold().UnderlineStyle(UnderlineStyle.singleLine);

                            document.InsertParagraph();

                            var t4 = document.AddTable(5, 4);
                            t4.Design = TableDesign.None;
                            t4.AutoFit = AutoFit.Window;

                            foreach (Row row in t4.Rows)
                            {
                                row.Cells[0].Width = 180;
                                row.Cells[1].Width = 120;
                                row.Cells[2].Width = 80;
                                row.Cells[3].Width = 80;
                            }

                            t4.Rows[0].Cells[0].Paragraphs[0].Append("   SURNAME         FIRST NAME        M.I.").FontSize(9).Bold();
                            t4.Rows[0].Cells[1].Paragraphs[0].Append("STUDENT NUMBER").FontSize(9).Bold().Alignment = Alignment.center;
                            t4.Rows[0].Cells[2].Paragraphs[0].Append("COURSE").FontSize(9).Bold().Alignment = Alignment.center;
                            t4.Rows[0].Cells[3].Paragraphs[0].Append("SIGNATURE").FontSize(9).Bold().Alignment = Alignment.center;

                            t4.Rows[1].Cells[0].Paragraphs[0].Append("1. ").Bold().FontSize(9);
                            t4.Rows[2].Cells[0].Paragraphs[0].Append("2. ").Bold().FontSize(9);
                            t4.Rows[3].Cells[0].Paragraphs[0].Append("3. ").Bold().FontSize(9);
                            t4.Rows[4].Cells[0].Paragraphs[0].Append("4. ").Bold().FontSize(9);

                            underline = returnCount(30, txtName1.Text);

                            t4.Rows[1].Cells[0].Paragraphs[0].Append("_" + txtName1.Text + underline).UnderlineStyle(UnderlineStyle.thick).Bold().FontSize(9);
                            underline = "";

                            underline = returnCount(30, txtName2.Text);

                            t4.Rows[2].Cells[0].Paragraphs[0].Append("_" + txtName2.Text + underline).UnderlineStyle(UnderlineStyle.thick).Bold().FontSize(9);
                            underline = "";

                            underline = returnCount(30, txtName3.Text);

                            t4.Rows[3].Cells[0].Paragraphs[0].Append("_" + txtName3.Text + underline).UnderlineStyle(UnderlineStyle.thick).Bold().FontSize(9);
                            underline = "";

                            underline = returnCount(30, txtName4.Text);

                            t4.Rows[4].Cells[0].Paragraphs[0].Append("_" + txtName4.Text + underline).UnderlineStyle(UnderlineStyle.thick).Bold().FontSize(9);
                            underline = "";

                            t4.Rows[1].Cells[1].Paragraphs[0].Append("____").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[2].Cells[1].Paragraphs[0].Append("____").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[3].Cells[1].Paragraphs[0].Append("____").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[4].Cells[1].Paragraphs[0].Append("____").UnderlineStyle(UnderlineStyle.thick).FontSize(9);

                            underline = returnCount(16, txtStud1.Text);

                            t4.Rows[1].Cells[1].Paragraphs[0].Append(txtStud1.Text + underline).UnderlineStyle(UnderlineStyle.thick).Bold().FontSize(9);
                            underline = "";

                            underline = returnCount(16, txtStud2.Text);
                            t4.Rows[2].Cells[1].Paragraphs[0].Append(txtStud2.Text + underline).UnderlineStyle(UnderlineStyle.thick).Bold().FontSize(9);
                            underline = "";

                            underline = returnCount(16, txtStud3.Text);
                            t4.Rows[3].Cells[1].Paragraphs[0].Append(txtStud3.Text + underline).UnderlineStyle(UnderlineStyle.thick).Bold().FontSize(9);
                            underline = "";

                            underline = returnCount(16, txtStud4.Text);
                            t4.Rows[4].Cells[1].Paragraphs[0].Append(txtStud4.Text + underline).UnderlineStyle(UnderlineStyle.thick).Bold().FontSize(9);

                            t4.Rows[1].Cells[2].Paragraphs[0].Append("_________________").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[2].Cells[2].Paragraphs[0].Append("_________________").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[3].Cells[2].Paragraphs[0].Append("_________________").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[4].Cells[2].Paragraphs[0].Append("_________________").UnderlineStyle(UnderlineStyle.thick).FontSize(9);

                            t4.Rows[1].Cells[3].Paragraphs[0].Append("_________________").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[2].Cells[3].Paragraphs[0].Append("_________________").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[3].Cells[3].Paragraphs[0].Append("_________________").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            t4.Rows[4].Cells[3].Paragraphs[0].Append("_________________").UnderlineStyle(UnderlineStyle.thick).FontSize(9);
                            document.InsertTable(t4);

                            document.InsertParagraph();

                            var p7 = document.InsertParagraph("PHARMACY LABORATORY'S COPY").FontSize(9).Bold().Alignment = Alignment.center;

                            document.Save();
                            Process.Start("WINWORD.EXE", filename);
                            emptyFields();
                        }
                        break;
                    case MessageBoxResult.No:
                        break;
                }

                /*
                Report_Management.IssuanceForm issuanceForm = new Report_Management.IssuanceForm();
                issuanceForm.Show();
                */
            }

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

        private void emptyFields()
        {
            txtDate.Text = null;
            txtIssued.Text = null;
            txtLock.Text = null;
            cmbSubj.SelectedIndex = -1;
            txtProf.Text = null;
            txtSched.Text = null;
            txtSect.Text = null;

            txtName1.Text = null;
            txtName2.Text = null;
            txtName3.Text = null;
            txtName4.Text = null;
            txtStud1.Text = null;
            txtStud2.Text = null;
            txtStud3.Text = null;
            txtStud4.Text = null;
            

            items.Clear();
            studInfo.Clear();
        }
    }


}
