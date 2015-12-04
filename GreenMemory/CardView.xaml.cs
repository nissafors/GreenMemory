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

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for CardView.xaml
    /// </summary>
    public partial class CardView : UserControl 
    {
        private static Brush backgroundImage = Brushes.Wheat;

        private int id;
        private Brush cardImage;

        public static Brush BackgroundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }


        public CardView(int id, Brush cardImage)
        {
            InitializeComponent();

            this.id = id;
            this.Background = backgroundImage;
            this.cardImage = cardImage;
        }

        public int Id
        {
            get { return id; }
        }

        public void FlipCard()
        {
            if (this.Background == backgroundImage)
                this.Background = cardImage;
            else
                this.Background = backgroundImage;
        }

        public bool IsUp()
        {
            return this.Background == cardImage;
        }
    }
}
