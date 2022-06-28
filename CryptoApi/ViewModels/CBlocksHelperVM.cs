using Atomex.Models;
using CryptoApi.Models.DB;
using CryptoApi.Services;

namespace CryptoApi.ViewModels;

/// <summary>
///     Вспомогательный класс (сервис) для удобства работы в файлах cshtml.
/// </summary>
public class CBlocksHelperVM 
{
    CDbM db;
    WebApplication app;
    IConfiguration conf;
    CCoinsM coinsModel;
    CCoinPairsM pairsModel;
    IHttpContextAccessor contextAccessor;
    IWebHostEnvironment env;

    public static CBlocksHelperVM instance;


    /// <summary>
    ///     Конструктор. заполняет  необходимые поля при создании модели.
    /// </summary>
    public CBlocksHelperVM (IWebHostEnvironment env, IHttpContextAccessor context, IConfiguration conf, CDbM db, CCoinsM coins_model, CCoinPairsM pairs_model)
    {
        this.db = db;
        this.coinsModel = coins_model;
        this.pairsModel = pairs_model;
        this.conf = conf;
        contextAccessor = context;
        this.env = env;

        instance = this;
    }
    public string GetUrl ()
    {
        return conf.GetValue<string>("BaseUrl");
    }

    public string ToCurr (decimal num)
    {
        return num.ToString("C", new System.Globalization.CultureInfo("en-US")).Replace(".00", "");
    }
    public List<string[]> GetDownloadBtnData()
    {
        return new List<string[]>()
        {
            new string[] { "windows-brand.svg", "#", "Get Windows wallet ", " dcj-win" },
            new string[] { "apple-brand.svg", "#", "Get macOS wallet", " dcj-mac" },
            new string[] { "ubuntu-brand.svg", "#", "Get Ubuntu wallet", " dcj-linux" },
            new string[] { "android-icon.svg", "https://play.google.com/store/apps/details?id=com.atomex.android", "Get Android wallet", "" },
            new string[] { "apple-brand.svg", "https://apps.apple.com/us/app/atomex-wallet-dex/id1534717828", "Get iOS wallet", "" },
            new string[] { "web-icon.svg", "https://wallet.atomex.me", "Launch Web Wallet", "" }
        };
    }

    public List<string[]> GetDownloadBtnDataMobile()
    {
        return new List<string[]>()
        {
            new string[] { "windows-brand.svg", "https://github.com/atomex-me/atomex.client.desktop/releases/download/1.2.11/Atomex.Client-1.2.11.0-x64.msi", "Get Windows wallet " },
            new string[] { "apple-brand.svg", "https://github.com/atomex-me/atomex.client.desktop/releases/download/1.2.11/Atomex.1.2.11.dmg", "Get macOS wallet" },
            new string[] { "ubuntu-brand.svg", "https://wallet.atomex.me/", "Get Ubuntu wallet" },
            new string[] { "android-icon.svg", "https://play.google.com/store/apps/details?id=com.atomex.android", "Get Android wallet" },
            new string[] { "apple-brand.svg", "https://apps.apple.com/us/app/atomex-wallet-dex/id1534717828", "Get iOS wallet" },
            new string[] { "web-icon.svg", "https://wallet.atomex.me/", "Launch Web Wallet" }
        };
    }

    public CCoinPairDataVM GetExchangeWidgetData()
    {
        return GetPairList(1).FirstOrDefault();
    }
    public CMenuVM GetFooterMenu()
    {
        return new CMenuVM()
        {
            Title = "Atomex",

            Links = new List<CLinkVM>()
            {
                new CLinkVM("Exchange"),
                new CLinkVM("Wallet"),
                new CLinkVM("Downloads"),
                new CLinkVM("Docs"),
            }
        };
    }

    public CMenuVM GetFooterCoinsMenu()
    {
        var coins = GetCoinList(4);

        var links = new List<CLinkVM>();

        foreach (var coin in coins)
        {
            var data = new Dictionary<string, string>() { { "coin", coin.data.name } };
            links.Add(new CLinkVM(coin.data.name_full, "Coins", "Coin", data));
        }

        return new CMenuVM()
        {
            Title = "Supported Coins",
            Links = links,
            MoreLink = new CLinkVM("View all coins", controller: "Coins")
        };
    }

    public CMenuVM GetFooterPairsMenu()
    {
        var pairs = GetPairList(5);

        var links = new List<CLinkVM>();

        foreach (var pair in pairs)
        {
            var data = new Dictionary<string, string>() 
            {
                { "coin1", pair.data.name_1 },
                { "coin2", pair.data.name_2 }
            };

            string title = $"{pair.data.name_1.ToLower()} to {pair.data.name_2.ToLower()}";

            links.Add(new CLinkVM(title, "CoinPairs", "Pair", data));
        }

        return new CMenuVM()
        {
            Title = "Exchange Pairs",
            Links = links,
            MoreLink = new CLinkVM("View all coin pairs", controller: "CoinPairs")
        };
    }

    public string GetActiveClass(string controller, string action = "Index")
    {
        var context = contextAccessor.HttpContext;
        string? r_contr = (string?)context.GetRouteValue("controller");
        string? r_action = (string?)context.GetRouteValue("action");

        return r_contr == controller && r_action == action ? "dcg-active" : "";
    }

    public string GetQuery (string key)
    {
        var context = contextAccessor.HttpContext;
        return context.Request.Query[key].ToString() ?? "";
    }
    /// <summary>
    ///     Возвращает список монет относительно кол-ва и номера страницы.
    /// </summary>
    public IEnumerable<CCoinDataVM> GetCoinList(int count, int page = 1, string filter = "", string? order = "c.id", string order_type = "asc")
    {
        return coinsModel.GetCoins(page, count, filter, order, order_type);
    }
    /// <summary>
    ///     Возвращает список похожих монет относительно текущей.
    /// </summary>
    public IEnumerable<CCoinDataVM> GetCoinList(CCoinDataM coin) => coinsModel.GetCoins(coin);
    
    /// <summary>
    ///     Возвращает список пар относительно кол-ва и номера страницы.
    /// </summary>
    public IEnumerable<CCoinPairDataVM>? GetPairList(int count, int page = 1, string? filter = null)
    {
        return pairsModel.GetPairs(page, count, filter);
    }
    /// <summary>
    ///     Возвращает список похожих пар относительно текущей.
    /// </summary>
    public IEnumerable<CCoinPairDataVM> GetPairList(CCoinPairDataM pair) => pairsModel.GetPairs(pair);
    
    /// <summary>
    ///     Возвращает акутальный список ссылок пагинации.
    /// </summary>
    public List<(string Url, string Title)> GetPagination(int max_page, int page, string link_start = "?page=")
    {
        List<(string Url, string Title)> pag_items = new List<(string Url, string Title)>();
        /*int max_links_per_page = 10;*/
        int dott_backward_page = (int)Math.Ceiling(decimal.Divide(1 + page, 2));
        int dott_forward_page = (int)Math.Ceiling(decimal.Divide(max_page + page, 2));

        /*if (max_page <= max_links_per_page)
        {
            for (int i = 1; i <= max_page; i++) 
                pag_items.Add((link_start + i.ToString(), i.ToString()));

            return pag_items;
        }*/
        if (max_page <= 1) return pag_items;
        
        if (page == 1)
        {
            string _page = page.ToString();
            pag_items.Add((link_start + _page, _page));
        }
        else if (page > 1)
        {
            if (page == 3) pag_items.Add((link_start + "1", "1"));
            else if (page > 3)
            {
                pag_items.Add((link_start + "1", "1"));
                pag_items.Add((link_start + dott_backward_page.ToString(), "..."));
            }
            if (page == max_page - 1) 
                pag_items.Add((link_start + (page - 2).ToString(), (page - 2).ToString()));
            if (page > 2)
            {
                if (page == max_page && max_page >= 4) // ???
                {
                    pag_items.Add((link_start + (page - 3).ToString(), (page - 3).ToString()));
                    pag_items.Add((link_start + (page - 2).ToString(), (page - 2).ToString()));
                }
                if (max_page < 5) // ???
                    pag_items.Add((link_start + (page - 2).ToString(), (page - 2).ToString()));
            }
            pag_items.Add((link_start + (page - 1).ToString(), (page - 1).ToString()));
            pag_items.Add((link_start + page.ToString(), page.ToString()));
        }
        if (page + 1 < max_page)
        {
            pag_items.Add((link_start + (page + 1).ToString(), (page + 1).ToString()));
            
            if (page == 1) pag_items.Add((link_start + (page + 2).ToString(), (page + 2).ToString()));
            if (page <= 2) pag_items.Add((link_start + "4", "4"));
            if (max_page > 4 && page < max_page - 2) pag_items.Add((link_start + dott_forward_page.ToString(), "..."));
        }
        if (max_page > page) pag_items.Add((link_start + max_page.ToString(), max_page.ToString()));

        return pag_items;
    }
}
