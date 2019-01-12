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
using System.Threading.Tasks;
using System.Windows.Media;
using System.Drawing;
namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for GenerateIssuanceForm.xaml
    /// </summary>
    public partial class GenerateIssuanceForm : System.Windows.Controls.Page
    {
        public static string profName;
        public static string schedule;
        public static string lockerNumber;
        public static string subjAndSect;
        ObservableCollection<LVIssuance> items = new ObservableCollection<LVIssuance>();

        int i = 1;
        public GenerateIssuanceForm(string fullName)
        {
            InitializeComponent();
            fillSubjects();
            txtIssued.Text = fullName;
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
            subjAndSect = txtSubject.Text;
        }

        private ObservableCollection<LVIssuance> LoadCollectionData()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            string name = "", size = "", manuf = "";
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT sub.qty, sub.prodCode, ai.name, ai.size from Subjects sub INNER JOIN ApparatusInventory ai on sub.prodCode = ai.prodCode where sub.subjName = @subjName", conn))
            {
                cmd.Parameters.AddWithValue("@subjName", txtSubject.Text);
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        items.Clear();
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

                            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT qty from ApparatusInventory where prodCode = @prodCode", conn))
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

                                            if (qty > countQty)
                                            {
                                                MessageBox.Show("Item: " + name + "has low stocks, please stock in as soon as possible!");
                                                items.Add(new LVIssuance()
                                                {
                                                    i = i,
                                                    inventName = name,
                                                    manufList = manufacturer,
                                                    manuf = manuf,
                                                    size = size,
                                                    qty = countQty
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
                                                    size = size,
                                                    qty = qty
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
            if (string.IsNullOrEmpty(cmbSubj.Text))
            {
                MessageBox.Show("Fill in the missing fields");
            }
            else
            {
                string user = Environment.UserName;
                string filename = @"C:\Users\" + user + @"\Desktop\GENERATEDFORM.docx"; //change GENERATED FORM INTO SOMETHING THAT CAN SAVE THE ISSUEDDATE+SUBJECTANDSECT+SCHED

                using (DocX document = DocX.Create(filename))
                {

                    var p1 = document.InsertParagraph("ISSUANCE FORM").Bold().FontSize(9)
                    .Alignment = Alignment.right;

                    var p2 = document.InsertParagraph("PHARMACY LABORATORY").Bold().FontSize(7)
                    .UnderlineColor(System.Drawing.Color.Black).SpacingAfter(22).Alignment = Alignment.right;


                    var t0 = document.AddTable(2, 1);
                    t0.Design = TableDesign.None;
                    t0.Alignment = Alignment.right;

                    t0.Rows[0].Cells[0].Paragraphs[0].Bold().Append("________"+ txtLock.Text + "___________").Bold().UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.center;
                    t0.Rows[1].Cells[0].Paragraphs[0].Append("LOCKER NUMBER").Bold().Alignment = Alignment.center;

                    document.InsertTable(t0);

                    document.InsertParagraph();

                    var t1 = document.AddTable(2, 3);
                    t1.Design = TableDesign.None;
                    t1.AutoFit = AutoFit.Window;

                    foreach(Row row in t1.Rows)
                    {
                        row.Cells[0].Width = 200;
                        row.Cells[1].Width = 200;
                        row.Cells[2].Width = 200;

                    }

                    t1.Rows[1].Cells[0].Paragraphs[0].Append("PROFESSOR").Bold().Alignment = Alignment.center;
                    t1.Rows[0].Cells[0].Paragraphs[0].Append("___").FontSize(10).Bold().UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.center;
                    t1.Rows[0].Cells[0].Paragraphs[0].Append(txtProf.Text + "___").FontSize(10).Bold().UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.center;
                    t1.Rows[1].Cells[1].Paragraphs[0].Append("SCHEDULE").FontSize(10).Bold().Alignment = Alignment.center;
                    t1.Rows[0].Cells[1].Paragraphs[0].Append("_____").Bold().UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.center;
                    t1.Rows[0].Cells[1].Paragraphs[0].Append(txtSched.Text + "____").FontSize(10).Bold().UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.center;
                    t1.Rows[1].Cells[2].Paragraphs[0].Append("SUBJECT AND SECTION").Bold().Alignment = Alignment.center;
                    t1.Rows[0].Cells[2].Paragraphs[0].Append("____").FontSize(10).Bold().UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.center;
                    t1.Rows[0].Cells[2].Paragraphs[0].Append(txtSubject.Text + "____").FontSize(10).Bold().UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.center;
                    document.InsertTable(t1);

                    document.InsertParagraph(); //newline

                    var t2 = document.AddTable(26, 6);
                    t2.Design = TableDesign.TableGrid;
                    t2.AutoFit = AutoFit.Window;

                    foreach(Row row in t2.Rows)
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

                    document.InsertTable(t2);


                    document.InsertParagraph(); //newline

                    var t3 = document.AddTable(2, 2);
                    t3.Design = TableDesign.None;
                    t3.AutoFit = AutoFit.Window;

                    t3.Rows[0].Cells[0].Paragraphs[0].Append("ISSUED ON :         __").FontSize(10).Bold();
                    t3.Rows[0].Cells[0].Paragraphs[0].Append(txtDate.Text + "__________").FontSize(10).Bold().UnderlineStyle(UnderlineStyle.singleLine).Alignment = Alignment.left;
                    t3.Rows[1].Cells[0].Paragraphs[0].Append("ISSUED BY :          __").FontSize(10).Bold().Alignment = Alignment.left;
                    t3.Rows[1].Cells[0].Paragraphs[0].Append(txtIssued.Text + "________").UnderlineStyle(UnderlineStyle.singleLine).FontSize(10).Bold().Alignment = Alignment.left;
                    t3.Rows[0].Cells[1].Paragraphs[0].Append("RETURNED ON :_________________________").FontSize(10).Bold().Alignment = Alignment.right;
                    t3.Rows[1].Cells[1].Paragraphs[0].Append("RECEIVED BY   :_________________________").FontSize(10).Bold().Alignment = Alignment.right;

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

                    t4.Rows[1].Cells[0].Paragraphs[0].Append("1. _______________________________").FontSize(9);
                    t4.Rows[2].Cells[0].Paragraphs[0].Append("2. _______________________________").FontSize(9);
                    t4.Rows[3].Cells[0].Paragraphs[0].Append("3. _______________________________").FontSize(9);
                    t4.Rows[4].Cells[0].Paragraphs[0].Append("4. _______________________________").FontSize(9);

                    t4.Rows[1].Cells[1].Paragraphs[0].Append("_____________________").FontSize(9);
                    t4.Rows[2].Cells[1].Paragraphs[0].Append("_____________________").FontSize(9);
                    t4.Rows[3].Cells[1].Paragraphs[0].Append("_____________________").FontSize(9);
                    t4.Rows[4].Cells[1].Paragraphs[0].Append("_____________________").FontSize(9);

                    t4.Rows[1].Cells[2].Paragraphs[0].Append("_________________").FontSize(9);
                    t4.Rows[2].Cells[2].Paragraphs[0].Append("_________________").FontSize(9);
                    t4.Rows[3].Cells[2].Paragraphs[0].Append("_________________").FontSize(9);
                    t4.Rows[4].Cells[2].Paragraphs[0].Append("_________________").FontSize(9);

                    t4.Rows[1].Cells[3].Paragraphs[0].Append("_________________").FontSize(9);
                    t4.Rows[2].Cells[3].Paragraphs[0].Append("_________________").FontSize(9);
                    t4.Rows[3].Cells[3].Paragraphs[0].Append("_________________").FontSize(9);
                    t4.Rows[4].Cells[3].Paragraphs[0].Append("_________________").FontSize(9);
                    document.InsertTable(t4);

                    document.InsertParagraph();

                    var p7 = document.InsertParagraph("PHARMACY LABORATORY'S COPY").FontSize(9).Bold().Alignment = Alignment.center;

                    foreach (var items in items)
                    {
                        document.InsertParagraph(items.inventName + " " + items.manuf + " " + items.prodCode);
                    }
                    document.Save();

                    Process.Start("WINWORD.EXE", filename);
                }

                /*
                Report_Management.IssuanceForm issuanceForm = new Report_Management.IssuanceForm();
                issuanceForm.Show();
                */
            }

        }

        private void txtProf_TextChanged(object sender, TextChangedEventArgs e)
        {
            profName = txtProf.Text;
        }
        private void txtSched_TextChanged(object sender, TextChangedEventArgs e)
        {
            schedule = txtSched.Text;
        }
        private void txtLock_TextChanged(object sender, TextChangedEventArgs e)
        {
            lockerNumber = txtLock.Text;
        }
    }


}
