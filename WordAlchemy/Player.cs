
namespace WordAlchemy
{
    public class Player
    {
        public int X {  get; set; }
        public int Y { get; set; }

        public string Symbol { get; set; }

        public Player()
        {
            X = 0;
            Y = 0;

            //Symbol = "\u263A";
            Symbol = "\u263B";
        }

    }
}
