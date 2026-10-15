
using System.Windows;
using VizProgLaba1._1.ViewModel;

namespace VizProgLaba1._1.View
{
    public partial class WindowRole : Window
    {
        public WindowRole()
        {
            InitializeComponent();
            DataContext = new RoleViewModel();
        }
    }
}
