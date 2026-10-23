
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using VizProgLaba1._1.ViewModel;

namespace VizProgLaba1._1.Model
{
    public class PersonDpo : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _roleName;

        public string RoleName
        {
            get
            {
                return _roleName;
            }
            set
            {
                _roleName = value;
                OnPropertyChanged("RoleName");
            }
        }

        private string firstName;

        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        private string lastName;

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private string birthday;

        public string Birthday
        {
            get
            {
                return birthday;
            }
            set
            {
                birthday = value;
                OnPropertyChanged("Birthday");
            }
        }

        public PersonDpo() { }

        public PersonDpo(int id, string roleName,
            string firstName, string lastName, string birthday)
        {
            this.Id = id;
            this.RoleName = roleName;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Birthday = birthday;
        }

        public PersonDpo ShallowCopy()
        {
            return (PersonDpo)this.MemberwiseClone();
        }

        public PersonDpo CopyFromPerson(Person person)
        {
            PersonDpo perDpo = new PersonDpo();

            RoleViewModel vmRole = new RoleViewModel();

            string role = "Не задано";

            foreach (var r in vmRole.ListRole)
            {
                if (r.Id == person.RoleId)
                {
                    role = r.NameRole;
                    break;
                }
            }

            perDpo.Id = person.Id;
            perDpo.RoleName = role;
            perDpo.FirstName = person.FirstName;
            perDpo.LastName = person.LastName;
            perDpo.Birthday = person.Birthday;

            return perDpo;
        }

        /// <summary>
        /// Приведение даты рождения к строке dd.MM.yyyy для JSON.
        /// </summary>
        public static string GetStringBirthday(string value)
        {
            DateTime date;
            if (DateTime.TryParse(value, CultureInfo.GetCultureInfo("ru-RU"),
                    DateTimeStyles.None, out date) ||
                DateTime.TryParse(value, CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out date))
            {
                return date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            }

            throw new FormatException("Некорректная дата рождения.");
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
