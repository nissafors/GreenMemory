using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Tests.Run();

            (mainGrid.Children[0] as StartView).btnQuickStart.Click += ChangeState;
            (mainGrid.Children[0] as StartView).btnStart.Click += ChangeState;

            // TODO: Get settings from saved file or set to default.
            SettingsModel.Columns = 4;
            SettingsModel.Rows = 4;
            SettingsModel.AgainstAI = true;
        }

        private void ChangeState(object sender, RoutedEventArgs e)
        {
            if (mainGrid.Children[0] is StartView)
            {
                if (sender == (mainGrid.Children[0] as StartView).btnQuickStart)
                {
                    mainGrid.Children.Remove(mainGrid.Children[0]);
                    mainGrid.Children.Add(new GameView());
                }
                else if (sender == (mainGrid.Children[0] as StartView).btnStart)
                {
                    mainGrid.Children.Remove(mainGrid.Children[0]);
                    mainGrid.Children.Add(new SettingsView());

                    (mainGrid.Children[0] as SettingsView).btnPlay.Click += ChangeState;
                }
            }
            else if (mainGrid.Children[0] is SettingsView)
            {
                if (sender == (mainGrid.Children[0] as SettingsView).btnPlay)
                {
                    mainGrid.Children.Remove(mainGrid.Children[0]);
                    mainGrid.Children.Add(new GameView());
                }
            }
        }
    }
}
