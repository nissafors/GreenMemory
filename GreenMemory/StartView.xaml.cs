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
using System.Windows.Media.Animation;
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
            string toolTipSettings = "BOARD SIZE : ";
            if (SettingsModel.Rows == 4)
                toolTipSettings += "SMALL\n";
            else if (SettingsModel.Rows == 5)
                toolTipSettings += "MEDIUM\n";
            else
                toolTipSettings += "LARGE\n";

            if (SettingsModel.AgainstAI)
            {
                toolTipSettings += "AI LEVEL : " + SettingsModel.AILevel + "\n";
            }
            else
                toolTipSettings += "TWO PLAYER MODE\n";

            toolTipSettings += "THEME : ";
            if (SettingsModel.Theme == 0)
                toolTipSettings += "CARDS\n";
            else if (SettingsModel.Theme == 1)
                toolTipSettings += "POKEMON\n";
            else toolTipSettings += "NERD\n";

            toolTipSettings += "SOUND : " + (SettingsModel.Sound ? "ON" : "OFF") + "\n";
            toolTipSettings += "MUSIC : " + (SettingsModel.Music ? "ON" : "OFF") + "\n";
            lblToolTip.Content = toolTipSettings;
        }

        private void quickstart(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Game);
        }

        private void settings(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Settings);
        }

        private void te_MouseEnter(object sender, MouseEventArgs e)
        {
            lblToolTip.Visibility = Visibility.Visible;
            DoubleAnimation fadeIn = new DoubleAnimation();
            fadeIn.From = 0;
            fadeIn.To = 1;
            fadeIn.Duration = TimeSpan.FromMilliseconds(500);
            fadeIn.FillBehavior = FillBehavior.Stop;
            lblToolTip.BeginAnimation(OpacityProperty, fadeIn);
            lblToolTip.Opacity = 1;
            
        }

        private void te_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation fadeOut = new DoubleAnimation();
            fadeOut.From = lblToolTip.Opacity;
            fadeOut.To = 0;
            fadeOut.Duration = TimeSpan.FromMilliseconds(250);
            fadeOut.FillBehavior = FillBehavior.Stop;
            lblToolTip.BeginAnimation(OpacityProperty, fadeOut);
            lblToolTip.Opacity = 0;
            fadeOut.Completed += (s, eArgs) => { lblToolTip.Visibility = Visibility.Hidden; };
        }
    }
}
