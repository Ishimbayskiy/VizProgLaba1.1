using System;

namespace VizProgLaba1._1.Model
{
    public class Person
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public virtual Role Role { get; set; }

        public Person()
        {
        }

        public Person(
            int id,
            int roleId,
            string firstName,
            string lastName,
            DateTime birthday)
        {
            Id = id;
            RoleId = roleId;
            FirstName = firstName;
            LastName = lastName;
            Birthday = birthday;
        }
    }
}
