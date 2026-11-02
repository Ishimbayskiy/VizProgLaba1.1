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
using System.Linq;
namespace VizProgLaba1._1.ViewModel

{
    public class PersonViewModel : INotifyPropertyChanged
    {
        readonly string path = @"C:\Users\glgle\OneDrive\Desktop\2Semak PROGA\тест мой проект\raspisaniesettings\VizProgLaba2\DataModels\PersonData.json";
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

        public ObservableCollection<PersonDpo> ListPersonDpo { get; set; }

        public PersonViewModel()
        {
            ListPerson = new ObservableCollection<Person>();
            ListPersonDpo = new ObservableCollection<PersonDpo>();
            ListPerson = LoadPerson();
            ListPersonDpo = GetListPersonDpo();
        }

        public ObservableCollection<PersonDpo> GetListPersonDpo()
        {
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
                            if (r != null)
                            {
                                per.RoleName = r.NameRole;
                                per.Birthday = PersonDpo.GetStringBirthday(per.Birthday);
                                ListPersonDpo.Add(per);

                                // Добавление нового сотрудника в коллекцию ListPerson
                                Person p = new Person();
                                p = p.CopyFromPersonDpo(per);
                                ListPerson.Add(p);
                                try
                                {
                                    SaveChanges(ListPerson);
                                }
                                catch (Exception e)
                                {
                                    Error = "Ошибка добавления данных в json файл\n" + e.Message;
                                }
                            }
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
                            if (r != null)
                            {
                                personDpo.RoleName = r.NameRole;
                                personDpo.FirstName = tempPerson.FirstName;
                                personDpo.LastName = tempPerson.LastName;
                                personDpo.Birthday = PersonDpo.GetStringBirthday(tempPerson.Birthday);

                                // Перенос данных из класса отображения данных в класс Person
                                var per = ListPerson.FirstOrDefault(p => p.Id == personDpo.Id);
                                if (per != null)
                                {
                                    per = per.CopyFromPersonDpo(personDpo);
                                }
                                try
                                {
                                    SaveChanges(ListPerson);
                                }
                                catch (Exception e)
                                {
                                    Error = "Ошибка редактирования данных в json файл\n" + e.Message;
                                }
                            }
                            else
                            {
                                Message = "Необходимо выбрать должность сотрудника.";
                            }
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
                            try
                            {
                                ListPersonDpo.Remove(person);
                                var per = ListPerson.FirstOrDefault(p => p.Id == person.Id);
                                if (per != null)
                                {
                                    ListPerson.Remove(per);
                                    SaveChanges(ListPerson);
                                }
                            }
                            catch (Exception e)
                            {
                                Error = "Ошибка удаления данных\n" + e.Message;
                            }
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
            if (_jsonPersons != null)
            {
                ListPerson = JsonConvert.DeserializeObject<ObservableCollection<Person>>(_jsonPersons);
                return ListPerson;
            }
            else
            {
                return null;
            }
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
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла\n" + e.Message;
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
