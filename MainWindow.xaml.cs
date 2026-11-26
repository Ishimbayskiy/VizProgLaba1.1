using System.Windows;
using SortingComparison.ViewModel;

namespace SortingComparison
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
