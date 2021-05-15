using AppService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UICarManager
{
    /// <summary>
    /// Interaction logic for RefWindow.xaml
    /// </summary>
    public partial class RefWindow : Window
    {
        WindowProperty winProp = new WindowProperty();
        public RefWindow()
        {
            InitializeComponent();
            this.DataContext = winProp;
        }

        public WindowProperty GetData()
        {
            return winProp;
        }


    }
}
