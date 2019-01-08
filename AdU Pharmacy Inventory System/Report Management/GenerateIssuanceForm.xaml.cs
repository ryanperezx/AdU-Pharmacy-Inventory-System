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
                string filename = @"C:\Users\" + user + @"\Desktop\GENERATEDFORM.docx";

                using (DocX document = DocX.Create(filename))
                {
                    Formatting titleFormat = new Formatting();
                    titleFormat.FontFamily = new Font("Segoe UI");
                    titleFormat.Bold = true;

                    titleFormat.Size = 9;
                    Paragraph text = document.InsertParagraph("ISSUANCE FORM", false, titleFormat);
                    text.Alignment = Alignment.right;

                    titleFormat.Size = 8;
                    text = document.InsertParagraph("PHARMACY LABORATORY " + Environment.NewLine, false, titleFormat);
                    text.Alignment = Alignment.right;

                    titleFormat.Size = 9;
                    text = document.InsertParagraph("_______________________", false, titleFormat);
                    text.Alignment = Alignment.right;

                    text = document.InsertParagraph("LOCKER NUMBER", false, titleFormat);
                    text.Alignment = Alignment.right;

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
