﻿using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for GameOverWindow.xaml
    /// </summary>
    public partial class GameOverWindow : UserControl
    {
        static string[] pointImages;
        private string ainame;

        public event RoutedEventHandler ClickedRestart;
        public GameOverWindow()
        {
            if (pointImages == null)
            {
                pointImages = Directory.GetFiles("Game\\Score\\3X");

            }
            InitializeComponent();
            switch (SettingsModel.Theme)
            {
                case 0:
                    ainame = "LE CHIFFRE";
                    break;
                case 1:
                    ainame = "TEAM ROCKET";
                    break;
                case 2:
                    ainame = "DEEP THOUGHT";
                    break;
                case 3:
                    ainame = "HAL 9000";
                    break;
                default:
                    ainame = "";
                    break;

            }
            
        }

        public void updateScore(int player0Score, int player1Score)
        {
            scorePlayer0.Source = new BitmapImage(new Uri(pointImages[player0Score], UriKind.Relative));
            scorePlayer1.Source = new BitmapImage(new Uri(pointImages[player1Score], UriKind.Relative));

            // Update content
            labelPlayerName0.Content = SettingsModel.TopPlayerName;

            if (SettingsModel.AgainstAI)
            {
             
                labelPlayerName1.Content = ainame;
                
               
            }
            else
            {
                labelPlayerName1.Content = SettingsModel.BottomPlayerName;
            }
        }


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
                    animation.Completed += (sender, eArgs) => 
                    { 
                        base.Visibility = value;
                        if (base.Visibility == Visibility.Visible)
                            SoundControl.Player.playSound(SoundControl.SoundType.GameOver, 1, true);
                    };
                    this.BeginAnimation(OpacityProperty, animation);
                    this.Opacity = (double)animation.To;
                }
            }
        }

        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Settings);
        }

        private void btReset_Click(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Game);
            if (ClickedRestart != null)
                ClickedRestart(this, e);
        }
    }
}
