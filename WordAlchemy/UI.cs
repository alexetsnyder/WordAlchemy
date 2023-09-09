using SDL2;
using WordAlchemy.Viewers;
using WordAlchemy.WorldGen;

namespace WordAlchemy
{
    internal class UI
    {
        public MapViewer MapViewer { get; set; }

        public string GroupTypeStr { get; private set; }
        public int GroupTypeStrWidth { get; private set; }
        public int GroupTypeStrHeight { get; private set; }

        private GraphicSystem GraphicSystem { get; set; }

        public UI(MapViewer mapViewer)
        {
            MapViewer = mapViewer;

            GroupTypeStr = string.Empty;
            GroupTypeStrWidth = 0;
            GroupTypeStrHeight = 0;

            GraphicSystem = GraphicSystem.Instance;  
        }

        public void SetGroupTypeStr(string groupTypeStr)
        {
            GroupTypeStr = groupTypeStr;

            GraphicSystem.SizeText(GroupTypeStr, FontName.IBM_VGA_8X14, out int width, out int height);
            GroupTypeStrWidth = width;
            GroupTypeStrHeight = height;
        }

        public void Update()
        {
            SDL.SDL_GetMouseState(out int screenX, out int screenY);

            MapViewer.ScreenToWorld(screenX, screenY, out int worldX, out int worldY);

            Cell cell = MapViewer.Map.Grid.GetCellFromWorld(worldX, worldY);
            if (MapViewer.Map.IsCellGrouped(cell.I, cell.J))
            {
                Group? group = MapViewer.Map.GetGroup(cell.I, cell.J);
                if (group != null)
                {
                    SetGroupTypeStr($"{group.Name} {group.Id}");
                }
            }
        }

        public void Draw()
        {
            int windowWidth = AppSettings.Instance.WindowWidth;
            int windowHeight = AppSettings.Instance.WindowHeight;

            GraphicSystem.DrawText(GroupTypeStr, windowWidth - GroupTypeStrWidth - 10, windowHeight - 2 * GroupTypeStrHeight, Colors.White(), FontName.IBM_VGA_8X14);
        }
    }
}
