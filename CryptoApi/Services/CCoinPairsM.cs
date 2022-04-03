﻿using CryptoApi.Api;
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
        yield break;
        var coins = coinsModel.GetTrueCoins().ToArray();

        foreach (var coin1 in coins)
        {
            foreach (var coin2 in coins)
            {
                yield return new CCoinPairDataM(coin1, coin2, GetMeta(coin1.id, coin2.id));
            }
        }
    }

    private IEnumerable<CCoinPairDataM> GetPairsData(uint shift, int limit)
    {
        var coins = coinsModel.GetTrueCoins().ToArray();
        uint count = (uint)coins.Length;
        
        for (uint i = shift; i < shift + limit; i++)
        {
            uint index1 = i / count;
            uint index2 = i % count;

            yield return new CCoinPairDataM(coins[index1], coins[index2], GetMeta(coins[index1].id, coins[index2].id));
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
        return GetPairsData((uint)(--page * count), count)
            .Select(p => new CCoinPairDataVM()
            {
                data = p,
                coin1 = p.coin_1,
                coin2 = p.coin_2
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
                coin2 = p.coin_2
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
            coin2 = coin2.data
        };
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
