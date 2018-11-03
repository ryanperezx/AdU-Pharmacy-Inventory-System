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

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for GenerateIssuanceForm.xaml
    /// </summary>
    public partial class GenerateIssuanceForm : Page
    {
        public GenerateIssuanceForm()
        {
            InitializeComponent();
            
        }

        private void cmbSubject_SelectionChanged(object sender, EventArgs e)
        {
            var item = (ComboBoxItem)cmbSubject.SelectedValue;
            var content = (string)item.Content;

            if(content.Equals("Biochem Lab"))
            {
                this.lvSubject.Items.Clear();
                this.lvSubject.Items.Add(new MyItem { apparatusName = "Aspirator", qty = 1, remarks = "" });
                this.lvSubject.Items.Add(new MyItem { apparatusName = "Beaker", qty = 1, remarks = "1000mL" });
            }
        }
    }


}
