
using WordAlchemy.Grids;
using WordAlchemy.Settings;
using WordAlchemy.Systems;

namespace WordAlchemy.GUI
{
    public class TextElement : IUIElement
    {
        public string UIText { get; private set; }
        private Func<string> GetUIText { get; set; }

        private Point Size { get; set; }

        public TextElement(Func<string> getUIText)
        {
            UIText = "";
            GetUIText = getUIText;
            Size = new Point(0, 0);
        }

        public void SetGroupTypeStr(string groupTypeStr)
        {
            UIText = groupTypeStr;

            GraphicSystem.Instance.SizeText(UIText, AppSettings.Instance.MapFontName, out int width, out int height);
            Size = new Point(width, height);
        }

        public void Update()
        {
            SetGroupTypeStr(GetUIText());
        }

        public void Draw()
        {
            int windowWidth = AppSettings.Instance.WindowWidth;
            int windowHeight = AppSettings.Instance.WindowHeight;

            GraphicSystem.Instance.DrawText(UIText, windowWidth - Size.W - 10, windowHeight - 2 * Size.H, Colors.White(), AppSettings.Instance.MapFontName);
        }  
    }
}
