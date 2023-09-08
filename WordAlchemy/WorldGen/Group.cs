
namespace WordAlchemy.WorldGen
{
    public class Group
    {
        public int Id { get; set; }
        public TerrainType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<MapNode> MapNodeList { get; set; }

        public Dictionary<Tuple<int, int>, byte> CellDict { get; set; }

        public Group(int id, TerrainType type, string name)
        {
            Id = id;
            Type = type;
            Name = name;
            Description = "";

            MapNodeList = new List<MapNode>();
            CellDict = new Dictionary<Tuple<int, int>, byte>();
        }

        public bool IsCellInGroup(int i, int j)
        {
            return CellDict.ContainsKey(Tuple.Create(i, j));
        }
    }
}
