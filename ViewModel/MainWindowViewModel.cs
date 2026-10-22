using System.Windows;
using VizProgLaba1._1.Helper;
using VizProgLaba1._1.View;

namespace VizProgLaba1._1.ViewModel
{
    public class MainWindowViewModel
    {
        /// <summary>
        /// команда открытия окна "Сотрудники"
        /// </summary>
        private RelayCommand openEmployee;
        public RelayCommand OpenEmployee
        {
            get
            {
                return openEmployee ??
                (openEmployee = new RelayCommand(obj =>
                {
                    WindowEmployee wnEmployee = new WindowEmployee
                    {
                        Title = "Сотрудники"
                    };
                    wnEmployee.ShowDialog();
                }));
            }
        }

        /// <summary>
        /// команда открытия окна "Должности"
        /// </summary>
        private RelayCommand openRole;
        public RelayCommand OpenRole
        {
            get
            {
                return openRole ??
                (openRole = new RelayCommand(obj =>
                {
                    WindowRole wnRole = new WindowRole
                    {
                        Title = "Должности"
                    };
                    wnRole.ShowDialog();
                }));
            }
        }

        /// <summary>
        /// команда выхода из приложения
        /// </summary>
        private RelayCommand exitApp;
        public RelayCommand ExitApp
        {
            get
            {
                return exitApp ??
                (exitApp = new RelayCommand(obj =>
                {
                    Application.Current.Shutdown();
                }));
            }
        }
    }
}

