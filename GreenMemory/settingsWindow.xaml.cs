using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for settingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : UserControl
    {
        private const double HIDDEN =   0.0;
        private const double DISABLED = 0.2;
        private const double FADED =    0.5;
        private const double VISIBLE =  1.0;

        private static BitmapImage SOUNDONIMAGE =   new BitmapImage(new Uri("Game\\Icons\\3X\\Sound@3x.png", UriKind.Relative));
        private static BitmapImage SOUNDOFFIMAGE =  new BitmapImage(new Uri("Game\\Icons\\3X\\Mute@3x.png", UriKind.Relative));
        private static BitmapImage MUSICONIMAGE =   new BitmapImage(new Uri("Game\\Icons\\3X\\Music@3x.png", UriKind.Relative));
        private static BitmapImage MUSICOFFIMAGE =  new BitmapImage(new Uri("Game\\Icons\\3X\\No Music@3x.png", UriKind.Relative));
        private static BitmapImage EASYAIIMAGE =    new BitmapImage(new Uri("Game\\Icons\\3X\\AI1@3x.png", UriKind.Relative));
        private static BitmapImage MEDIUMAIIMAGE =  new BitmapImage(new Uri("Game\\Icons\\3X\\AI2@3x.png", UriKind.Relative));
        private static BitmapImage HARDAIIMAGE =    new BitmapImage(new Uri("Game\\Icons\\3X\\AI3@3x.png", UriKind.Relative));
        private static BitmapImage AIOFFIMAGE =     new BitmapImage(new Uri("Game\\Icons\\3X\\AI@3x.png", UriKind.Relative));

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            
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
                        updateButtons();
                        animation.From = HIDDEN;
                        animation.To = VISIBLE;
                    }
                    else
                    {
                        animation.From = VISIBLE;
                        animation.To = HIDDEN;
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
        private void updateButtons()
        {
            if (SettingsModel.Sound)
            {
                imgSound.Source = SOUNDONIMAGE;
            }
            else
            {
                imgSound.Source = SOUNDOFFIMAGE;
            }

            if (SettingsModel.Music)
            {
                imgMusic.Source = new BitmapImage(new Uri("Game\\Icons\\3X\\Music@3x.png", UriKind.Relative));
            }
            else
            {
                imgMusic.Source = new BitmapImage(new Uri("Game\\Icons\\3X\\No Music@3x.png", UriKind.Relative));
            }

            if (SettingsModel.AgainstAI)
            {
                imgAI.IsEnabled = true;
                imgAI.Opacity = FADED;

                if(SettingsModel.AILevel == AIModel.Difficulty.Easy)
                {
                    imgAI.Source = new BitmapImage(new Uri("Game\\Icons\\3X\\AI1@3x.png", UriKind.Relative));
                }
                else if( SettingsModel.AILevel == AIModel.Difficulty.Medium)
                {
                    imgAI.Source = new BitmapImage(new Uri("Game\\Icons\\3X\\AI2@3x.png", UriKind.Relative));
                }
                else
                {
                    imgAI.Source = new BitmapImage(new Uri("Game\\Icons\\3X\\AI3@3x.png", UriKind.Relative));
                }
            }
            else
            {
                imgAI.IsEnabled = false;
                imgAI.Source = new BitmapImage(new Uri("Game\\Icons\\3X\\AI@3x.png", UriKind.Relative));
                imgAI.Opacity = DISABLED;
            }

            if((this.Parent as Grid).Name == "gameGrid")
            {
                imgNewgame.IsEnabled = true;
                imgNewgame.Opacity = FADED;
            }
            else
            {
                imgNewgame.IsEnabled = false;
                imgNewgame.Opacity = DISABLED;
            }
        }
        
        /// <summary>
        /// Called when mouse enter New game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startHoverNewGame(object sender, MouseEventArgs e)
        {
            lblSettingsText.Content = "START NEW GAME";
            animateElementFade((sender as Image), (sender as Image).Opacity, VISIBLE);
            animateElementFade(lblSettingsText, lblSettingsText.Opacity, VISIBLE);
        }

        /// <summary>
        /// Called when mouse enter AI button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startHoverAI(object sender, MouseEventArgs e)
        {
            lblSettingsText.Content = SettingsModel.AILevel.ToString().ToUpper();
            animateElementFade((sender as Image), (sender as Image).Opacity, VISIBLE);
            animateElementFade(lblSettingsText, lblSettingsText.Opacity, VISIBLE);
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
                lblSettingsText.Content = "SOUND ON";
            }
            else
            {
                lblSettingsText.Content = "SOUND OFF";
            }
            animateElementFade((sender as Image), (sender as Image).Opacity, VISIBLE);
            animateElementFade(lblSettingsText, lblSettingsText.Opacity, VISIBLE);
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
                lblSettingsText.Content = "MUSIC ON";
            }
            else
            {
                lblSettingsText.Content = "MUSIC OFF";
            }
            animateElementFade((sender as Image), (sender as Image).Opacity, VISIBLE);
            animateElementFade(lblSettingsText, lblSettingsText.Opacity, VISIBLE);
        }

        /// <summary>
        /// Called when mouse leaves any of the settingsbuttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopHoverButton(object sender, MouseEventArgs e)
        {
            animateElementFade((sender as Image), (sender as Image).Opacity, FADED);
            animateElementFade(lblSettingsText, lblSettingsText.Opacity, HIDDEN);
        }

        /// <summary>
        /// Animates a fading of the settings text label. Opacity goes from 
        /// the from parameter to the to parameter
        /// </summary>
        /// <param name="from">Start opacity</param>
        /// <param name="to">End opacity</param>
        private void animateElementFade(UIElement element, double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                FillBehavior = FillBehavior.Stop,
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(200))
            };
            element.BeginAnimation(OpacityProperty, animation);
            element.Opacity = to;
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
            updateButtons();
        }

        /// <summary>
        /// Called when clicking the AI button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleDifficulty(object sender, RoutedEventArgs e)
        {
            SettingsModel.AILevel = (AIModel.Difficulty)(((int)SettingsModel.AILevel + 1) % 3);
            lblSettingsText.Content = SettingsModel.AILevel.ToString().ToUpper();
            updateButtons();
            imgAI.Opacity = VISIBLE;
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
            updateButtons();
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

        private void onWindowLoaded(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).KeyUp += toggleWindow;
            updateButtons();
        }
    }
}