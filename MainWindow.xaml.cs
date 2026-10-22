using System.Windows;
using VizProgLaba1._1.ViewModel;

namespace VizProgLaba1._1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
