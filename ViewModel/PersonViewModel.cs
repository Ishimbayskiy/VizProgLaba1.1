using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using VizProgLaba1._1.Helper;
using VizProgLaba1._1.Model;
using VizProgLaba1._1.View;

namespace VizProgLaba1._1.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        private PersonDpo _selectedPersonDpo;

        public PersonDpo SelectedPersonDpo
        {
            get { return _selectedPersonDpo; }
            set
            {
                _selectedPersonDpo = value;
                OnPropertyChanged("SelectedPersonDpo");
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public ObservableCollection<Person> ListPerson { get; private set; }
        public ObservableCollection<PersonDpo> ListPersonDpo { get; private set; }

        public string Error { get; private set; }
        public string Message { get; private set; }

        public PersonViewModel()
        {
            ListPerson = GetPersons();
            ListPersonDpo = GetListPersonDpo();
        }

        private ObservableCollection<Person> GetPersons()
        {
            var listPerson = new ObservableCollection<Person>();
            using (var context = new CompanyEntities())
            {
                var query = from person in context.Persons.Include("Role")
                            orderby person.LastName
                            select person;

                foreach (Person person in query)
                {
                    listPerson.Add(person);
                }
            }

            return listPerson;
        }

        private ObservableCollection<PersonDpo> GetListPersonDpo()
        {
            var listPersonDpo = new ObservableCollection<PersonDpo>();
            foreach (Person person in ListPerson)
            {
                listPersonDpo.Add(new PersonDpo().CopyFromPerson(person));
            }

            return listPersonDpo;
        }

        private void ReloadPersons(int? selectedId = null)
        {
            ListPerson = GetPersons();
            ListPersonDpo = GetListPersonDpo();
            OnPropertyChanged("ListPerson");
            OnPropertyChanged("ListPersonDpo");
            SelectedPersonDpo = selectedId.HasValue
                ? ListPersonDpo.FirstOrDefault(person => person.Id == selectedId.Value)
                : null;
        }

        private RelayCommand _addPerson;
        public RelayCommand AddPerson
        {
            get
            {
                return _addPerson ?? (_addPerson = new RelayCommand(obj =>
                {
                    PersonDpo personDpo = new PersonDpo
                    {
                        Birthday = DateTime.Now.ToString(
                            "dd.MM.yyyy",
                            CultureInfo.GetCultureInfo("ru-RU"))
                    };

                    WindowNewEmployee wnPerson = null;
                    wnPerson = new WindowNewEmployee
                    {
                        Title = "Новый сотрудник"
                    };
                    wnPerson.DataContext = new WindowNewEmployeeViewModel(
                        personDpo,
                        result => wnPerson.DialogResult = result);

                    if (wnPerson.ShowDialog() != true)
                    {
                        return;
                    }

                    Role role = wnPerson.CbRole.SelectedItem as Role;
                    if (role == null)
                    {
                        Message = "Необходимо выбрать должность сотрудника.";
                        OnPropertyChanged("Message");
                        return;
                    }

                    try
                    {
                        using (var context = new CompanyEntities())
                        {
                            Person newPerson = new Person
                            {
                                RoleId = role.Id,
                                FirstName = personDpo.FirstName,
                                LastName = personDpo.LastName,
                                Birthday = PersonDpo.GetDateTimeBirthday(
                                    PersonDpo.GetStringBirthday(personDpo.Birthday)),
                                Role = context.Roles.Find(role.Id)
                            };

                            context.Persons.Add(newPerson);
                            context.SaveChanges();
                            ReloadPersons(newPerson.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = "Ошибка добавления сотрудника\n" + ex.Message;
                        OnPropertyChanged("Error");
                    }
                }));
            }
        }

        private RelayCommand _editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return _editPerson ?? (_editPerson = new RelayCommand(obj =>
                {
                    PersonDpo selectedPerson = SelectedPersonDpo;
                    if (selectedPerson == null)
                    {
                        return;
                    }

                    PersonDpo tempPerson = selectedPerson.ShallowCopy();
                    WindowNewEmployee wnPerson = null;
                    wnPerson = new WindowNewEmployee
                    {
                        Title = "Редактирование данных сотрудника"
                    };
                    wnPerson.DataContext = new WindowNewEmployeeViewModel(
                        tempPerson,
                        result => wnPerson.DialogResult = result);

                    if (wnPerson.ShowDialog() != true)
                    {
                        return;
                    }

                    Role role = wnPerson.CbRole.SelectedItem as Role;
                    if (role == null)
                    {
                        Message = "Необходимо выбрать должность сотрудника.";
                        OnPropertyChanged("Message");
                        return;
                    }

                    try
                    {
                        using (var context = new CompanyEntities())
                        {
                            Person person = context.Persons.Find(selectedPerson.Id);
                            if (person != null)
                            {
                                person.RoleId = role.Id;
                                person.FirstName = tempPerson.FirstName;
                                person.LastName = tempPerson.LastName;
                                person.Birthday = PersonDpo.GetDateTimeBirthday(
                                    PersonDpo.GetStringBirthday(tempPerson.Birthday));
                                context.SaveChanges();
                            }
                        }

                        ReloadPersons(selectedPerson.Id);
                    }
                    catch (Exception ex)
                    {
                        Error = "Ошибка редактирования сотрудника\n" + ex.Message;
                        OnPropertyChanged("Error");
                    }
                }, (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
            }
        }

        private RelayCommand _deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return _deletePerson ?? (_deletePerson = new RelayCommand(obj =>
                {
                    PersonDpo selectedPerson = SelectedPersonDpo;
                    if (selectedPerson == null)
                    {
                        return;
                    }

                    MessageBoxResult result = MessageBox.Show(
                        "Удалить данные по сотруднику:\n" +
                        selectedPerson.LastName + " " + selectedPerson.FirstName,
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
                            Person person = context.Persons.Find(selectedPerson.Id);
                            if (person != null)
                            {
                                context.Persons.Remove(person);
                                context.SaveChanges();
                            }
                        }

                        ReloadPersons();
                    }
                    catch (Exception ex)
                    {
                        Error = "Ошибка удаления сотрудника\n" + ex.Message;
                        OnPropertyChanged("Error");
                    }
                }, (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
            }
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
