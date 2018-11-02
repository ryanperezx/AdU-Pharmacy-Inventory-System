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
    /// Interaction logic for Apparatus_Inventory_Summary.xaml
    /// </summary>
    public partial class Apparatus_Inventory_Summary : Page
    {
        public Apparatus_Inventory_Summary()
        {
            InitializeComponent();
            date.Text = DateTime.Now.ToString("dd MMMM yyyy");
        }
    }
}
