
using System.Windows;
using VizProgLaba1._1.ViewModel;

namespace VizProgLaba1._1.View
{
    public partial class WindowEmployer : Window
    {
        public WindowEmployer()
        {
            InitializeComponent();

            DataContext = new PersonViewModel();
        }
    }
}
