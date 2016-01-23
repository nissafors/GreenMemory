using System;
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
using System.IO;
using System.ComponentModel;
using System.Globalization;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for PlayerView.xaml
    /// </summary>
    public partial class PlayerView : UserControl, INotifyPropertyChanged
    {
        static string[] pointImages;

        // Triggers visual indication of if the player is active or not.
        // (Opacity values are set by the BoolToOpacityConverter class.)
        private bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                if (value != active)
                {
                    active = value;
                    NotifyPropertyChanged("Active");
                }
            }
        }

        // Constructor
        public PlayerView()
        {
            if(pointImages == null)
            {
                 pointImages = Directory.GetFiles("Game\\Score\\3X");
            }
            InitializeComponent();
        }

        public void setPoints(int points)
        {
            imgScore.Source = new BitmapImage(new Uri(pointImages[points], UriKind.Relative));
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

    }
}
