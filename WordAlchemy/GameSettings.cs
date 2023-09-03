
namespace WordAlchemy
{
    public class GameSettings
    {
        public static GameSettings Instance { get { return Nested.instance; } }

        public GameState State { get; set; }

        private GameSettings()
        {
            State = GameState.NONE;
        }

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly GameSettings instance = new GameSettings();
        }
    }


    public enum GameState
    {
        NONE,
        MAP,
        PLAYER,
    }
}
