﻿using CryptoApi.Models.DB;
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

    
    /// <summary>
    ///     Конструктор. заполняет  необходимые поля при создании модели.
    /// </summary>
    public CBlocksHelperVM (IHttpContextAccessor context, IConfiguration conf, CDbM db, CCoinsM coins_model, CCoinPairsM pairs_model)
    {
        this.db = db;
        this.coinsModel = coins_model;
        this.pairsModel = pairs_model;
        this.conf = conf;
        contextAccessor = context;
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
    public IEnumerable<CCoinDataVM> GetCoinList(int count, int page = 1, string filter = "", string? order = null, string order_type = "ask")
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
