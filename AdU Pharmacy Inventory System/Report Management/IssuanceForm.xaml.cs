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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Data.SqlServerCe;
using System.Data.Common;
using Spire.Doc;
using Spire.Doc.Documents;
using System.Diagnostics;

namespace AdU_Pharmacy_Inventory_System.Report_Management
{
    /// <summary>
    /// Interaction logic for IssuanceForm.xaml
    /// </summary>
    public partial class IssuanceForm : Window, INotifyPropertyChanged
    {
        public string profName;
        public string ProfName
        {
            get { return profName; }
            set
            {
                profName = value;
                OnPropertyChanged();
            }
        }

        public string subjAndSect;
        public string SubjAndSect
        {
            get { return subjAndSect; }
            set
            {
                subjAndSect = value;
                OnPropertyChanged();
            }
        }

        public string schedule;
        public string Schedule
        {
            get { return schedule; }
            set
            {
                schedule = value;
                OnPropertyChanged();
            }
        }

        public string lockerNumber;
        public string LockerNumber
        {
            get { return lockerNumber; }
            set
            {
                lockerNumber = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
 
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        int i = 0;
        public IssuanceForm()
        {
            InitializeComponent();
            lblProf.Content = GenerateIssuanceForm.profName;
            lblSchedule.Content = GenerateIssuanceForm.schedule;
            lblLockerNumber.Content = GenerateIssuanceForm.lockerNumber;
            lblSubject.Content = GenerateIssuanceForm.subjAndSect;
            Console.WriteLine(profName);
            fillList();
        }

        private void fillList()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();

            string prodCode = "";
            string subjName = "";
            List<string> prodCodes = new List<string>(); 

            try
            {
                subjName = lblSubject.Content.ToString();
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
            
           
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT prodCode FROM Subjects WHERE subjName = '" + subjName + "'", conn))
                {
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int prodCodeIndex = reader.GetOrdinal("prodCode");
                                prodCode = Convert.ToString(reader.GetValue(prodCodeIndex));
                                prodCodes.Add(prodCode);
                            }
                        }
                    }
                }

            lvList.Items.Clear();
           
            for (int j = 0; j < prodCodes.Count; j++)
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT TOP 1 Subjects.qty, ApparatusInventory.name, ApparatusInventory.size, ApparatusInventory.manuf, ApparatusInventory.remarks from Subjects INNER JOIN ApparatusInventory ON ApparatusInventory.prodCode = '" + prodCodes.ElementAt(j) + "' AND Subjects.subjName = '" + subjName + "'", conn))
                {
                    Debug.WriteLine(j + prodCodes.ElementAt(j));
                    using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            
                            while (reader.Read())
                            {
                                int quantityIndex = reader.GetOrdinal("qty");
                                int quantity = Convert.ToInt32(reader.GetValue(quantityIndex));

                                int apparatusIndex = reader.GetOrdinal("name");
                                string apparatusName = Convert.ToString(reader.GetValue(apparatusIndex));
                                Debug.WriteLine(apparatusName);

                                string sizeBrandRemarks = "";

                                int sizeIndex = reader.GetOrdinal("size");
                                string size = Convert.ToString(reader.GetValue(sizeIndex));

                                int manufIndex = reader.GetOrdinal("manuf");
                                string manuf = Convert.ToString(reader.GetValue(manufIndex));

                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));

                                sizeBrandRemarks = size + " / " + manuf + " / " + remarks;

                                lvList.Items.Add(new LVIssuance
                                {
                                    qty = quantity,
                                    apparatusName = apparatusName,
                                    sizeBrandRemarks = sizeBrandRemarks,
                                    breakages = null,

                                });
                                i++;
                            }
                        }
                    }
                }
            }
        }

        private void btnPrintIssuance_Click(object sender, RoutedEventArgs e)
        {
            /*sv.ScrollToHome();
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintVisual(this, "Window Printing");*/
        }

        Dictionary<string, string> GetReplaceDictionary()
        {
            Dictionary<string, string> replaceDict = new Dictionary<string, string>();
            replaceDict.Add("#prof#", lblProf.Content.ToString());
            replaceDict.Add("#sched#", lblSchedule.Content.ToString());
            replaceDict.Add("#lockerNo#", lblLockerNumber.Content.ToString());

            return replaceDict;
        }

        private void btnPrintCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }


    }
}
