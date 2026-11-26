using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VizProgLaba1._1.Model
{
    public class Role : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string nameRole;

        public string NameRole
        {
            get { return nameRole; }
            set
            {
                nameRole = value;
                OnPropertyChanged("NameRole");
            }
        }

        public Role()
        {
            Persons = new HashSet<Person>();
        }

        public Role(int id, string nameRole)
            : this()
        {
            Id = id;
            NameRole = nameRole;
        }

        public virtual ICollection<Person> Persons { get; set; }

        public Role ShallowCopy()
        {
            return (Role)MemberwiseClone();
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
