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

namespace AdU_Pharmacy_Inventory_System
{
    /// <summary>
    /// Interaction logic for GenerateChemicalMonitoring.xaml
    /// </summary>
    public partial class GenerateChemicalMonitoring : Page
    {
        public GenerateChemicalMonitoring()
        {
            InitializeComponent();
        }

        private void btnGenForm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(date.Text))
            {
                MessageBox.Show("Date field is empty!");
                date.Focus();
            }
            else
            {
                ChemicalStocks cs = new ChemicalStocks();
            }
        }
    }
}