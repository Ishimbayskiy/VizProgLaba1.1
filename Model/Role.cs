namespace VizProgLaba1._1.Model
{
    public class Role
    {
        public int Id { get; set; }
        public string NameRole { get; set; }

        public Role() { }

        public Role(int id, string nameRole)
        {
            this.Id = id;
            this.NameRole = nameRole;
        }
    }
}
