using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    /// 
    public partial class StartView : UserControl
    {
        public StartView()
        {
            InitializeComponent();
        }

        private void quickstart(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Game);
        }

        private void settings(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Settings);
        }
    }
}
