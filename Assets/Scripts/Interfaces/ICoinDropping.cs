namespace NijiDive
{
    public interface ICoinDropping
    {
        public int CoinCount { get; }
    }

    public enum CoinValue { Small = 1, Medium = 5, Large = 10 }
}