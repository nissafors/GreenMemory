using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl, INotifyPropertyChanged
    {
        private const int NUMBER_OF_THEMES = 4;

        // seleted board size
        Label selectedBoardLabel = null;

        // Theme fader rectangle visibility properties. These trigger PropertyChanged events when updated.
        private Visibility[] themeFadeVisibilities = new Visibility[NUMBER_OF_THEMES];

        public Visibility Theme0FadeVisibility
        {
            get { return themeFadeVisibilities[0]; }
            set
            {
                if (themeFadeVisibilities[0] != value)
                {
                    themeFadeVisibilities[0] = value;
                    NotifyPropertyChanged("theme0FadeVisibility");
                }
            }
        }
        public Visibility Theme1FadeVisibility
        {
            get { return themeFadeVisibilities[1]; }
            set
            {
                if (themeFadeVisibilities[1] != value)
                {
                    themeFadeVisibilities[1] = value;
                    NotifyPropertyChanged("theme1FadeVisibility");
                }
            }
        }
        public Visibility Theme2FadeVisibility
        {
            get { return themeFadeVisibilities[2]; }
            set
            {
                if (themeFadeVisibilities[2] != value)
                {
                    themeFadeVisibilities[2] = value;
                    NotifyPropertyChanged("theme2FadeVisibility");
                }
            }
        }
        public Visibility Theme3FadeVisibility
        {
            get { return themeFadeVisibilities[3]; }
            set
            {
                if (themeFadeVisibilities[3] != value)
                {
                    themeFadeVisibilities[3] = value;
                    NotifyPropertyChanged("theme3FadeVisibility");
                }
            }
        }

        // Constructor
        public SettingsView()
        {
            InitializeComponent();
            // stop all sounds
            SoundControl.Player.stopMusic();
            // mark labels
            switch (SettingsModel.Rows)
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

            setThemeFadeVisibilities();

            if (SettingsModel.AgainstAI)
                selectLabel(lblCpu);
            else
                selectLabel(lblHuman);

            updateAICircles(SettingsModel.SettingsType.AIDifficulty);
            SettingsModel.AddChangeSettingsListener(updateAICircles);
        }

        // Set theme fader rectangles visibilities based on SettingsModel.Theme
        private void setThemeFadeVisibilities()
        {
            Theme0FadeVisibility = SettingsModel.Theme == 0 ? Visibility.Hidden : Visibility.Visible;
            Theme1FadeVisibility = SettingsModel.Theme == 1 ? Visibility.Hidden : Visibility.Visible;
            Theme2FadeVisibility = SettingsModel.Theme == 2 ? Visibility.Hidden : Visibility.Visible;
            Theme3FadeVisibility = SettingsModel.Theme == 3 ? Visibility.Hidden : Visibility.Visible;
        }

        private void deSelectLabel(Label label)
        {
            label.Foreground = Brushes.White;
            label.Background = Brushes.Transparent;
        }

        private void selectLabel(Label label)
        {
            deSelectLabel(label);
            var bc = new BrushConverter();
            label.Background = (Brush)bc.ConvertFrom("#FFFFFF");
            label.Foreground = Brushes.Black;
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
            if (SettingsModel.AgainstAI == false)
            {
                deSelectLabel(lblHuman);
                selectLabel(lblCpu);
                SettingsModel.AgainstAI = true;
            }
            else
            {
                SettingsModel.AILevel = (AIModel.Difficulty)(((int)SettingsModel.AILevel + 1) % 3);
            }
        }

        private void setTwoPlayer(object sender, MouseButtonEventArgs e)
        {
            deSelectLabel(lblCpu);
            selectLabel(lblHuman);
            SettingsModel.AgainstAI = false;
        }

        private void hoverLabel(object sender, MouseEventArgs e)
        {
            Label label;
            if (sender == MediumEllipse || sender == EasyEllipse || sender == HardEllipse)
            {
                label = lblCpu;
            }
            else
            {
                label = sender as Label;
            }

            if (label != selectedBoardLabel && !(SettingsModel.AgainstAI && label == lblCpu) && !(!SettingsModel.AgainstAI && label == lblHuman))
            {
                ColorAnimation anim = new ColorAnimation(Colors.White, TimeSpan.FromMilliseconds(100));
                label.Background = new SolidColorBrush((Color)label.Background.GetValue(SolidColorBrush.ColorProperty)); // annars kastas exception
                label.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                label.Foreground = Brushes.Black;
            }

        }

        private void leaveLabel(object sender, MouseEventArgs e)
        {
            Label label;
            if (sender == MediumEllipse || sender == EasyEllipse || sender == HardEllipse)
            {
                label = lblCpu;
            }
            else
            {
                label = sender as Label;
            }

            if (label != selectedBoardLabel && !(SettingsModel.AgainstAI && label == lblCpu) && !(!SettingsModel.AgainstAI && label == lblHuman))
            {
                ColorAnimation anim = new ColorAnimation(Colors.Transparent, TimeSpan.FromMilliseconds(200));
                label.Background = new SolidColorBrush((Color)label.Background.GetValue(SolidColorBrush.ColorProperty));
                label.Background.BeginAnimation(SolidColorBrush.ColorProperty, anim);
                label.Foreground = Brushes.White;
            }
        }

        private void chooseTheme(object sender, MouseButtonEventArgs e)
        {
            if (sender == cardGrid.Children[0])
            {
                SettingsModel.Theme = 0;
            }
            else if (sender == cardGrid.Children[1])
            {
                SettingsModel.Theme = 1;
            }
            else if (sender == cardGrid.Children[2])
            {
                SettingsModel.Theme = 2;
            }
            else if (sender == cardGrid.Children[3])
            {
                SettingsModel.Theme = 3;
            }

            // Update themes fade effect rectangles visibility
            setThemeFadeVisibilities();

            CardView.UpdateBackground();
        }

        private void showSettingsWindow(object sender, RoutedEventArgs e)
        {
            settingsWin.Visibility = Visibility.Visible;
        }

        // INotifyPropertyChanged: Fires when a property notifies a change value.
        public event PropertyChangedEventHandler PropertyChanged;

        // Call this method to trigger PropertyChanged event.
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void updateAICircles(SettingsModel.SettingsType type)
        {
            if (SettingsModel.AgainstAI == true)
            {
                switch (SettingsModel.AILevel)
                {
                    case AIModel.Difficulty.Easy:
                        EasyEllipse.Fill = new SolidColorBrush(Color.FromArgb(0xB0, 0x22, 0xFB, 0x00));
                        MediumEllipse.Fill = Brushes.Transparent;
                        HardEllipse.Fill = Brushes.Transparent;
                        break;

                    case AIModel.Difficulty.Medium:
                        EasyEllipse.Fill = new SolidColorBrush(Color.FromArgb(0xB0, 0x22, 0xFB, 0x00));
                        MediumEllipse.Fill = new SolidColorBrush(Color.FromArgb(0xB0, 0xFA, 0xFA, 0x00));
                        HardEllipse.Fill = Brushes.Transparent;
                        break;

                    case AIModel.Difficulty.Hard:
                        EasyEllipse.Fill = new SolidColorBrush(Color.FromArgb(0xB0, 0x22, 0xFB, 0x00));
                        MediumEllipse.Fill = new SolidColorBrush(Color.FromArgb(0xB0, 0xFA, 0xFA, 0x00));
                        HardEllipse.Fill = new SolidColorBrush(Color.FromArgb(0xB0, 0xFF, 0x0D, 0x00));
                        break;
                }
            }
            if (SettingsModel.AgainstAI == false)
            {
                EasyEllipse.Fill = Brushes.Transparent;
                MediumEllipse.Fill = Brushes.Transparent;
                HardEllipse.Fill = Brushes.Transparent;
            }
        }

        private void setDifficulty(object sender, MouseButtonEventArgs e)
        {
            deSelectLabel(lblHuman);
            selectLabel(lblCpu);
            SettingsModel.AgainstAI = true;

            if (sender == EasyEllipse)
            {
                SettingsModel.AILevel = AIModel.Difficulty.Easy;
            }
            else if(sender == MediumEllipse)
            {
                SettingsModel.AILevel = AIModel.Difficulty.Medium;
            }
            else if(sender == HardEllipse)
            {
                SettingsModel.AILevel = AIModel.Difficulty.Hard;
            }
        }
    }
}
