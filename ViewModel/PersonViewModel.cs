using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VizProgLaba1._1.Model;
using VizProgLaba1._1.Helper;
using VizProgLaba1._1.View;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
namespace VizProgLaba1._1.ViewModel

{
    public class PersonViewModel : INotifyPropertyChanged
    {
        private PersonDpo selectedPersonDpo;


        public PersonDpo SelectedPersonDpo
        {
            get
            {
                return selectedPersonDpo;
            }
            set
            {
                selectedPersonDpo = value;

                OnPropertyChanged("SelectedPersonDpo");

                CommandManager.InvalidateRequerySuggested();
            }
        }


        public ObservableCollection<Person> ListPerson { get; set; }
            = new ObservableCollection<Person>();

        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; }
            = new ObservableCollection<PersonDpo>();

        public PersonViewModel()
        {
            ListPerson.Add(new Person
            {
                Id = 1,
                RoleId = 1,
                FirstName = "Иван",
                LastName = "Иванов",
                Birthday = new DateTime(1980, 02, 28)
            });

            ListPerson.Add(new Person
            {
                Id = 2,
                RoleId = 2,
                FirstName = "Петр",
                LastName = "Петров",
                Birthday = new DateTime(1981, 03, 20)
            });

            ListPerson.Add(new Person
            {
                Id = 3,
                RoleId = 3,
                FirstName = "Виктор",
                LastName = "Викторов",
                Birthday = new DateTime(1982, 04, 15)
            });

            ListPerson.Add(new Person
            {
                Id = 4,
                RoleId = 3,
                FirstName = "Сидор",
                LastName = "Сидоров",
                Birthday = new DateTime(1983, 05, 10)
            });

            GetListPersonDpo();
        }

        public ObservableCollection<PersonDpo> GetListPersonDpo()
        {
            ListPersonDpo.Clear();

            foreach (var person in ListPerson)
            {
                PersonDpo p = new PersonDpo();

                p = p.CopyFromPerson(person);

                ListPersonDpo.Add(p);
            }

            return ListPersonDpo;
        }

        public int MaxId()
        {
            int max = 0;

            foreach (var p in ListPerson)
            {
                if (max < p.Id)
                {
                    max = p.Id;
                }
            }

            return max;
        }

        private RelayCommand addPerson;

        public RelayCommand AddPerson
        {
            get
            {
                return addPerson ??
                    (addPerson = new RelayCommand(obj =>
                    {
                        WindowNewEmployer wnPerson = new WindowNewEmployer
                        {
                            Title = "Новый сотрудник"
                        };

                        int maxIdPerson = MaxId() + 1;

                        PersonDpo per = new PersonDpo
                        {
                            Id = maxIdPerson,
                            Birthday = DateTime.Today
                        };

                        wnPerson.DataContext = per;


                        if (wnPerson.ShowDialog() == true)
                        {
                            Role r = (Role)wnPerson.CbRole.SelectedItem;

                            per.RoleName = r.NameRole;

                            // Добавление в коллекцию отображения
                            ListPersonDpo.Add(per);

                            // Добавление в коллекцию Person
                            Person p = new Person();
                            p = p.CopyFromPersonDPO(per);
                            ListPerson.Add(p);
                        }

                    }));
            }
        }

        private RelayCommand editPerson;

        public RelayCommand EditPerson
        {
            get
            {
                return editPerson ??
                    (editPerson = new RelayCommand(obj =>
                    {
                        WindowNewEmployer wnPerson = new WindowNewEmployer
                        {
                            Title = "Редактирование сотрудника"
                        };

                        PersonDpo personDpo = SelectedPersonDpo;

                        PersonDpo tempPerson = personDpo.ShallowCopy();

                        wnPerson.DataContext = tempPerson;


                        if (wnPerson.ShowDialog() == true)
                        {
                            // Сохранение данных в оперативной памяти
                            // Перенос данных из временного класса
                            // в класс отображения данных

                            Role r = (Role)wnPerson.CbRole.SelectedItem;

                            personDpo.RoleName = r.NameRole;
                            personDpo.FirstName = tempPerson.FirstName;
                            personDpo.LastName = tempPerson.LastName;
                            personDpo.Birthday = tempPerson.Birthday;

                            // Перенос данных из класса отображения
                            // данных в класс Person

                            FindPerson finder = new FindPerson(personDpo.Id);

                            List<Person> listPerson = ListPerson.ToList();

                            Person p = listPerson.Find(
                                new Predicate<Person>(finder.PersonPredicate));

                            p = p.CopyFromPersonDPO(personDpo);
                        }


                    }, (obj) => SelectedPersonDpo != null &&
                                ListPersonDpo.Count > 0));
            }
        }

        private RelayCommand deletePerson;

        public RelayCommand DeletePerson
        {
            get
            {
                return deletePerson ??
                    (deletePerson = new RelayCommand(obj =>
                    {
                        PersonDpo person = SelectedPersonDpo;

                        MessageBoxResult result = MessageBox.Show(
                            "Удалить данные по сотруднику:\n" +
                            person.LastName + " " + person.FirstName,
                            "Предупреждение",
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.OK)
                        {
                            Person personToDelete = null;

                            foreach (Person p in ListPerson)
                            {
                                if (p.Id == person.Id)
                                {
                                    personToDelete = p;
                                    break;
                                }
                            }

                            if (personToDelete != null)
                            {
                                ListPerson.Remove(personToDelete);
                            }

                            ListPersonDpo.Remove(person);

                            SelectedPersonDpo = null;
                        }

                    }, (obj) => SelectedPersonDpo != null &&
                                ListPersonDpo.Count > 0));
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
