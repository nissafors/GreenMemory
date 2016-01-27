using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GreenMemory
{
    public class SoundPlayer
    {
        private static SoundPlayer singelTon;

        private bool musicIsPlaying = false;
        private bool soundIsPlaying = false;

        private MediaPlayer musicPlayer = new MediaPlayer();

        /// <summary>
        /// Gets the current instance of the soundPlayer
        /// </summary>
        public static SoundPlayer Player
        {
            get {
                    if(singelTon == null)
                        singelTon = new SoundPlayer();
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
            if(musicPlayer.)
        }
    }
}
