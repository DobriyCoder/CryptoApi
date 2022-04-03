using CryptoApi.Models.DB;

namespace CryptoApi.Models
{
    static public class CCurrMath
    {
        static public decimal? Exchange (decimal? val1, decimal? val2)
        {
            if (val1 == null || val2 == null) return null;

            return Math.Round((decimal)(1 / val1 * val2), 6);
        }

        static public IUpdatedData FilterLust (int days, IEnumerable<IUpdatedData> list)
        {
            return list.First();
        }

        static public decimal GetChangePrice (int days, IEnumerable<CCoinsExtDataM> coins)
        {
            var first = (CCoinsExtDataM)FilterLust(days, coins);
            var last = coins.Last();

            return last.usd_price.Value - first.usd_price.Value;
        }

        static public decimal GetChangePercentPrice(int days, IEnumerable<CCoinsExtDataM> coins)
        {
            var last = coins.Last().usd_price.Value;
            var diff = GetChangePrice(days, coins);
            var result = Math.Round(100 / last * diff, 3);
            
            return result;
        }
    }
}
