using SDL2;

namespace WordAlchemy
{
    public sealed class EventSystem
    {
        public static EventSystem Instance { get { return Nested.instance; } }

        private Dictionary<SDL.SDL_EventType, List<Action<SDL.SDL_Event>>> EventDict { get; set; }

        private EventSystem()
        {
            EventDict = new Dictionary<SDL.SDL_EventType, List<Action<SDL.SDL_Event>>>();
        }

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly EventSystem instance = new EventSystem();
        }

        public void Invoke(SDL.SDL_Event e)
        {
            if (EventDict.ContainsKey(e.type))
            {
                List<Action<SDL.SDL_Event>> actions = EventDict[e.type];
                foreach (Action<SDL.SDL_Event> action in actions)
                {
                    action(e);
                }
            }
        }

        public void Listen(SDL.SDL_EventType eventType, Action<SDL.SDL_Event> action)
        {
            if (!EventDict.ContainsKey(eventType))
            {
                EventDict.Add(eventType, new List<Action<SDL.SDL_Event>>());
            }

            EventDict[eventType].Add(action);
        }
    }
}
