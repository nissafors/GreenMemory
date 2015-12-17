using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Tests.Run();

            (mainGrid.Children[0] as StartView).btnQuickStart.Click += ChangeState;
            (mainGrid.Children[0] as StartView).btnStart.Click += ChangeState;
        }

        private void ChangeState(object sender, RoutedEventArgs e)
        {
            if(sender == (mainGrid.Children[0] as StartView).btnQuickStart)
            {
                mainGrid.Children.Remove(mainGrid.Children[0]);
                mainGrid.Children.Add(new GameView(4, 4));
            }
            else if(sender == (mainGrid.Children[0] as StartView).btnStart)
            {
                mainGrid.Children.Remove(mainGrid.Children[0]);
                mainGrid.Children.Add(new GameView(4, 4));
            }
        }
    }
}
