
using System.Windows;
using VizProgLaba1._1.ViewModel;

namespace VizProgLaba1._1.View
{
    public partial class WindowNewEmployer : Window
    {
        public WindowNewEmployer()
        {
            InitializeComponent();

            RoleViewModel vmRole = new RoleViewModel();

            CbRole.ItemsSource = vmRole.ListRole;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbLastName.Text))
            {
                MessageBox.Show(
                    "Введите фамилию сотрудника!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(tbFirstName.Text))
            {
                MessageBox.Show(
                    "Введите имя сотрудника!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (CbRole.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите должность!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (dpBirthday.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату рождения!",
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
