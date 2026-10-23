using System;
using System.IO;
using Newtonsoft.Json;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VizProgLaba1._1.Model;
using VizProgLaba1._1.Helper;
using VizProgLaba1._1.View;
using System.Windows;
using System.Windows.Input;
namespace VizProgLaba1._1.ViewModel
{
    public class RoleViewModel : INotifyPropertyChanged
    {
        readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "DataModels", "RoleData.json");
        string _jsonRoles = String.Empty;
        public string Error { get; set; }

        private Role _selectedRole;


        public Role SelectedRole
        {
            get
            {
                return _selectedRole;
            }
            set
            {
                _selectedRole = value;
                OnPropertyChanged("SelectedRole");
                EditRole.CanExecute(true);
            }
        }


        public ObservableCollection<Role> ListRole { get; set; }
            = new ObservableCollection<Role>();

        public RoleViewModel()
        {
            ListRole = LoadRole();
        }

        public int MaxId()
        {
            int max = 0;

            foreach (var r in ListRole)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                }

            }

            return max;
        }

        private RelayCommand _addRole;


        public RelayCommand AddRole
        {
            get
            {
                return _addRole ??
                     (_addRole = new RelayCommand(obj =>
                     {
                         WindowNewRole wnRole = new WindowNewRole
                         {
                             Title = "Новая должность",
                         };

                         // формирование кода новой должности
                         int maxIdRole = MaxId() + 1;

                         Role role = new Role { Id = maxIdRole };

                         wnRole.DataContext = new WindowNewRoleViewModel(
                             role, result => wnRole.DialogResult = result);

                         if (wnRole.ShowDialog() == true)
                         {
                             ListRole.Add(role);
                             SaveChanges(ListRole);
                         }

                         SelectedRole = role;
                     }));
            }
        }


        private RelayCommand _editRole;

        public RelayCommand EditRole
        {
            get
            {
                return _editRole ??
                    (_editRole = new RelayCommand(obj =>
                    {
                        WindowNewRole wnRole = new WindowNewRole
                        {
                            Title = "Редактирование должности"
                        };

                        Role role = SelectedRole;

                        Role tempRole = new Role();
                        tempRole = role.ShallowCopy();

                        wnRole.DataContext = new WindowNewRoleViewModel(
                            tempRole, result => wnRole.DialogResult = result);

                        if (wnRole.ShowDialog() == true)
                        {
                            role.NameRole = tempRole.NameRole;
                            SaveChanges(ListRole);
                        }

                    }, (obj) => SelectedRole != null &&
                                ListRole.Count > 0));
            }
        }

        private RelayCommand _deleteRole;

        public RelayCommand DeleteRole
        {
            get
            {
                return _deleteRole ??
                    (_deleteRole = new RelayCommand(obj =>
                    {
                        Role role = SelectedRole;

                        MessageBoxResult result = MessageBox.Show(
                            "Удалить данные по должности: " +
                            role.NameRole,
                            "Предупреждение",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.OK)
                        {
                            ListRole.Remove(role);
                            SaveChanges(ListRole);
                        }

                    }, (obj) => SelectedRole != null &&
                                ListRole.Count > 0));
            }
        }

        #region Methods
        /// <summary>
        /// Загрузка JSON-файла с данными по должностям.
        /// </summary>
        public ObservableCollection<Role> LoadRole()
        {
            _jsonRoles = File.ReadAllText(path);
            ListRole = JsonConvert.DeserializeObject<ObservableCollection<Role>>(_jsonRoles)
                ?? new ObservableCollection<Role>();
            return ListRole;
        }

        /// <summary>
        /// Сохранение изменённой коллекции в JSON-файл.
        /// </summary>
        private void SaveChanges(ObservableCollection<Role> listRole)
        {
            var jsonRole = JsonConvert.SerializeObject(listRole);
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonRole);
                }
                Error = string.Empty;
            }
            catch (IOException e)
            {
                Error = "Ошибка записи JSON-файла\n" + e.Message;
                MessageBox.Show(Error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException e)
            {
                Error = "Ошибка записи JSON-файла\n" + e.Message;
                MessageBox.Show(Error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
