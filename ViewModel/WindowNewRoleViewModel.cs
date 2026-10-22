using System;
using VizProgLaba1._1.Helper;
using VizProgLaba1._1.Model;

namespace VizProgLaba1._1.ViewModel
{
    public class WindowNewRoleViewModel
    {
        private readonly Action<bool> close;
        private RelayCommand save;
        private RelayCommand cancel;

        public Role Role { get; }

        public WindowNewRoleViewModel(Role role, Action<bool> close)
        {
            Role = role;
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
