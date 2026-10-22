using System.Windows;
using VizProgLaba1._1.View;

namespace VizProgLaba1._1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Employee_OnClick(object sender, RoutedEventArgs e)
        {
            WindowEmployee window = new WindowEmployee();
            window.Show();
        }

        private void Role_OnClick(object sender, RoutedEventArgs e)
        {
            WindowRole window = new WindowRole();
            window.Show();
        }
    }
}
