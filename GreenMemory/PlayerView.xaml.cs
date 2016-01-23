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
        private const double ACTIVE_OPACITY = 1D;
        private const double INACTIVE_OPACITY = 0.2;

        public static double ActiveOpacity { get; private set; }
        public static double InactiveOpacity { get; private set; }

        double fromOpacity;
        public double FromOpacity {
            get { return fromOpacity; }
            private set
            {
                if (value != fromOpacity)
                {
                    fromOpacity = value;
                    NotifyPropertyChanged("FromOpacity");
                }
            }
        }
        double toOpacity;
        public double ToOpacity
        {
            get { return toOpacity; }
            private set
            {
                if (value != toOpacity)
                {
                    toOpacity = value;
                    NotifyPropertyChanged("ToOpacity");
                }
            }
        }

        
        // Triggers visual indication of if the player is active or not.
        private bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                if (value != active)
                {
                    active = value;
                    if (active)
                    {
                        FromOpacity = INACTIVE_OPACITY;
                        ToOpacity = ACTIVE_OPACITY;
                    }
                    else
                    {
                        FromOpacity = ACTIVE_OPACITY;
                        ToOpacity = INACTIVE_OPACITY;
                    }

                    NotifyPropertyChanged("Active");
                }
            }
        }

        static string[] pointImages;

        // Constructor
        public PlayerView()
        {
            if(pointImages == null)
            {
                 pointImages = Directory.GetFiles("Game\\Score\\3X");
            }
            InitializeComponent();

            ActiveOpacity = ACTIVE_OPACITY;
            InactiveOpacity = INACTIVE_OPACITY;
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
