
namespace WordAlchemy
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Group(int id)
        {
            Id = id;
            Name = "";
            Description = "";
        }
    }
}
