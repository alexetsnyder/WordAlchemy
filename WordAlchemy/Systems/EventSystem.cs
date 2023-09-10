using SDL2;

namespace WordAlchemy.Systems
{
    public sealed class EventSystem
    {
        public static EventSystem Instance { get { return Nested.instance; } }

        public static readonly int GLOBAL = -1;

        private Dictionary<SDL.SDL_EventType, List<Action<SDL.SDL_Event>>> GlobalEventDict { get; set; }

        private Dictionary<uint, Dictionary<SDL.SDL_EventType, List<Action<SDL.SDL_Event>>>> EventDict { get; set; }

        private EventSystem()
        {
            GlobalEventDict = new Dictionary<SDL.SDL_EventType, List<Action<SDL.SDL_Event>>>();
            EventDict = new Dictionary<uint, Dictionary<SDL.SDL_EventType, List<Action<SDL.SDL_Event>>>>();
        }

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly EventSystem instance = new EventSystem();
        }

        public void Invoke(int state, SDL.SDL_Event e)
        {
            if (GlobalEventDict.ContainsKey(e.type))
            {
                List<Action<SDL.SDL_Event>> actionList = GlobalEventDict[e.type];
                foreach (Action<SDL.SDL_Event> action in actionList)
                {
                    action(e);
                }
            }

            if (state >= 0)
            {
                uint uintState = (uint)state;
                if (EventDict.ContainsKey(uintState) && EventDict[uintState].ContainsKey(e.type))
                {
                    List<Action<SDL.SDL_Event>> actionList = EventDict[uintState][e.type];
                    foreach (Action<SDL.SDL_Event> action in actionList)
                    {
                        action(e);
                    }
                }
            }
        }

        public void Listen(int state, SDL.SDL_EventType eventType, Action<SDL.SDL_Event> action)
        {
            if (state < 0)
            {
                if (!GlobalEventDict.ContainsKey(eventType))
                {
                    GlobalEventDict.Add(eventType, new List<Action<SDL.SDL_Event>>());
                }

                GlobalEventDict[eventType].Add(action);
            }
            else
            {
                uint uintState = (uint)state;

                if (!EventDict.ContainsKey(uintState))
                {
                    EventDict.Add(uintState, new Dictionary<SDL.SDL_EventType, List<Action<SDL.SDL_Event>>>());
                }

                if (!EventDict[uintState].ContainsKey(eventType))
                {
                    EventDict[uintState].Add(eventType, new List<Action<SDL.SDL_Event>>());
                }

                EventDict[uintState][eventType].Add(action);
            }
        }
    }
}
