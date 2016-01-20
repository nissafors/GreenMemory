﻿using System;
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
    /// Interaction logic for settingsWindow.xaml
    /// </summary>
    public partial class settingsWindow : UserControl
    {
        public settingsWindow()
        {
            InitializeComponent();
            updateButtonImages();
        }
        
        private void updateButtonImages()
        {
            if(SettingsModel.Sound)
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
        }

        private void startHoverButton(object sender, MouseEventArgs e = null)
        {
            if(sender == btnAI)
            {
                // TODO: Change text to depend on AI difficulty
                lblSettingsText.Content = "AI DIFFICULTY";
            }
            else if(sender == btnNewgame)
            {
                lblSettingsText.Content = "START NEW GAME";
            }
            else if(sender == btnMusic)
            {
                if (SettingsModel.Music)
                {
                    lblSettingsText.Content = "MUSIC IS ON";
                }
                else
                {
                    lblSettingsText.Content = "MUSIC IS OFF";
                }
            }
            else if(sender == btnSound)
            {
                if (SettingsModel.Sound)
                {
                    lblSettingsText.Content = "SOUND IS ON";
                }
                else
                {
                    lblSettingsText.Content = "SOUND IS OFF";
                }
            }
        }

        private void stopHoverButton(object sender, MouseEventArgs e)
        {
            lblSettingsText.Content = "SETTINGS";
        }

        private void toggleMusic(object sender, RoutedEventArgs e)
        {
            SettingsModel.Music = !SettingsModel.Music;
            updateButtonImages();
            startHoverButton(sender);
        }

        private void toggleDifficulty(object sender, RoutedEventArgs e)
        {
            // TODO: Toggle AI difficulty
        }

        private void toggleSound(object sender, RoutedEventArgs e)
        {
            SettingsModel.Sound = !SettingsModel.Sound;
            updateButtonImages();
            startHoverButton(sender);
        }
    }
}
