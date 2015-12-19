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

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void play(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Game);
        }

        private void setSmall(object sender, MouseButtonEventArgs e)
        {
            SettingsModel.Rows = 4;
            SettingsModel.Columns = 4;
        }

        private void setMedium(object sender, MouseButtonEventArgs e)
        {
            SettingsModel.Rows = 5;
            SettingsModel.Columns = 6;
        }

        private void setLarge(object sender, MouseButtonEventArgs e)
        {
            SettingsModel.Rows = 6;
            SettingsModel.Columns = 8;
        }

        private void setAgainstAI(object sender, MouseButtonEventArgs e)
        {
            SettingsModel.AgainstAI = true;
        }

        private void setTwoPlayer(object sender, MouseButtonEventArgs e)
        {
            SettingsModel.AgainstAI = false;
        }
    }
}
