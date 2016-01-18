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
using System.Windows.Media.Animation;
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
        // seleted board size
        Label selectedBoardLabel = null;
        public SettingsView()
        {
            InitializeComponent();
            
            // mark labels
            switch(SettingsModel.Rows)
            {
                case 4:
                    selectedBoardLabel = lblSmall;
                    break;
                case 5:
                    selectedBoardLabel = lblMedium;
                    break;
                case 6:
                    selectedBoardLabel = lblLarge;
                    break;
            }
            selectLabel(selectedBoardLabel);
            if (SettingsModel.AgainstAI)
                selectLabel(lblCpu);
            else
                selectLabel(lblHuman);
        }
        private void deSelectLabel(Label label)
        {
            label.Foreground = Brushes.Black;
            label.Background = Brushes.Transparent;
        }
        private void selectLabel(Label label)
        {
            deSelectLabel(label);
            label.Background = Brushes.DarkGreen;
            label.Foreground = Brushes.White;
        }

        private void play(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Game);
        }

        private void setSmall(object sender, MouseButtonEventArgs e)
        {
            deSelectLabel(selectedBoardLabel);
            selectLabel(sender as Label);
            selectedBoardLabel = sender as Label;
            SettingsModel.Rows = 4;
            SettingsModel.Columns = 4;
        }

        private void setMedium(object sender, MouseButtonEventArgs e)
        {
            deSelectLabel(selectedBoardLabel);
            selectLabel(sender as Label);
            selectedBoardLabel = sender as Label;
            SettingsModel.Rows = 5;
            SettingsModel.Columns = 6;
        }

        private void setLarge(object sender, MouseButtonEventArgs e)
        {
            deSelectLabel(selectedBoardLabel);
            selectLabel(sender as Label);
            selectedBoardLabel = sender as Label;
            SettingsModel.Rows = 6;
            SettingsModel.Columns = 8;
        }

        private void setAgainstAI(object sender, MouseButtonEventArgs e)
        {
            deSelectLabel(lblHuman);
            selectLabel(lblCpu);
            SettingsModel.AgainstAI = true;
        }

        private void setTwoPlayer(object sender, MouseButtonEventArgs e)
        {
            deSelectLabel(lblCpu);
            selectLabel(lblHuman);
            SettingsModel.AgainstAI = false;
        }

        private void hoverLabel(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            if(label != selectedBoardLabel && !(SettingsModel.AgainstAI && label == lblCpu) && !(!SettingsModel.AgainstAI && label == lblHuman))
            {
                ColorAnimation anim = new ColorAnimation(Colors.LightGreen, TimeSpan.FromMilliseconds(100));
                label.Background = new SolidColorBrush((Color)label.Background.GetValue(SolidColorBrush.ColorProperty)); // annars kastas exception
                label.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            }
            
        }

        private void leaveLabel(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            if(label != selectedBoardLabel && !(SettingsModel.AgainstAI && label == lblCpu) && !(!SettingsModel.AgainstAI && label == lblHuman))
            {
                ColorAnimation anim = new ColorAnimation(Colors.Transparent, TimeSpan.FromMilliseconds(200));
                label.Background = new SolidColorBrush((Color)label.Background.GetValue(SolidColorBrush.ColorProperty));
                label.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
            }
        }
    }
}
