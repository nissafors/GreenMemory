using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for settingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            ((MainWindow)Application.Current.MainWindow).KeyUp += toggleWindow;
            updateButtonImages();
        }

        /// <summary>
        /// Gets and sets the visibilty of the settings window.
        /// Also makes the window fade in or out on setting visibilty.
        /// </summary>
        public new Visibility Visibility
        {
            get
            {
                return base.Visibility;
            }

            set
            {
                if (base.Visibility != value)
                {
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        FillBehavior = FillBehavior.Stop,
                        Duration = new Duration(TimeSpan.FromMilliseconds(250))
                    };

                    if (value == Visibility.Visible)
                    {
                        base.Visibility = value;
                        animation.From = 0.0;
                        animation.To = 1.0;
                    }
                    else
                    {
                        animation.From = 1.0;
                        animation.To = 0.0;
                    }
                    animation.Completed += (sender, eArgs) => { base.Visibility = value; };
                    this.BeginAnimation(OpacityProperty, animation);
                    this.Opacity = (double)animation.To;
                }
            }
        }

        /// <summary>
        /// Updates the images in the settings buttons depending on
        /// current settings in SettingsModel
        /// </summary>
        private void updateButtonImages()
        {
            if (SettingsModel.Sound)
            {
                btnSound.ButtonImage = new BitmapImage(new Uri("Game\\Icons\\3X\\Sound@3x.png", UriKind.Relative));
            }
            else
            {
                btnSound.ButtonImage = new BitmapImage(new Uri("Game\\Icons\\3X\\Mute@3x.png", UriKind.Relative));
            }

            if (SettingsModel.Music)
            {
                btnMusic.ButtonImage = new BitmapImage(new Uri("Game\\Icons\\3X\\Music@3x.png", UriKind.Relative));
            }
            else
            {
                btnMusic.ButtonImage = new BitmapImage(new Uri("Game\\Icons\\3X\\No Music@3x.png", UriKind.Relative));
            }

            if (SettingsModel.AgainstAI)
            {
                btnAI.IsEnabled = true;
            }
            else
            {
                btnAI.IsEnabled = false;
                btnAI.ButtonImage = new BitmapImage(new Uri("Game\\Icons\\3X\\AI@3x.png", UriKind.Relative));
            }
        }
        
        /// <summary>
        /// Called when mouse enter New game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startHoverNewGame(object sender, MouseEventArgs e)
        {
            // TODO: Change text to depend on AI difficulty
            lblSettingsText.Content = "START NEW GAME";
            animateHover(0.0, 1.0);
        }

        /// <summary>
        /// Called when mouse enter AI button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startHoverAI(object sender, MouseEventArgs e)
        {
            // TODO: Change text to depend on AI difficulty
            lblSettingsText.Content = "AI DIFFICULTY";
            animateHover(0.0, 1.0);
        }

        /// <summary>
        /// Called when mouse enter Sound button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startHoverSound(object sender, MouseEventArgs e)
        {
            if (SettingsModel.Sound)
            {
                lblSettingsText.Content = "SOUND IS ON";
            }
            else
            {
                lblSettingsText.Content = "SOUND IS OFF";
            }
            animateHover(0.0, 1.0);
        }

        /// <summary>
        /// Called when mouse enter Music button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startHoverMusic(object sender, MouseEventArgs e)
        {
            if (SettingsModel.Music)
            {
                lblSettingsText.Content = "MUSIC IS ON";
            }
            else
            {
                lblSettingsText.Content = "MUSIC IS OFF";
            }
            animateHover(0.0, 1.0);
        }

        /// <summary>
        /// Called when mouse leaves any of the settingsbuttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopHoverButton(object sender, MouseEventArgs e)
        {
            animateHover(1.0, 0.0);
        }

        /// <summary>
        /// Animates a fading of the settings text label. Opacity goes from 
        /// the from parameter to the to parameter
        /// </summary>
        /// <param name="from">Start opacity</param>
        /// <param name="to">End opacity</param>
        private void animateHover(double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                FillBehavior = FillBehavior.HoldEnd,
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(200))
            };
            lblSettingsText.BeginAnimation(OpacityProperty, animation);
            lblSettingsText.Opacity = (double)animation.To;
        }

        /// <summary>
        /// Called when clicking the Music button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleMusic(object sender, RoutedEventArgs e)
        {
            SettingsModel.Music = !SettingsModel.Music;
            
            if (SettingsModel.Music)
            {
                lblSettingsText.Content = "MUSIC IS ON";
            }
            else
            {
                lblSettingsText.Content = "MUSIC IS OFF";
            }
            updateButtonImages();
        }

        /// <summary>
        /// Called when clicking the AI button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleDifficulty(object sender, RoutedEventArgs e)
        {
            // TODO: Toggle AI difficulty
            // SettingsModel.AILevel = (SettingsModel.AILevel + 1) % 3;
            updateButtonImages();
        }

        /// <summary>
        /// Called when clicking the Sound button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleSound(object sender, RoutedEventArgs e)
        {
            SettingsModel.Sound = !SettingsModel.Sound;

            if (SettingsModel.Sound)
            {
                lblSettingsText.Content = "SOUND IS ON";
            }
            else
            {
                lblSettingsText.Content = "SOUND IS OFF";
            }
            updateButtonImages();
        }

        /// <summary>
        /// Called when clicking anywhere in the settings window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hideWindow(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(mainGrid);

            if (mainGrid.ColumnDefinitions[0].ActualWidth > mousePos.X ||
                (mainGrid.ColumnDefinitions[0].ActualWidth + mainGrid.ColumnDefinitions[1].ActualWidth) < mousePos.X ||
                mainGrid.RowDefinitions[0].ActualHeight > mousePos.Y ||
                (mainGrid.RowDefinitions[0].ActualHeight + mainGrid.RowDefinitions[1].ActualHeight) < mousePos.Y)
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called when pressing any keyboard button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleWindow(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Called when clicking the exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}