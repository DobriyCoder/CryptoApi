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
    CCommonM commonModel;

    /// <summary>
    ///     Конструктор. заполняет  необходимые поля при создании модели. Передает модель БД родителю.
    /// </summary>
    public CCoinPairsM(CDbM db, CCoinsM coins, CCommonM common) : base(db)
    //public CCoinPairsM(CDbM db, CCoinsM coins, CDbSingM dbSign) : base(db, dbSign)
    {
        coinsModel = coins;
        /*db.CoinPairs = GetPairsData();*/
        commonModel = common;
    }

    public IEnumerable<CCoinPairDataM> GetPairsData (string? filter = null)
    {
        var coins = coinsModel.GetTrueCoins(filter).ToArray();
        //var coins = new CCoinDataM[0];

        foreach (var coin1 in coins)
        {
            foreach (var coin2 in coins)
            {
                if (coin1.name == coin2.name || coin1.id > coin2.id) continue;

                yield return new CCoinPairDataM(coin1, coin2, GetMeta(coin1.id, coin2.id));
            }
        }
    }
    
    private List<int[]> GetPairsIndexes(uint shift, int limit)
    {
        var indexes_list = new List<int[]>();
        uint count = this.coinsModel.Count();
        if (count == 0) return indexes_list;

        uint i = shift;
        int current = 0;
        while (current < limit)
        {
            int index1 = (int)(i / count);
            int index2 = (int)(i % count);
            //if (index1 > index2) break;
            i++;
            if (index1 == index2) continue;

            indexes_list.Add(new int[] { index1, index2 });
            Console.WriteLine($"{index1}\t{index2}");

            current++;
        }

        return indexes_list;
    }
    private Dictionary<int, CCoinDataM> GetCoinsByPairsIndexes (List<int[]> indexes_list)
    {
        Dictionary<int, CCoinDataM> result = new Dictionary<int, CCoinDataM>();

        foreach (int[] indexes in indexes_list)
        {
            int index1 = indexes[0];
            int index2 = indexes[1];

            if (!result.ContainsKey(index1))
                result.Add(index1, this.coinsModel.GetCoinByIndex((int)index1));

            if (!result.ContainsKey(index2))
                result.Add(index2, this.coinsModel.GetCoinByIndex((int)index2));
        }

        return result;
    }
    private IEnumerable<CCoinPairDataM>? GetPairsData(uint shift, int limit)
    {
        var indexes_list = GetPairsIndexes(shift, limit);
        var coins = GetCoinsByPairsIndexes(indexes_list);

        foreach (var indexes in indexes_list)
        {
            int index1 = indexes[0];
            int index2 = indexes[1];

            var coin1 = coins[index1];
            var coin2 = coins[index2];

            yield return new CCoinPairDataM(coin1, coin2, GetMeta(coin1.id, coin2.id));
        }
    }

    public IEnumerable<CCoinPairsMetaDataM> GetMeta (uint id1, uint id2)
    {
        return db.CoinPairsMeta
            .Where(m => m.coin_1_id == id1 && m.coin_2_id == id2);
    }

    /// <summary>
    ///     Возвращает количество пар.
    /// </summary>
    public uint Count(string? filter = null)
    {
        int count = coinsModel.TrueCount(filter);

        uint pair_count = 0;
        for (uint i = 1; i < count; i++)
        {
            pair_count += i;
        }

        return pair_count;
    }

    /// <summary>
    ///     Достает пары из БД используя исходное заданное количество и номер страницы.
    /// </summary>
    public IEnumerable<CCoinPairDataVM>? GetPairs(int page, int count, string? filter = null)
    {
        return GetPairsData((uint)(--page * count), count)
            .Select(p => new CCoinPairDataVM()
            {
                data = p,
                coin1 = p.coin_1,
                coin2 = p.coin_2,
                commonModel = commonModel,
            });
    }

    /// <summary>
    ///     Достает похожие пары из БД используя id исходной пары.
    /// </summary>
    public IEnumerable<CCoinPairDataVM> GetPairs(CCoinPairDataM pair)
    {
        return GetPairsData(0, 10)
            .Select(p => new CCoinPairDataVM
            {
                data = p,
                coin1 = p.coin_1,
                coin2 = p.coin_2,
                commonModel = commonModel,
            });
    }

    /// <summary>
    ///     Достает пару из БД используя name монет.
    /// </summary>
    public CCoinPairDataVM GetPairByNames(string name1, string name2)
    {
        var coin1 = coinsModel.GetCoinByName(name1);
        var coin2 = coinsModel.GetCoinByName(name2);
        
        return new CCoinPairDataVM()
        {
            data = new CCoinPairDataM(coin1.data, coin2.data, GetMeta(coin1.data.id, coin2.data.id)),
            coin1 = coin1.data,
            coin2 = coin2.data,
            commonModel = commonModel,
        };
    }

    /// <summary>
    ///     Возвращает число максимально возможной страницы для пагинации.
    /// </summary>
    public int GetMaxPage(int count, string? filter = null)
    {
        int max_count = (int)Math.Ceiling(Count(filter) / count * 1f);
        return max_count;
    }
}
