
using WordAlchemy.Grids;

namespace WordAlchemy
{
    public class Player
    {
        public Point WorldPos {  get; set; }

        public string Symbol { get; set; }

        public Player()
        {
            WorldPos = new Point(0, 0);

            //Symbol = "\u263A";
            Symbol = "\u263B";
        }

    }
}
