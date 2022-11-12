namespace NijiDive
{
    public interface IPauseable
    {
        public bool IsPaused { get; }

        public void SetPaused(bool paused);
    }
}