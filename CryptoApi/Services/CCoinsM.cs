using Atomex.Models;
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
    IWebHostEnvironment env;

    /// <summary>
    ///     Конструктор. Передает модель БД родителю.
    /// </summary>
    public CCoinsM(IWebHostEnvironment env, CDbM db, IConfiguration conf, CCommonM common, ILogger logger) : base(db) 
    {
        this.conf = conf;
        this.commonModel = common;
        this.logger = logger;
        this.env = env;
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
        try
        {
            var true_coin = ApiToData(coin);
            db.Coins.Add(true_coin);
            logger.Write(true_coin, ELogMode.Add);
        }
        catch (Exception ex)
        {
            logger.Write($"AddCoinAsync err: {ex.Message}");
        }
        if (save) await db.SaveChangesAsync();
    }

    /// <summary>
    ///     Асинхронно обновляет монету.
    /// </summary>
    public async Task UpdateCoinAsync(IApiCoin coin, CCoinDataM? has_coin, bool save = true)
    {
        try
        {
            ApiToData(coin, has_coin);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UpdateCoinAsync err: {ex}");
        }

        if (save) await db.SaveChangesAsync();
        //logger.Write(has_coin, ELogMode.Update);
    }

    public string? SaveCoinIcon(string url, string name)
    {
        string icon_url = conf.GetValue<string>("BaseUrl") + "coin-icons/";
        string path = env.WebRootPath.TrimEnd('/') + icon_url;
        string? file_name = CImgLoader.Load(url, path, name);

        return file_name == null ? file_name : icon_url + file_name;
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
        new_coin.name = coin.Name.ToUpper();
        new_coin.slug = coin.Name;
        new_coin.image = new_coin.image == null || new_coin.image == "" ? SaveCoinIcon(coin.Image, coin.Name) : new_coin.image;
        new_coin.last_updated = now;

        if (coin.UsdPrice == null) return new_coin;

        var ext = new CCoinsExtDataM()
        {
            usd_price = coin.UsdPrice,
            market_cap = coin.MarketCap,
            low = coin.Low,
            high = coin.High,
            last_updated = now,
            circulating_supply = "",//coin.CirculatingSupply,
            total_supply = "",//coin.TotalSupply?.ToString() ?? "",
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
            try
            {
                var has_coin = HasCoin(coin);

                if (has_coin != null && has_coin.donor_id != coin.Id)
                {
                    coin.Name = coin.Id;
                    has_coin = HasCoin(coin);
                }

                if (has_coin != null)
                    await UpdateCoinAsync(coin, has_coin, false);
                else
                    await AddCoinAsync(coin, false);

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                CLogger.instance.Write($"AddCoinsAsync err: {ex} {ex.Message}");
            }
        }

        db.SaveChanges();

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
    public IEnumerable<CCoinDataVM> GetCoins(int page, int count, string? filter = null, string? order = "c.id", string order_type = "asc")
    {

        string query = 
            $"select c.* from coins as c" +
                $" join coinsext as e on c.id = e.coins_id" +
                $" where c.last_updated = e.last_updated and c.[enable] = 1" +
                //$" order by {order} {order_type}" +
                $" order by e.total_volume desc" +
                $" offset {(page - 1) * count} rows" +
                $" fetch next {count} rows ONLY"
        ;
        var coins = db.Coins.FromSqlRaw(query)
            .Select(c => new CCoinDataVM()
            {
                data = c,
                commonModel = commonModel
            })
            .ToList();
        ;

        coins = JoinExtToCoins(coins);
        
        return coins;
    }
    public List<CCoinDataVM> JoinExtToCoins (List<CCoinDataVM> coins)
    {
        string query = $"select * from coinsext where ";

        foreach (var coin in coins)
        {
            query += $"coins_id = {coin.data.id} or ";
        }

        query = query.TrimEnd(" or ".ToCharArray());

        var ext = db.CoinsExt.FromSqlRaw(query).ToList();
        
        foreach (var coin in coins)
        {
            coin.data.l_ext = ext.Where(e => e.coins_id == coin.data.id).ToList();
        }

        return coins;
    }
    public IEnumerable<CCoinDataM> GetCoins() => db.Coins.Where(c => c.enable.Value);
    public CCoinDataM GetCoinByIndex(int index)
    {
        string query =
            $"select c.* from coins as c" +
                $" where c.[enable] = 1" +
                $" order by c.id" +
                $" offset {index} rows" +
                $" fetch next 1 rows ONLY"
        ;

        var coins = db.Coins.FromSqlRaw(query)
            .Select(c => new CCoinDataVM()
            {
                data = c,
                commonModel = commonModel
            })
            .ToList();
        ;

        coins = JoinExtToCoins(coins);

        return coins.First().data;

        //return db.Coins.Skip(index).Include(c => c.ext).Where(c => c.enable.Value).First();
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
        int max_count = (int)Math.Ceiling(TrueCount(filter) / count * 1f);
        return max_count;
    }

    public void Clear ()
    {
        db.Coins.RemoveRange(db.Coins);
    }
}
