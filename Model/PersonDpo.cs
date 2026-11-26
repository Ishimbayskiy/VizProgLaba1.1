using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace VizProgLaba1._1.Model
{
    public class PersonDpo : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _roleName;
        public string RoleName
        {
            get { return _roleName; }
            set
            {
                _roleName = value;
                OnPropertyChanged("RoleName");
            }
        }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private string birthday;
        public string Birthday
        {
            get { return birthday; }
            set
            {
                birthday = value;
                OnPropertyChanged("Birthday");
            }
        }

        public PersonDpo()
        {
        }

        public PersonDpo(
            int id,
            string roleName,
            string firstName,
            string lastName,
            string birthday)
        {
            Id = id;
            RoleName = roleName;
            FirstName = firstName;
            LastName = lastName;
            Birthday = birthday;
        }

        public PersonDpo ShallowCopy()
        {
            return (PersonDpo)MemberwiseClone();
        }

        public PersonDpo CopyFromPerson(Person person)
        {
            return new PersonDpo
            {
                Id = person.Id,
                RoleName = person.Role == null ? string.Empty : person.Role.NameRole,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Birthday = person.Birthday.ToString(
                    "dd.MM.yyyy",
                    CultureInfo.GetCultureInfo("ru-RU"))
            };
        }

        public static string GetStringBirthday(string value)
        {
            return DateTime.Parse(value, CultureInfo.GetCultureInfo("ru-RU"))
                .ToString("dd.MM.yyyy");
        }

        public static DateTime GetDateTimeBirthday(string value)
        {
            return DateTime.ParseExact(
                value,
                "dd.MM.yyyy",
                CultureInfo.GetCultureInfo("ru-RU"));
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
