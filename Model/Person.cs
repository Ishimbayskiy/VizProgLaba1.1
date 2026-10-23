using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VizProgLaba1._1.ViewModel;
namespace VizProgLaba1._1.Model
{
    public class Person
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public Person() { }
        public Person(int id, int roleId, string firstName, string lastName, string birthday)
        {
            this.Id = id; this.RoleId = roleId;
            this.FirstName = firstName; this.LastName = lastName; this.Birthday = birthday;
        }

        public Person CopyFromPersonDpo(PersonDpo personDpo)
        {
            this.Id = personDpo.Id;
            this.FirstName = personDpo.FirstName;
            this.LastName = personDpo.LastName;
            this.Birthday = personDpo.Birthday;

            RoleViewModel vmRole = new RoleViewModel();

            foreach (Role role in vmRole.ListRole)
            {
                if (role.NameRole == personDpo.RoleName)
                {
                    this.RoleId = role.Id;
                    break;
                }
            }

            return this;
        }

    }
}

