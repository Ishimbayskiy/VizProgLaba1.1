using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using VizProgLaba1._1.Helper;
using VizProgLaba1._1.Model;
using VizProgLaba1._1.View;

namespace VizProgLaba1._1.ViewModel
{
    public class RoleViewModel : INotifyPropertyChanged
    {
        private Role _selectedRole;

        public Role SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                OnPropertyChanged("SelectedRole");
                CommandManagerInvalidate();
            }
        }

        public ObservableCollection<Role> ListRole { get; private set; }

        public string Error { get; private set; }

        public RoleViewModel()
        {
            ListRole = GetRoles();
        }

        private ObservableCollection<Role> GetRoles()
        {
            var listRole = new ObservableCollection<Role>();
            using (var context = new CompanyEntities())
            {
                var query = from role in context.Roles
                            orderby role.NameRole
                            select role;

                foreach (Role role in query)
                {
                    listRole.Add(role);
                }
            }

            return listRole;
        }

        private void ReloadRoles(int? selectedId = null)
        {
            ListRole = GetRoles();
            OnPropertyChanged("ListRole");
            SelectedRole = selectedId.HasValue
                ? ListRole.FirstOrDefault(role => role.Id == selectedId.Value)
                : null;
        }

        private RelayCommand _addRole;
        public RelayCommand AddRole
        {
            get
            {
                return _addRole ?? (_addRole = new RelayCommand(obj =>
                {
                    Role newRole = new Role();
                    WindowNewRole wnRole = null;
                    wnRole = new WindowNewRole
                    {
                        Title = "Новая должность"
                    };
                    wnRole.DataContext = new WindowNewRoleViewModel(
                        newRole,
                        result => wnRole.DialogResult = result);

                    if (wnRole.ShowDialog() == true)
                    {
                        try
                        {
                            using (var context = new CompanyEntities())
                            {
                                context.Roles.Add(newRole);
                                context.SaveChanges();
                            }

                            ReloadRoles(newRole.Id);
                        }
                        catch (Exception ex)
                        {
                            Error = "Ошибка добавления должности\n" + ex.Message;
                            OnPropertyChanged("Error");
                        }
                    }
                }));
            }
        }

        private RelayCommand _editRole;
        public RelayCommand EditRole
        {
            get
            {
                return _editRole ?? (_editRole = new RelayCommand(obj =>
                {
                    Role editRole = SelectedRole;
                    if (editRole == null)
                    {
                        return;
                    }

                    Role tempRole = editRole.ShallowCopy();
                    WindowNewRole wnRole = null;
                    wnRole = new WindowNewRole
                    {
                        Title = "Редактирование должности"
                    };
                    wnRole.DataContext = new WindowNewRoleViewModel(
                        tempRole,
                        result => wnRole.DialogResult = result);

                    if (wnRole.ShowDialog() == true)
                    {
                        try
                        {
                            using (var context = new CompanyEntities())
                            {
                                Role role = context.Roles.Find(editRole.Id);
                                if (role != null)
                                {
                                    role.NameRole = tempRole.NameRole == null
                                        ? string.Empty
                                        : tempRole.NameRole.Trim();
                                    context.SaveChanges();
                                }
                            }

                            ReloadRoles(editRole.Id);
                        }
                        catch (Exception ex)
                        {
                            Error = "Ошибка редактирования должности\n" + ex.Message;
                            OnPropertyChanged("Error");
                        }
                    }
                }, (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }

        private RelayCommand _deleteRole;
        public RelayCommand DeleteRole
        {
            get
            {
                return _deleteRole ?? (_deleteRole = new RelayCommand(obj =>
                {
                    Role selectedRole = SelectedRole;
                    if (selectedRole == null)
                    {
                        return;
                    }

                    MessageBoxResult result = MessageBox.Show(
                        "Удалить данные по должности: " + selectedRole.NameRole,
                        "Предупреждение",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);

                    if (result != MessageBoxResult.OK)
                    {
                        return;
                    }

                    try
                    {
                        using (var context = new CompanyEntities())
                        {
                            Role role = context.Roles.Find(selectedRole.Id);
                            if (role != null)
                            {
                                context.Roles.Remove(role);
                                context.SaveChanges();
                            }
                        }

                        ReloadRoles();
                    }
                    catch (Exception ex)
                    {
                        Error = "Ошибка удаления должности\n" + ex.Message;
                        OnPropertyChanged("Error");
                    }
                }, (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }

        private static void CommandManagerInvalidate()
        {
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

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
