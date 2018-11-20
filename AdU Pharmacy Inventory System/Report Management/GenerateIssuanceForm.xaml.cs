﻿using System;
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

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for GenerateIssuanceForm.xaml
    /// </summary>
    public partial class GenerateIssuanceForm : Page
    {
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
        }

        private List<LVApparatusStockOut> LoadCollectionData()
        {
            List<LVApparatusStockOut> items = new List<LVApparatusStockOut>();
            SqlCeConnection conn = DBUtils.GetDBConnection();
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
                            int inventNameIndex = reader.GetOrdinal("inventName");
                            string inventName = Convert.ToString(reader.GetValue(inventNameIndex));

                            int manufIndex = reader.GetOrdinal("manuf");
                            string manuf = Convert.ToString(reader.GetValue(manufIndex));

                            int qtyIndex = reader.GetOrdinal("qty");
                            int qty = Convert.ToInt32(reader.GetValue(qtyIndex));

                            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT qty from InventoryStock where name = @inventName and manuf = @manuf and qty = @qty"))
                            {
                                cmd1.Parameters.AddWithValue("@inventName", inventName);
                                cmd1.Parameters.AddWithValue("@manuf", manuf);
                                cmd1.Parameters.AddWithValue("@qty", qty);
                                using (DbDataReader dr = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                                {
                                    if (reader.HasRows)
                                    {
                                        dr.Read();

                                        int countQtyIndex = dr.GetOrdinal("qty");
                                        int countQty = Convert.ToInt32(reader.GetValue(countQtyIndex));
                                        if (qty > countQty)
                                        {
                                            MessageBox.Show("Item: " + inventName + "has low stocks, please stock in as soon as possible!");
                                            items.Add(new LVApparatusStockOut()
                                            {
                                                i = i,
                                                inventName = inventName,
                                                manuf = manuf,
                                                qty = countQty
                                            });
                                        }
                                        else
                                        {
                                            items.Add(new LVApparatusStockOut()
                                            {
                                                i = i,
                                                inventName = inventName,
                                                manuf = manuf,
                                                qty = qty
                                            });
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

    }


}
