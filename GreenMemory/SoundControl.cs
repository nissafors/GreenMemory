using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GreenMemory
{
    /// <summary>
    /// This class handles sound playback
    /// get the singelton instance by calling SoundControl.Player
    /// </summary>
    public class SoundControl
    {
        public enum SoundType { Flip, MoveCard, GameOver, Click };

        private static SoundControl singelTon;

        private MediaPlayer musicPlayer = new MediaPlayer();
        // This is a list of sounds currently playing, this prevents playing sounds from being garbage collected
        private List<MediaPlayer> activeSoundPlayers = new List<MediaPlayer>();
        /// <summary>
        /// Gets the current instance of the soundPlayer
        /// </summary>
        public static SoundControl Player
        {
            get {
                    if(singelTon == null)
                    {
                        singelTon = new SoundControl();
                        SettingsModel.AddChangeSettingsListener(singelTon.musicHandler);
                        // replay
                        singelTon.musicPlayer.MediaEnded += (sender, eArgs) => { singelTon.musicPlayer.Position = TimeSpan.Zero; singelTon.musicPlayer.Play(); };
                    }
                        
                    return singelTon;
                }
        }

        // React to settings
        private void musicHandler(SettingsModel.SettingsType type)
        {
            if(type == SettingsModel.SettingsType.Music)
            {
                if (!SettingsModel.Music)
                    stopMusic();
                else
                {
                    // only play music if we're playing the game
                    if(((MainWindow)Application.Current.MainWindow).Content is GameView)
                    {
                        playMusic();
                    }
                }      
            }
        }
        /// <summary>
        /// Plays a looping background music
        /// </summary>
        /// <param name="volume"></param>
        public void playMusic(double volume = 1)
        {
                musicPlayer.Close();
                
                // If sound is not found look for Common
                Uri url;
                if(File.Exists(SettingsModel.SoundPath + "music.mp3"))
                    url = new Uri(SettingsModel.SoundPath + "music.mp3", UriKind.Relative);
                else
                    url = new Uri("Game/Sounds/Common/music.mp3", UriKind.Relative);

                musicPlayer.Open(url);

                musicPlayer.Volume = volume;
                musicPlayer.Play();
        }

        /// <summary>
        /// Stops the current playing music
        /// </summary>
        public void stopMusic()
        {
            musicPlayer.Stop();
            musicPlayer.Close();
        }

        /// <summary>
        /// Plays a single sound to the end, each call
        /// creates a new mediaplayer, allowing multiple sounds at the same time
        /// </summary>
        /// <param name="path"></param>
        /// <param name="volume"></param>
        /// <param name="blocking"></param>
        public void playSound(SoundType type, double volume = 1, bool blocking = false)
        {
            if(SettingsModel.Sound)
            {
                // If blocking is true stop all playing sounds
                if(blocking)
                {
                    foreach(MediaPlayer p in activeSoundPlayers)
                    {
                        p.Stop();
                        p.Close();
                    }
                    activeSoundPlayers.Clear();
                }


                // Choose correct filename
                string str = string.Empty;

                switch(type)
                {
                    case SoundType.Flip:
                        str = "flip.wav";
                        break;
                    case SoundType.Click:
                        str = "click.wav";
                        break;
                    case SoundType.GameOver:
                        str = "gameover.wav";
                        break;
                    case SoundType.MoveCard:
                        str = "move.wav";
                        break;
                }
                // If sound is not found look for Common
                Uri url;
                if(File.Exists(SettingsModel.SoundPath + str))
                    url = new Uri(SettingsModel.SoundPath + str, UriKind.Relative);
                else
                    url = new Uri("Game/Sounds/Common/" + str, UriKind.Relative);

                MediaPlayer soundPlayer = new MediaPlayer();
                soundPlayer.Open(url);
                soundPlayer.Volume = volume;
                soundPlayer.Play();

                activeSoundPlayers.Add(soundPlayer);
                soundPlayer.MediaEnded += (sender, eArgs) => { activeSoundPlayers.Remove(soundPlayer); }; // allow the garbage collector to free memory
            }
            
        }
    }
}
