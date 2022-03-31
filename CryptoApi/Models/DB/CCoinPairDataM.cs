﻿namespace CryptoApi.Models.DB;

/// <summary>
///     Структурная модель данных CoinPairData из БД.
/// </summary>
public class CCoinPairDataM
{
    public uint coin1_id => coin_1.id;
    public uint coin2_id => coin_2.id;
    public decimal? price_1 => CCurrMath.Exchange(coin_1.ext.LastOrDefault()?.usd_price, coin_2.ext.LastOrDefault()?.usd_price);
    public decimal? price_2 => CCurrMath.Exchange(coin_2.ext.LastOrDefault()?.usd_price, coin_1.ext.LastOrDefault()?.usd_price);
    public decimal? day_percent => 0;// { get; set; }
    public decimal? day_high_1 => 0;// { get; set; }
    public decimal? day_high_2 => 0;// { get; set; }
    public decimal? day_low_1 => 0;// { get; set; }
    public decimal? day_low_2 => 0;// { get; set; }
    public decimal? market_cap => 274;// { get; set; }
    public IEnumerable<CCoinPairsMetaDataM> meta { get; private set; }
    public CCoinDataM coin_1 { get; private set; }
    public CCoinDataM coin_2 { get; private set; }

    public CCoinPairDataM(CCoinDataM coin1, CCoinDataM coin2, IEnumerable<CCoinPairsMetaDataM> meta)
    {
        coin_1 = coin1;
        coin_2 = coin2;
        this.meta = meta;
    }

    /// <summary>
    ///     Возвращает все метаданные монеты, при условии совпадения group.
    /// </summary>
    public IEnumerable<CCoinPairsMetaDataM> this[string group]
    {
        get => meta.Where(x => x.group == group);
    }
    /// <summary>
    ///     Возвращает все метаданные монеты, при условии совпадения group & option.
    /// </summary>
    public CCoinPairsMetaDataM this[string group, string option]
    {
        get => meta.Where(x => x.group == group && x.option == option).FirstOrDefault();
    }

    
}
