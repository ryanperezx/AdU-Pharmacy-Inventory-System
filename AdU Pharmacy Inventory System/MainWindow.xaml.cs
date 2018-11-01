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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //REPORT MANAGEMENT
        GenerateIssuanceForm gif = new GenerateIssuanceForm();
        GenerateChemicalMonitoring gcm = new GenerateChemicalMonitoring();
        //STOCK MANAGEMENT
        Stock_In si = new Stock_In();
        Add_Inventory ai = new Add_Inventory();
        ApparatusStockOut aso = new ApparatusStockOut();
        ChemicalStockOut cso = new ChemicalStockOut();
        //ACADEMIC MANAGEMENT
        AddSubject ads = new AddSubject();
        public MainWindow()
        {
            InitializeComponent();
            date.Content = DateTime.Now.ToString("D");
            stack.DataContext = new ExpanderListViewModel();
        }

        private void lblExit_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Do you want to exit the application?";
            string sCaption = "Exit";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (dr)
            {
                case MessageBoxResult.Yes:
                    this.DialogResult = true;
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void lblLogOut_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Do you want to log out?";
            string sCaption = "Logout";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNoCancel;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult dr = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (dr)
            {
                case MessageBoxResult.Yes:
                    this.DialogResult = false;
                    Close();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }



        private void tbGenerateIsuForm_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(gif);
        }

        private void tbGenerateIsuForm_MouseEnter(object sender, MouseEventArgs e)
        {
            tbGenerateIsuForm.TextDecorations = TextDecorations.Underline;
        }

        private void tbGenerateIsuForm_MouseLeave(object sender, MouseEventArgs e)
        {
            tbGenerateIsuForm.TextDecorations = null;
        }

        private void tbGenerateChemMon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(gcm);
        }

        private void tbGenerateChemMon_MouseEnter(object sender, MouseEventArgs e)
        {
            tbGenerateChemMon.TextDecorations = TextDecorations.Underline;
        }

        private void tbGenerateChemMon_MouseLeave(object sender, MouseEventArgs e)
        {
            tbGenerateChemMon.TextDecorations = null;
        }

        private void tbSubjects_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(ads);
        }

        private void tbSubjects_MouseEnter(object sender, MouseEventArgs e)
        {
            tbSubjects.TextDecorations = TextDecorations.Underline;
        }

        private void tbSubjects_MouseLeave(object sender, MouseEventArgs e)
        {
            tbSubjects.TextDecorations = null;
        }


        private void tbAppaStockOut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(aso);
        }

        private void tbChemStockOut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(cso);
        }

        private void tbAppaStockOut_MouseEnter(object sender, MouseEventArgs e)
        {
            tbAppaStockOut.TextDecorations = TextDecorations.Underline;
        }

        private void tbChemStockOut_MouseEnter(object sender, MouseEventArgs e)
        {
            tbChemStockOut.TextDecorations = TextDecorations.Underline;

        }

        private void tbAppaStockOut_MouseLeave(object sender, MouseEventArgs e)
        {
            tbAppaStockOut.TextDecorations = null;
        }

        private void tbChemStockOut_MouseLeave(object sender, MouseEventArgs e)
        {
            tbChemStockOut.TextDecorations = null;
        }

        private void tbStockIn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(si);
        }

        private void tbStockIn_MouseEnter(object sender, MouseEventArgs e)
        {
            tbStockIn.TextDecorations = TextDecorations.Underline;
        }

        private void tbStockIn_MouseLeave(object sender, MouseEventArgs e)
        {
            tbStockIn.TextDecorations = null;
        }

        private void tbAddInvent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(ai);
        }

        private void tbAddInvent_MouseEnter(object sender, MouseEventArgs e)
        {
            tbAddInvent.TextDecorations = TextDecorations.Underline;
        }

        private void tbAddInvent_MouseLeave(object sender, MouseEventArgs e)
        {
            tbAddInvent.TextDecorations = null;
        }
    }
}
