
namespace WordAlchemy
{
    public class Timer
    {
        public bool IsStarted {  get; set; }

        public bool IsPaused { get; set; }

        public int ElapsedTime { get; set; }

        private Time Time { get; set; }

        public Timer()
        {
            IsStarted = false;
            IsPaused = false;
            ElapsedTime = 0;

            Time = Time.Instance;
        }

        public void Start()
        {
            IsStarted = true;
            IsPaused = false;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Update()
        {
            if (IsStarted &&  !IsPaused)
            {
                ElapsedTime += Time.DeltaTime;
            }
        }

        public string GetTimeStr()
        {
            return $"{ (ElapsedTime / 1000).ToString().PadLeft(2, '0') }:{ (ElapsedTime % 1000 * 60 / 1000).ToString().PadLeft(2, '0') }";
        }
    }
}
