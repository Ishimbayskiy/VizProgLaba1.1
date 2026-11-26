using System.Data.Entity;
using VizProgLaba1._1.Data;
using VizProgLaba1._1.Model;

namespace VizProgLaba1._1
{
    /// <summary>
    /// Контекст базы данных приложения.
    /// </summary>
    public class CompanyEntities : DbContext
    {
        static CompanyEntities()
        {
            Database.SetInitializer(new CompanyDatabaseInitializer());
        }

        public CompanyEntities()
            : base("name=CompanyEntities")
        {
        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
    }
}
