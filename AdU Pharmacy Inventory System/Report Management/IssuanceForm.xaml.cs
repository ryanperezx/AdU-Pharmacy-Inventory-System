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
using Spire.Doc;


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
            get { return lockerNumber ; }
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

        public IssuanceForm()
        {
            InitializeComponent();
            lblProf.Content = GenerateIssuanceForm.profName;
            lblSchedule.Content = GenerateIssuanceForm.schedule;
            lblLockerNumber.Content = GenerateIssuanceForm.lockerNumber;
            Console.WriteLine(profName);
        }

        private void btnPrintIssuance_Click(object sender, RoutedEventArgs e)
        {
            /*sv.ScrollToHome();
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintVisual(this, "Window Printing");*/

            //initialize word object  
            Document document = new Document();
            string samplePath = "C:\\Users\\James\\Desktop\\ISSUANCEFORM.docx";
            string fileName = "Report.docx";
            document.LoadFromFile(samplePath);
            //get strings to replace  
            Dictionary<string, string> dictReplace = GetReplaceDictionary();
            //Replace text  
            foreach (KeyValuePair<string, string> kvp in dictReplace)
            {
                document.Replace(kvp.Key, kvp.Value, true, true);
                Console.WriteLine(kvp.Key);
            }
            //Save doc file.  
            document.SaveToFile(fileName, FileFormat.Docx);        
            MessageBox.Show("All tasks are finished.");
            document.Close();

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
