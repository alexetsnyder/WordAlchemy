using SDL2;
using WordAlchemy.Settings;
using WordAlchemy.Systems;
using WordAlchemy.TextRendering;
using WordAlchemy.Viewers;
using WordAlchemy.WorldGen;

namespace WordAlchemy
{
    public class UI
    {
        public string GroupTypeStr { get; private set; }
        public int GroupTypeStrWidth { get; private set; }
        public int GroupTypeStrHeight { get; private set; }

        private GraphicSystem GraphicSystem { get; set; }

        public UI()
        {
            GroupTypeStr = string.Empty;
            GroupTypeStrWidth = 0;
            GroupTypeStrHeight = 0;

            GraphicSystem = GraphicSystem.Instance;  
        }

        public void SetGroupTypeStr(string groupTypeStr)
        {
            GroupTypeStr = groupTypeStr;

            GraphicSystem.SizeText(GroupTypeStr, AppSettings.Instance.MapFontName, out int width, out int height);
            GroupTypeStrWidth = width;
            GroupTypeStrHeight = height;
        }

        public void Draw()
        {
            int windowWidth = AppSettings.Instance.WindowWidth;
            int windowHeight = AppSettings.Instance.WindowHeight;

            GraphicSystem.DrawText(GroupTypeStr, windowWidth - GroupTypeStrWidth - 10, windowHeight - 2 * GroupTypeStrHeight, Colors.White(), FontName.IBM_VGA_8X14);
        }
    }
}
