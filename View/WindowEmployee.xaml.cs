
using System.Windows;
using VizProgLaba1._1.ViewModel;

namespace VizProgLaba1._1.View
{
    public partial class WindowEmployee : Window
    {
        public WindowEmployee()
        {
            InitializeComponent();

            DataContext = new PersonViewModel();
        }
    }
}
