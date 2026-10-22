using System;
using System.Collections.ObjectModel;
using VizProgLaba1._1.Helper;
using VizProgLaba1._1.Model;

namespace VizProgLaba1._1.ViewModel
{
    public class WindowNewEmployeeViewModel
    {
        private readonly Action<bool> close;
        private RelayCommand save;
        private RelayCommand cancel;

        public PersonDpo Person { get; }
        public ObservableCollection<Role> ListRole { get; }

        public WindowNewEmployeeViewModel(PersonDpo person, Action<bool> close)
        {
            Person = person;
            ListRole = new RoleViewModel().ListRole;
            this.close = close;
        }

        public RelayCommand Save
        {
            get
            {
                return save ?? (save = new RelayCommand(obj => close(true)));
            }
        }

        public RelayCommand Cancel
        {
            get
            {
                return cancel ?? (cancel = new RelayCommand(obj => close(false)));
            }
        }
    }
}
