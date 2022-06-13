using CoinGecko.Entities.Response.Coins;
using CryptoApi.Services;
using System.Collections;

namespace CryptoApi.Api.Gecko
{
    public class CGeckoCoinsData : IApiCoinsData
    {
        /*IReadOnlyList<CoinFullData> coins_full;*/
        IEnumerable<CoinMarkets> coins_full;
        IReadOnlyList<CoinList> coins;

        string key = "gecko";
        public CGeckoCoinsData (IEnumerable<CoinMarkets> coins_full, IReadOnlyList<CoinList> coins)
        {
            this.coins = coins;
            this.coins_full = coins_full;
        }
        public IEnumerable<IApiCoin> GetEnumerable()
        {
            foreach(var coin in coins_full)
            {
                //Console.WriteLine("image: " + coin.Image.Large.AbsoluteUri);
                //Console.WriteLine("image: " + coin.Image.Small.AbsoluteUri);
                //Console.WriteLine("image: " + coin.Image.Thumb.AbsoluteUri);
                CApiCoin new_coin = default;
                try
                {
                    new_coin = new CApiCoin
                    {
                        Image = coin.Image.ToString(),
                        Donor = key,
                        Id = coin.Id,
                        FullName = coin.Name,
                        Name = coin.Symbol,
                        UsdPrice = coin.CurrentPrice ?? 0,
                        MarketCap = coin.MarketCap ?? 0,
                        Low = coin.Low24H ?? 0,
                        High = coin.High24H ?? 0,

                        CirculatingSupply = coin.CirculatingSupply,
                        TotalSupply = coin.TotalSupply ?? 0,
                        MarketCapRank = coin.MarketCapRank ?? 0,
                        TotalVolume = coin.TotalVolume ?? 0,
                    };
                }
                catch(Exception ex)
                {
                    CLogger.instance.Write($"GetEnumerable err: {ex.Message}");
                }
                

                yield return new_coin;
                /*yield return new CApiCoin
                {
                    Image = coin.Image.Large.AbsoluteUri,
                    Donor = key, 
                    Id = coin.Id, 
                    FullName = coin.Name, 
                    Name = coin.Symbol,
                    UsdPrice = coin.MarketData.CurrentPrice["usd"],
                    MarketCap = coin.MarketData.MarketCap["usd"],
                    Low = coin.MarketData.Low24H["usd"],
                    High = coin.MarketData.High24H["usd"]
                };*/
            }

            /*foreach (var coin in coins)
            {
                yield return new CApiCoin
                {
                    Donor = key,
                    Id = coin.Id,
                    FullName = coin.Name,
                    Name = coin.Symbol
                };
            }*/
        }
    }
}
