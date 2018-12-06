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
using System.Data.Common;

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for List_of_Subjects.xaml
    /// </summary>
    public partial class List_of_Subjects : Page
    {
        public List_of_Subjects()
        {
            InitializeComponent();
            fillSubjects();
        }

        private void fillSubjects()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT DISTINCT subjCode, subjName from Subjects", conn))
            {
                using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.HasRows)
                    {
                        lvList.Items.Clear();
                        while (reader.Read())
                        {
                            int subjCodeIndex = reader.GetOrdinal("subjCode");
                            string subjCode = Convert.ToString(reader.GetValue(subjCodeIndex));

                            int subjNameIndex = reader.GetOrdinal("subjName");
                            string subjName = Convert.ToString(reader.GetValue(subjNameIndex));

                            lvList.Items.Add(new LVSubject
                            {
                                subjCode = subjCode,
                                subjName = subjName
                            });
                        }
                    }
                }
            }
        }
    }
}
