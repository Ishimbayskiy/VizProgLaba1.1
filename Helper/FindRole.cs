using VizProgLaba1._1.Model;

namespace VizProgLaba1._1.Helper
{
    public class FindRole
    {
        int id;

        public FindRole(int id)
        {
            this.id = id;
        }

        public bool RolePredicate(Role role)
        {
            return role.Id == id;
        }
    }
}
