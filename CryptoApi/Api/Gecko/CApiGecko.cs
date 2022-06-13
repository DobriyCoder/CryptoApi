using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Interfaces;
using CryptoApi.Services;

namespace CryptoApi.Api.Gecko
{
    public class CApiGecko : CBaseApi, IApi
    {
        private readonly ICoinGeckoClient client;
        private IReadOnlyList<CoinFullData> CurrentCoins;

        public CApiGecko(IConfigurationSection conf, IConfigurationSection acc) : base(conf, acc)
        {
            client = CoinGeckoClient.Instance;
        }
        IEnumerable<CoinMarkets> GetAllMarkets ()
        {
            int page = 1;
            List<CoinMarkets> markets;
            int count = 0;
            int page_count = 0;
            int delay = 5000;
            CLogger.instance.Write("Start loading...");

            while(true)
            {
                CLogger.instance.Write($"{++page_count}. {count}. before ");

                try
                {
                    markets = client.CoinsClient.GetCoinMarkets("usd", new string[] { }, "market_cap_desc", 250, page++, false, "", "").Result;
                    //Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    CLogger.instance.Write($"err: ex.Message");
                    Thread.Sleep(delay);
                    delay += 1000;
                    continue;
                }

                CLogger.instance.Write($"after: {markets.Count}; delay: {delay}");
                if (markets.Count == 0) break;

                foreach (CoinMarkets market in markets)
                {
                    count++;
                    yield return market;
                }
            }

            Console.WriteLine($"All count: {count}");
        }
        public async Task<IApiCoinsData> GetCoinsAsync()
        {
            Inc();
            //CurrentCoins = await client.CoinsClient.GetAllCoinsData();
            //Console.WriteLine($"current coins: {CurrentCoins.Count()}");
            //var markets = await client.CoinsClient.GetCoinMarkets("usd");
            //Console.WriteLine($"markets: {markets.Count()}");
            //
            //return null;
            
            //var coins = await client.CoinsClient.GetCoinList();

            return new CGeckoCoinsData(GetAllMarkets(), null);
        }
        public async Task<IApiCoinPairsData> GetCoinPairsAsync()
        {
            return new CGeckoCoinPairsData(CurrentCoins);
        }

        public async Task TestAsync()
        {
            var markets = await client.CoinsClient.GetCoinMarkets("zoc");

            foreach(var market in markets)
            {
                Console.WriteLine($"{market.Name} - {market.PriceChange24H}");
            }
        }
    }
}
