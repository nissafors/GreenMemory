using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GreenMemory
{
    /// <summary>
    /// This class handles sound playback
    /// get the singelton instance by calling SoundControl.Player
    /// </summary>
    public class SoundControl
    {
        private static SoundControl singelTon;

        private MediaPlayer musicPlayer = new MediaPlayer();
        // This is a list of sounds currently playing, this prevents playing sounds from being gc collected
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
                        // replay
                        singelTon.musicPlayer.MediaEnded += (sender, eArgs) => { singelTon.musicPlayer.Position = TimeSpan.Zero; singelTon.musicPlayer.Play(); };
                    }
                        
                    return singelTon;
                }
        }

        /// <summary>
        /// Plays a looping background music
        /// </summary>
        /// <param name="path"></param>
        /// <param name="volume"></param>
        public void playMusic(string path, double volume = 1)
        {
            musicPlayer.Close();
            musicPlayer.Open(new Uri(path, UriKind.Relative));

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
        public void playSound(string path, double volume = 1)
        {
            MediaPlayer soundPlayer = new MediaPlayer();
            soundPlayer.Open(new Uri(path, UriKind.Relative));
            soundPlayer.Volume = volume;
            soundPlayer.Play();

            activeSoundPlayers.Add(soundPlayer);
            soundPlayer.MediaEnded += (sender, eArgs) => {activeSoundPlayers.Remove(soundPlayer);}; // all gc to free memory once sound has finished playing
        }
    }
}
