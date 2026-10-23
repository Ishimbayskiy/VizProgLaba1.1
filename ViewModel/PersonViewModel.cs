using System.IO;
using Newtonsoft.Json;
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
        readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "DataModels", "PersonData.json");
        string _jsonPersons = String.Empty;
        public string Error { get; set; }
        public string Message { get; set; }

        private PersonDpo _selectedPersonDpo;


        public PersonDpo SelectedPersonDpo
        {
            get
            {
                return _selectedPersonDpo;
            }
            set
            {
                _selectedPersonDpo = value;

                OnPropertyChanged("SelectedPersonDpo");


            }
        }


        public ObservableCollection<Person> ListPerson { get; set; }
            = new ObservableCollection<Person>();

        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; }
            = new ObservableCollection<PersonDpo>();

        public PersonViewModel()
        {
            ListPerson = LoadPerson();
            ListPersonDpo = GetListPersonDpo();
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

        private RelayCommand _addPerson;

        public RelayCommand AddPerson
        {
            get
            {
                return _addPerson ??
                    (_addPerson = new RelayCommand(obj =>
                    {
                        WindowNewEmployee wnPerson = new WindowNewEmployee
                        {
                            Title = "Новый сотрудник"
                        };

                        int maxIdPerson = MaxId() + 1;

                        PersonDpo per = new PersonDpo
                        {
                            Id = maxIdPerson,
                            Birthday = DateTime.Now.ToString()
                        };

                        wnPerson.DataContext = new WindowNewEmployeeViewModel(
                            per, result => wnPerson.DialogResult = result);


                        if (wnPerson.ShowDialog() == true)
                        {
                            Role r = (Role)wnPerson.CbRole.SelectedItem;
                            if (r == null)
                            {
                                Message = "Необходимо выбрать должность сотрудника.";
                                return;
                            }

                            per.RoleName = r.NameRole;
                            per.Birthday = PersonDpo.GetStringBirthday(per.Birthday);

                            // Добавление в коллекцию отображения
                            ListPersonDpo.Add(per);

                            // Добавление в коллекцию Person
                            Person p = new Person();
                            p = p.CopyFromPersonDpo(per);
                            ListPerson.Add(p);
                            SaveChanges(ListPerson);
                        }

                    }, (obj) => true));
            }
        }

        private RelayCommand _editPerson;

        public RelayCommand EditPerson
        {
            get
            {
                return _editPerson ??
                    (_editPerson = new RelayCommand(obj =>
                    {
                        WindowNewEmployee wnPerson = new WindowNewEmployee
                        {
                            Title = "Редактирование данных сотрудника"
                        };

                        PersonDpo personDpo = SelectedPersonDpo;

                        PersonDpo tempPerson = personDpo.ShallowCopy();

                        wnPerson.DataContext = new WindowNewEmployeeViewModel(
                            tempPerson, result => wnPerson.DialogResult = result);


                        if (wnPerson.ShowDialog() == true)
                        {
                            // Сохранение данных в оперативной памяти
                            // Перенос данных из временного класса
                            // в класс отображения данных

                            Role r = (Role)wnPerson.CbRole.SelectedItem;
                            if (r == null)
                            {
                                Message = "Необходимо выбрать должность сотрудника.";
                                return;
                            }

                            personDpo.RoleName = r.NameRole;
                            personDpo.FirstName = tempPerson.FirstName;
                            personDpo.LastName = tempPerson.LastName;
                            personDpo.Birthday = PersonDpo.GetStringBirthday(tempPerson.Birthday);

                            // Перенос данных из класса отображения
                            // данных в класс Person

                            var per = ListPerson.FirstOrDefault(p => p.Id == personDpo.Id);
                            if (per != null)
                            {
                                per = per.CopyFromPersonDpo(personDpo);
                            }
                            SaveChanges(ListPerson);
                        }


                    }, (obj) => SelectedPersonDpo != null &&
                                ListPersonDpo.Count > 0));
            }
        }

        private RelayCommand _deletePerson;

        public RelayCommand DeletePerson
        {
            get
            {
                return _deletePerson ??
                    (_deletePerson = new RelayCommand(obj =>
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
                            var per = ListPerson.FirstOrDefault(p => p.Id == person.Id);
                            if (per != null)
                            {
                                ListPerson.Remove(per);
                                SaveChanges(ListPerson);
                            }

                            ListPersonDpo.Remove(person);

                            SelectedPersonDpo = null;
                        }

                    }, (obj) => SelectedPersonDpo != null &&
                                ListPersonDpo.Count > 0));
            }
        }

        #region Methods
        /// <summary>
        /// Загрузка JSON-файла с данными по сотрудникам.
        /// </summary>
        public ObservableCollection<Person> LoadPerson()
        {
            _jsonPersons = File.ReadAllText(path);
            ListPerson = JsonConvert.DeserializeObject<ObservableCollection<Person>>(_jsonPersons)
                ?? new ObservableCollection<Person>();
            return ListPerson;
        }

        /// <summary>
        /// Сохранение изменённой коллекции в JSON-файл.
        /// </summary>
        private void SaveChanges(ObservableCollection<Person> listPersons)
        {
            var jsonPerson = JsonConvert.SerializeObject(listPersons);
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonPerson);
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
