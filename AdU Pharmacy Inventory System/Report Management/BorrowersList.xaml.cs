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


namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for BorrowersList.xaml
    /// </summary>
    public partial class BorrowersList : Page
    {
        int i = 1;
        public BorrowersList()
        {
            InitializeComponent();
            fillList();
        }

        private void fillList()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT * from BorrowerList", conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        lvList.Items.Clear();
                        while (reader.Read())
                        {
                            int dateIndex = reader.GetOrdinal("date");
                            DateTime myDate = reader.GetDateTime(dateIndex);
                            string date = myDate.ToString("MM/dd/yyyy");

                            int studentNoIndex = reader.GetOrdinal("studentNo");
                            string studentNo = Convert.ToString(reader.GetValue(studentNoIndex));

                            int fullNameIndex = reader.GetOrdinal("fullName");
                            string fullName = Convert.ToString(reader.GetValue(fullNameIndex));

                            int subjIndex = reader.GetOrdinal("subject");
                            string subj = Convert.ToString(reader.GetValue(subjIndex));

                            int grpIDIndex = reader.GetOrdinal("groupID");
                            int grpID = Convert.ToInt32(reader.GetValue(grpIDIndex));

                            int appaNameIndex = reader.GetOrdinal("borrowedInventName");
                            string appaName = Convert.ToString(reader.GetValue(appaNameIndex));

                            int manufIndex = reader.GetOrdinal("manuf");
                            string manuf = Convert.ToString(reader.GetValue(manufIndex));

                            int qtyIndex = reader.GetOrdinal("qty");
                            int qty = Convert.ToInt32(reader.GetValue(qtyIndex));
                            lvList.Items.Add(new LVBorrower
                            {
                                date = date,
                                studentNo = studentNo,
                                fullName = fullName,
                                subj = subj,
                                grpID = grpID,
                                appaName = appaName,
                                manuf = manuf,
                                qty = qty
                            });
                            i++;
                        }
                    }
                }
            }
        }
    }
}
