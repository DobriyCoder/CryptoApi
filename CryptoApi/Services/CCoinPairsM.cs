using CryptoApi.Api;
using CryptoApi.Models.DB;
using CryptoApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CryptoApi.Services;

/// <summary>
///     Модель (сервис), для работы с CoinPairs из БД.
/// </summary>
public class CCoinPairsM : CBaseDbM
{
    CCoinsM coinsModel;

    /// <summary>
    ///     Конструктор. заполняет  необходимые поля при создании модели. Передает модель БД родителю.
    /// </summary>
    public CCoinPairsM(CDbM db, CCoinsM coins) : base(db)
    {
        coinsModel = coins;
        db.CoinPairs = GetPairsData();
    }

    private IEnumerable<CCoinPairDataM> GetPairsData ()
    {
        var coins = coinsModel.GetTrueCoins().ToList();
        //var count = coins.Count();

        foreach (var coin1 in coins)
        {
            foreach (var coin2 in coins)
            {
                yield return new CCoinPairDataM(coin1, coin2, GetMeta(coin1.id, coin2.id));
            }
        }

        /*for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                yield return new CCoinPairDataM(coins[i], coins[j], GetMeta(coin1.id, coin2.id));
            }
        }*/
    }

    public IEnumerable<CCoinPairsMetaDataM> GetMeta (uint id1, uint id2)
    {
        return db.CoinPairsMeta
            .Where(m => m.coin_1_id == id1 && m.coin_2_id == id2);
    }

    /// <summary>
    ///     Возвращает количество пар.
    /// </summary>
    public uint Count()
    {
        int count = coinsModel.TrueCount();
        return (uint)(count*count);
    }

    /// <summary>
    ///     Достает пары из БД используя исходное заданное количество и номер страницы.
    /// </summary>
    public IEnumerable<CCoinPairDataVM> GetPairs(int page, int count)
    {
        return (from p in db.CoinPairs
                
                join c1 in db.Coins on p.coin1_id equals c1.id
                join c2 in db.Coins on p.coin2_id equals c2.id
                
                select new CCoinPairDataVM()
                {
                    data = p,
                    coin1 = c1,
                    coin2 = c2
                })
                //.Include(p => p.meta)
                .Skip((page - 1) * count)
                .Take(count);
    }

    /// <summary>
    ///     Достает похожие пары из БД используя id исходной пары.
    /// </summary>
    public IEnumerable<CCoinPairDataVM> GetPairs(CCoinPairDataM pair)
    {
        return null;
        /*return from item in pair["pairs"]
               where item.coin_1_id == pair.coin1_id || item.coin_2_id == pair.coin2_id || item.coin_1_id == pair.coin2_id || item.coin_2_id == pair.coin1_id
               let pair_m = db.CoinPairs.First(m => m.id);
               select new CCoinPairDataVM()
               {
                   data = pair_m,
                   coin1 = db.Coins.Find(item.coin_1_id),
                   coin2 = db.Coins.Find(item.coin_2_id)
               };*/
    }

    /// <summary>
    ///     Достает пару из БД используя name монет.
    /// </summary>
    public CCoinPairDataVM GetPairByNames(string name1, string name2)
    {
        return null;
        /*return db.CoinPairs
            .Include(p => p.meta)
            .Join
            (
                db.Coins,
                pair => pair.coin1_id,
                coin => coin.id,
                (pair, coin) => new { Pair = pair, Coin = coin }
            )
            .Join
            (
                db.Coins,
                data => data.Pair.coin2_id,
                coin => coin.id,
                (data, coin) => new { Pair = data.Pair, Coin1 = data.Coin, Coin2 = coin }
            )
            .Where(data => data.Coin1.name == name1 && data.Coin2.name == name2)
            .Select(data => new CCoinPairDataVM()
            {
                data = data.Pair,
                coin1 = data.Coin1,
                coin2 = data.Coin2
            })
            .FirstOrDefault();*/
    }

    /// <summary>
    ///     Возвращает число максимально возможной страницы для пагинации.
    /// </summary>
    public int GetMaxPage(int count)
    {
        int max_count = (int)Math.Ceiling(Count() / count * 1f);
        return max_count;
    }
}
