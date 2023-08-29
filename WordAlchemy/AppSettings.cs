
namespace WordAlchemy
{
    public class AppSettings
    {
        public static AppSettings Instance { get { return Nested.instance; } }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        private AppSettings()
        {
            WindowWidth = 0;
            WindowHeight = 0;
        }

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly AppSettings instance = new AppSettings();
        }

        public void Init(int windowWidth, int windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }
    }
}
