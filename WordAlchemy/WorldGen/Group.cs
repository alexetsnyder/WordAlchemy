
using WordAlchemy.Grids;

namespace WordAlchemy.WorldGen
{
    public class Group
    {
        public int Id { get; set; }
        public TerrainType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private Dictionary<Tuple<int, int>, byte> CellDict { get; set; }

        public Group(int id, TerrainType type, string name)
        {
            Id = id;
            Type = type;
            Name = name;
            Description = "";

            CellDict = new Dictionary<Tuple<int, int>, byte>();
        }

        public bool IsCellInGroup(Cell cell)
        {
            return CellDict.ContainsKey(cell.GridPos.PointTuple);
        }

        public void AddCell(Cell cell, byte value)
        {
            CellDict.Add(cell.GridPos.PointTuple, value);
        }

        public void RemoveCell(Cell cell)
        {
            CellDict.Remove(cell.GridPos.PointTuple);
        }

        public List<Cell> GetGroupCells(BoundedGrid grid)
        {
            List<Cell> cellList = new List<Cell>();

            foreach (var cellTuple in CellDict.Keys)
            {
                Point gridPos = new Point(cellTuple);
                Cell? cell = grid.GetCell(gridPos);
                if (cell.HasValue)
                {
                    cellList.Add(cell.Value);
                }
            }

            return cellList;
        }
    }
}
