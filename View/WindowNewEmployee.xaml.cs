using System.Windows;

namespace VizProgLaba1._1.View
{
    public partial class WindowNewEmployee : Window
    {
        public WindowNewEmployee()
        {
            InitializeComponent();
        }

        private void tbBirthday_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ClBirthday == null)
            {
                return;
            }

            if (tbBirthday.Visibility == Visibility.Hidden)
            {
                ClBirthday.Visibility = Visibility.Visible;
            }
            else
            {
                ClBirthday.Visibility = Visibility.Hidden;
            }
        }
    }
}
