
namespace WordAlchemy.GUI
{
    public class UserInterface
    {
        public List<IUIElement> UIElementList { get; private set; }

        public UserInterface()
        {
            UIElementList = new List<IUIElement>();
        }

        public void AddUIElement(IUIElement uiElement)
        {
            UIElementList.Add(uiElement);
        }

        public void Update()
        {
            foreach (IUIElement element in UIElementList)
            {
                element.Update();
            }
        }

        public void Draw()
        {
            foreach(IUIElement element in UIElementList)
            {
                element.Draw();
            }
        }
    }
}