using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum View { Start, Settings, Game };

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Tests.Run();

            if (!SettingsModel.readSettingsFromFile())
            {
                SettingsModel.Columns = 4;
                SettingsModel.Rows = 4;
                SettingsModel.AgainstAI = true;
                SettingsModel.AILevel = 0.1;
            }
        }

        /// <summary>
        /// Changes the view being displayed.
        /// </summary>
        /// <param name="view"></param>
        public void ChangeView(View view)
        {
            switch (view)
            {
                case View.Start:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new StartView());
                    break;
                case View.Settings:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new SettingsView());
                    break;
                case View.Game:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new GameView());
                    break;
            }
        }

        /// <summary>
        /// Handler for closing the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindowClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsModel.writeSettingsToFile();
        }
    }
}
