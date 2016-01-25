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
            if (!SettingsModel.readSettingsFromFile())
            {
                SettingsModel.Columns = 4;
                SettingsModel.Rows = 4;
                SettingsModel.AgainstAI = true;
                SettingsModel.Sound = true;
                SettingsModel.Music = true;
                SettingsModel.AILevel = AIModel.Difficulty.Medium;
                SettingsModel.Theme = 0;
                SettingsModel.CardImagePath = "Game\\Poker\\";
                SettingsModel.GameviewBackgroundPath = "Backgrounds\\Filt Background.png";
                SettingsModel.TopPlayerName = "PLAYER ONE";
                SettingsModel.BottomPlayerName = "PLAYER TWO";
            }

            InitializeComponent();
            Tests.Run();
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
                    this.Content = new StartView();
                    break;
                case View.Settings:
                    this.Content = new SettingsView();
                    break;
                case View.Game:
                    this.Content = new GameView();
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
