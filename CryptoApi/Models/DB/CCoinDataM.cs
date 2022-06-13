using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoApi.Models.DB;

/// <summary>
///     Структурная модель данных CoinData из БД.
/// </summary>
public class CCoinDataM
{
    public uint id { get; set; }
    public string donor { get; set; }
    public string donor_id { get; set; }
    public string name_full { get; set; }
    public string name { get; set; }
    public string slug { get; set; }
    private string? _image { get; set; }
    public string? image
    {
        get
        {
            return _image;
            return _image != null ? _image : "/images/coin-unknown.png";
        }
        set
        {
            _image = value;
        }
    }
    public DateTime last_updated { get; set; }
    public bool? enable { get; set; }

    public decimal? day_change => true_ext.Count() == 0 ? null : CCurrMath.GetChangePrice(1, true_ext);

    public decimal? day_percent_change => true_ext.Count() == 0 ? null : CCurrMath.GetChangePercentPrice(1, true_ext);
    public decimal? week_percent_change => true_ext.Count() == 0 ? null : CCurrMath.GetChangePercentPrice(7, true_ext);
    public decimal? month_percent_change => true_ext.Count() == 0 ? null : CCurrMath.GetChangePercentPrice(30, true_ext);

    public decimal? usd_price => true_ext.Count() == 0 ? 0 : true_ext.Last()?.usd_price;

    public decimal? market_cap => true_ext.Count() == 0 ? null : true_ext.Last()?.market_cap;
    public decimal? low => true_ext.Count() == 0 ? null : true_ext.Last()?.low;
    public decimal? high => true_ext.Count() == 0 ? null : true_ext.Last()?.high;

    public string circulating_supply => true_ext.Count() == 0 ? "" : true_ext.Last().circulating_supply;

    public string max_supply => true_ext.Count() == 0 ? null : true_ext.Last().total_supply;
    public decimal? cmc_rank => true_ext.Count() == 0 ? null : true_ext.Last().market_cap_rank;
    public decimal? volume_24h => true_ext.Count() == 0 ? null : true_ext.Last().total_volume;
    public decimal? change_1h => true_ext.Count() == 0 ? null : 2.21m;

    public ICollection<CCoinsMetaDataM> meta { get; set; }
    public ICollection<CCoinsExtDataM> ext { get; set; }

    [NotMapped]
    public IEnumerable<CCoinsExtDataM> true_ext => ext.Count > 0 ? ext : l_ext;

    [NotMapped]
    public List<CCoinsExtDataM> l_ext { get; set; }
    /// <summary>
    /// 
    ///     Конструктор. заполняет  необходимые поля при создании модели.
    /// </summary>
    public CCoinDataM ()
    {
        meta = new List<CCoinsMetaDataM> ();
        ext = new List<CCoinsExtDataM> ();
    }

    /// <summary>
    ///     Возвращает все метаданные монеты, при условии совпадения group.
    /// </summary>
    public IEnumerable<CCoinsMetaDataM> this[string group]
    {
        get => meta.Where(x => x.group == group);
    }

    /// <summary>
    ///     Возвращает все метаданные монеты, при условии совпадения group & option.
    /// </summary>
    public CCoinsMetaDataM? this[string group, string option]
    {
        get => meta?.Where(x => x.group == group && x.option == option).FirstOrDefault();
    }
}
