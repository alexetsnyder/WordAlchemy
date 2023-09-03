using SDL2;
using WordAlchemy.WorldGen;

namespace WordAlchemy
{
    internal class UI
    {
        public MapViewer MapViewer { get; set; }

        public string GroupTypeStr { get; private set; }
        public int GroupTypeStrWidth { get; private set; }
        public int GroupTypeStrHeight { get; private set; }

        private SDLGraphics Graphics { get; set; }

        public UI(MapViewer mapViewer)
        {
            MapViewer = mapViewer;

            GroupTypeStr = string.Empty;
            GroupTypeStrWidth = 0;
            GroupTypeStrHeight = 0;

            Graphics = SDLGraphics.Instance;  
        }

        public void SetGroupTypeStr(string groupTypeStr)
        {
            GroupTypeStr = groupTypeStr;

            Graphics.SizeText(GroupTypeStr, FontName.IBM_VGA_8X14, out int width, out int height);
            GroupTypeStrWidth = width;
            GroupTypeStrHeight = height;
        }

        public void Update()
        {
            SDL.SDL_GetMouseState(out int screenX, out int screenY);

            MapViewer.ScreenToWorld(screenX, screenY, out int worldX, out int worldY);

            MapNode? mapNode = MapViewer.Map.GetMapNode(worldX, worldY);
            if (mapNode != null && mapNode.GroupID != null)
            {
                Group? group = MapViewer.Map.GetGroup((int)mapNode.GroupID);
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

            Graphics.DrawText(GroupTypeStr, windowWidth - GroupTypeStrWidth - 10, windowHeight - 2 * GroupTypeStrHeight, Colors.White(), FontName.IBM_VGA_8X14);
        }
    }
}
