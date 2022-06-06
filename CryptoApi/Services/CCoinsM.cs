using CryptoApi.Api;
using CryptoApi.Models.DB;
using CryptoApi.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CryptoApi.Services;

/// <summary>
///     Модель (сервис), для работы с CoinPairs из БД.
/// </summary>
public class CCoinsM : CBaseDbM
{
    IConfiguration conf;
    CCommonM commonModel;
    ILogger logger;
    /// <summary>
    ///     Конструктор. Передает модель БД родителю.
    /// </summary>
    public CCoinsM(CDbM db, IConfiguration conf, CCommonM common, ILogger logger) : base(db) 
    {
        this.conf = conf;
        this.commonModel = common;
        this.logger = logger;
    }
    //public CCoinsM(CDbM db, CDbSingM dbSign) : base(db, dbSign) { }

    /// <summary>
    ///     Возвращает словарь с моделями монет по ключу name монеты.
    /// </summary>
    public Dictionary<string, CCoinDataM> GetCoinsByNames(string[] names)
    {
        var coins = db.Coins.Where(c => names.Contains(c.name)).Select(c => c).ToList();
        var coins_dict = new Dictionary<string, CCoinDataM>();
        
        foreach (var coin in coins)
        {
            coins_dict.TryAdd(coin.name, coin);
        }

        return coins_dict;
    }

    /// <summary>
    ///     Проверяет, есть ли пара в БД, используя donor & id монеты.
    /// </summary>
    public CCoinDataM? HasCoin(string donor, string name)
    {
        return db.Coins.Where(c => c.name.ToUpper() == name.ToUpper()).FirstOrDefault();
    }

    /// <summary>
    ///     Проверяет, есть ли пара в БД, используя id монеты.
    /// </summary>
    public CCoinDataM? HasCoin (CCoinDataM coin)
    {
        return HasCoin(coin.donor, coin.name);
    }

    /// <summary>
    ///     Проверяет, есть ли пара в БД, используя name монеты.
    /// </summary>
    public CCoinDataM? HasCoin(IApiCoin coin)
    {
        return HasCoin(coin.Donor, coin.Name);
    }

    /// <summary>
    ///     Асинхронно добавляет монету.
    /// </summary>
    public async Task AddCoinAsync(IApiCoin coin, bool save = true)
    {
        var true_coin = ApiToData(coin);
        await db.Coins.AddAsync(true_coin);
        if (save) await db.SaveChangesAsync();
        logger.Write(true_coin, ELogMode.Add);
    }

    /// <summary>
    ///     Асинхронно обновляет монету.
    /// </summary>
    public async Task UpdateCoinAsync(IApiCoin coin, CCoinDataM? has_coin, bool save = true)
    {
        ApiToData(coin, has_coin);
        if (save) await db.SaveChangesAsync();
        logger.Write(has_coin, ELogMode.Update);
    }

    /// <summary>
    ///     Асинхронно вытягивает монету.
    /// </summary>
    public CCoinDataM ApiToData (IApiCoin coin, CCoinDataM? data = null)
    {
        var new_coin = data ?? new CCoinDataM();
        var now = DateTime.Now;

        if (data == null)
            new_coin.enable = true;

        new_coin.donor = coin.Donor;
        new_coin.donor_id = coin.Id;
        new_coin.name_full = coin.FullName;
        new_coin.name = coin.Name;
        new_coin.slug = coin.Name;
        new_coin.image = coin.Image ?? new_coin.image;
        new_coin.last_updated = now;

        if (coin.UsdPrice == null) return new_coin;

        var ext = new CCoinsExtDataM()
        {
            usd_price = coin.UsdPrice,
            market_cap = coin.MarketCap,
            low = coin.Low,
            high = coin.High,
            last_updated = now,
            circulating_supply = coin.CirculatingSupply,
            total_supply = coin.TotalSupply.ToString(),
            market_cap_rank = coin.MarketCapRank,
            total_volume = coin.TotalVolume
        };

        new_coin.ext.Add(ext);

        return new_coin;
    }
    private CDbM CreateDb ()
    {
        var optionsBuilder = new DbContextOptionsBuilder<CDbM>();

        var options = optionsBuilder
                .UseSqlServer(conf.GetConnectionString("DefaultConnection"))
                .Options;

        return new CDbM(options);
    }
    /// <summary>
    ///     Асинхронно добавляет монеты.
    /// </summary>
    public async Task AddCoinsAsync(IEnumerable<IApiCoin> coins)
    {
        if (coins == null) return;
        var old_db = this.db;
        this.db = CreateDb();

        foreach (var coin in coins)
        {

            var has_coin = HasCoin(coin);
            
            if (has_coin != null)
                await UpdateCoinAsync(coin, has_coin, false);
            else
                await AddCoinAsync(coin, false);

            await db.SaveChangesAsync();
        }

        await db.SaveChangesAsync();

        this.db.Dispose();
        this.db = old_db;
    }

    /// <summary>
    ///     Возвращает количество монет.
    /// </summary>
    public uint Count(string? filter = null)
    {
        return (uint)db.Coins
            .Where(c => (filter == "" || filter == null) ? true : c.name.Contains(filter) || c.name_full.Contains(filter))
            .Count();
    }

    public int TrueCount(string? filter = null)
    {
        filter = filter == null ? "" : filter;

        var coins = db.Coins
            .Where(c => c.enable.Value && (filter == "" || c.name.Contains(filter) || c.name_full.Contains(filter)));

        return coins.Count();
    }
    int Test(CCoinDataM c)
    {
        //Console.WriteLine($"{c.name}");
        return (int)c.id;
    }
    /// <summary>
    ///     Достает монеты из БД используя исходное заданное количество и номер страницы.
    /// </summary>
    public IEnumerable<CCoinDataVM> GetCoins(int page, int count, string? filter = null, string? order = null, string order_type = "ask")
    {
        /*if (filter != "" && filter != null)
            result = result.Where(c => c.name.Contains(filter) || c.name_full.Contains(filter));*/
        
        var query = db.Coins.AsQueryable()
            .Include(c => c.ext)
            .Include(c => c.meta)
            .Where(c => c.enable.Value)
            .Skip((page - 1) * count)
            .Take(count)
            .Select(c =>
                new CCoinDataVM()
                {
                    data = c,
                    commonModel = commonModel
                }
            )
        ;

        if (order != null)
            if (order_type == "ask")
                return query.AsEnumerable().OrderBy(c =>
                {
                    Type type = typeof(CCoinDataM);
                    return type.GetProperty(order)?.GetValue(c.data, null) ?? 0;
                });
            else
                return query.AsEnumerable().OrderByDescending(c =>
                {
                    Type type = typeof(CCoinDataM);
                    return type.GetProperty(order)?.GetValue(c.data, null) ?? 0;
                });

        return query;
    }
    public IEnumerable<CCoinDataM> GetCoins() => db.Coins.Where(c => c.enable.Value);
    public CCoinDataM GetCoinByIndex(int index)
    {
        return db.Coins.Skip(index).Include(c => c.ext).Where(c => c.enable.Value).First();
    }
    public IEnumerable<CCoinDataM> GetTrueCoins(string? filter = null)
    {
        filter = filter == null ? "" : filter;

        return db.Coins
            .Where(c => c.enable.Value && (filter == "" || c.name.Contains(filter) || c.name_full.Contains(filter)))
            .Include(c => c.ext);
        //.Where(c => c.ext.Count() > 0);
    }

    /// <summary>
    ///     Достает похожие монеты из БД используя id исходной пары.
    /// </summary>
    public IEnumerable<CCoinDataVM> GetCoins(CCoinDataM coin)
    {
        return GetCoins(1, 10);
        return from item in coin["coins"]
               where item.coins_id == coin.id
               select new CCoinDataVM() { data = db.Coins.Find(uint.Parse(item.value)) };
    }

    /// <summary>
    ///     Достает монету из БД используя name монеты.
    /// </summary>
    public CCoinDataVM GetCoinByName (string name)
    {
        return db.Coins
            .Where(c => c.name == name)
            .Include(c => c.meta)
            .Include(c => c.ext)
            .Select(c => new CCoinDataVM()
            {
                data = c
            })
            .FirstOrDefault();
    }

    /// <summary>
    ///     Возвращает число максимально возможной страницы для пагинации.
    /// </summary>
    public int GetMaxPage (int count, string? filter = null)
    {
        int max_count = (int)Math.Ceiling(Count(filter) / count * 1f);
        return max_count;
    }

    public void Clear ()
    {
        db.Coins.RemoveRange(db.Coins);
    }
}
