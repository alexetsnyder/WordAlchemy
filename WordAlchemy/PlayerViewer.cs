
namespace WordAlchemy
{
    public class PlayerViewer : Viewer
    {
        public Player Player { get; set; }

        public PlayerViewer(ViewWindow? srcViewWindow, ViewWindow? dstViewWindow)
            : base(srcViewWindow,  dstViewWindow)
        {
            Player = new Player();
        }

        public void Update()
        {

        }

        public void Draw()
        {

        }
    }
}
