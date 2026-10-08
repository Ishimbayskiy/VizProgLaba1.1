using System.Collections.Generic;
using VizProgLaba1._1.Model;

namespace VizProgLaba1._1.ViewModel
{
    public class RoleViewModel
    {
        public List<Role> ListRole { get; set; }

        public RoleViewModel()
        {
            ListRole = new List<Role>
            {
                new Role
                {
                    Id = 1,
                    NameRole = "Менеджер"
                },

                new Role
                {
                    Id = 2,
                    NameRole = "Программист"
                }
            };
        }
    }
}



