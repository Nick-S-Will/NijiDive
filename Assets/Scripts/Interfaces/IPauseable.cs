namespace NijiDive
{
    public interface IPauseable
    {
        public bool IsPaused { get; }

        public void Pause(bool paused = true);
    }
}