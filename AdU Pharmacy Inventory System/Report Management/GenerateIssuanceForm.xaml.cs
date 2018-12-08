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
using System.Data.SqlClient;
using System.Data.Common;
using System.Threading;
using System.Collections.ObjectModel;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for GenerateIssuanceForm.xaml
    /// </summary>
    public partial class GenerateIssuanceForm : Page
    {
        public static string profName;
        public static string schedule;
        public static string lockerNumber;
        public static string subjAndSect;

        int i = 1;
        public GenerateIssuanceForm()
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
                            string subjName = reader["subjName"].ToString();
                            cmbSubject.Items.Add(subjName);
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

        private ObservableCollection<LVApparatusStockOut> LoadCollectionData()
        {
            ObservableCollection<LVApparatusStockOut> items = new ObservableCollection<LVApparatusStockOut>();
            //List<string> manufList = new List<string>();
            SqlCeConnection conn = DBUtils.GetDBConnection();
            string name = "", size = "", manuf = "";
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from Subjects where subjName = @subjName", conn))
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

                            int countQty;
                            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT name, size from ApparatusInventory where prodCode = @prodCode",conn))
                            {
                                cmd1.Parameters.AddWithValue("@prodCode", prodCode);
                                using (DbDataReader dr = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    if (dr.HasRows)
                                    {
                                        dr.Read();

                                        int nameIndex = dr.GetOrdinal("name");
                                        name = Convert.ToString(dr.GetValue(nameIndex));

                                        int sizeIndex = dr.GetOrdinal("size");
                                        size = Convert.ToString(dr.GetValue(sizeIndex));

                                    }
                                }
                            }
                            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT manuf from ApparatusInventory where prodCode = @prodCode", conn))
                            {
                                cmd1.Parameters.AddWithValue("@prodCode", prodCode);
                                using (DbDataReader dr = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    if (dr.HasRows)
                                    {
                                        while (dr.Read())
                                        {
                                            int manufIndex = dr.GetOrdinal("manuf");
                                            manuf = Convert.ToString(dr.GetValue(manufIndex));

                                            
                                            //manufList.Add(manuf);

                                            //cmbManuf.ItemsSource = manufList;
                                        }
                                    }
                                }
                            }
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
                                                MessageBox.Show("Item: " + prodCode + "has low stocks, please stock in as soon as possible!");
                                                items.Add(new LVApparatusStockOut()
                                                {
                                                    i = i,
                                                    inventName = name,
                                                    manuf = manuf,
                                                    size = size,
                                                    qty = countQty
                                                });
                                            }
                                            else
                                            {
                                                items.Add(new LVApparatusStockOut()
                                                {
                                                    i = i,
                                                    inventName = name,
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
            this.NavigationService.Navigate(new GenerateIssuanceForm());
        }

        private void btnGenForm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cmbSubject.Text))
            {
                MessageBox.Show("Fill in the missing fields");
            }
            else
            {
                Report_Management.IssuanceForm issuanceForm = new Report_Management.IssuanceForm();
                issuanceForm.Show();
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
