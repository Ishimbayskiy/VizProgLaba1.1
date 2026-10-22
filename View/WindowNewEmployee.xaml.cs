
using System.Windows;
using VizProgLaba1._1.ViewModel;

namespace VizProgLaba1._1.View
{
    public partial class WindowNewEmployee : Window
    {
        public WindowNewEmployee()
        {
            InitializeComponent();

            RoleViewModel vmRole = new RoleViewModel();

            CbRole.ItemsSource = vmRole.ListRole;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CbRole.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите должность!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
