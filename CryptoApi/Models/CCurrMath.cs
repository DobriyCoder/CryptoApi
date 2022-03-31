namespace CryptoApi.Models
{
    static public class CCurrMath
    {
        static public decimal? Exchange (decimal? val1, decimal? val2)
        {
            if (val1 == null || val2 == null) return null;

            return Math.Round((decimal)(1 / val1 * val2), 6);
        }
    }
}
