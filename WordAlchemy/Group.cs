
namespace WordAlchemy
{
    public class Group
    {
        public int Id { get; set; }
        public TerrainType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<MapNode> MapNodeList { get; set; }

        public Group(int id, TerrainType type, string name)
        {
            Id = id;
            Type = type;
            Name = name;
            Description = "";

            MapNodeList = new List<MapNode>();
        }
    }
}
