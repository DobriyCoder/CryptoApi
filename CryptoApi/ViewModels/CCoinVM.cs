﻿using CryptoApi.Services;

namespace CryptoApi.ViewModels;

/// <summary>
///     Вью-Модель (сервис) страницы монеты.
/// </summary>
public class CCoinVM
{
    public CCoinDataVM coin;
    private CCoinsM model;
    private IConfiguration conf;
    private CCommonM commonModel;

    public CTextBlockVM SeoInfo
    {
        get
        {
            string? title = coin.data["seo", "title"]?.value ?? commonModel["coin seo", "title"]?.value ?? "";
            string? text = coin.data["seo", "text"]?.value ?? commonModel["coin seo", "text"]?.value ?? "";
            
            return new CTextBlockBuilder()
                .SetTitle(title)
                .SetTitleValues(coin.data["seo tpl", "title"]?.value ?? "")
                .SetTitleData(coin.data)
                .SetText(text)
                .SetTextValues(coin.data["seo tpl", "text"]?.value ?? "")
                .SetTextData(coin.data)
                .Build();
        }
    }

    /// <summary>
    ///     Возвращает заголовок страницы из БД.
    /// </summary>
    public CTextBlockVM PageHead
    {
        get
        {
            string title = coin.data["pagehead", "title"] != null ? coin.data["pagehead", "title"].value : commonModel["coin pagehead", "title"]?.value;
            string text = coin.data["pagehead", "text"] != null ? coin.data["pagehead", "text"].value : commonModel["coin pagehead", "text"]?.value;

            return new CTextBlockBuilder()
                .SetTitle(title)
                .SetTitleValues(coin.data["pagehead tpl", "title"]?.value)
                .SetTitleData(coin.data)
                .SetText(text)
                .SetTextValues(coin.data["pagehead tpl", "title"]?.value)
                .SetTextData(coin.data)
                .Build();
        }
    }
    /// <summary>
    ///     Используя паттерн Buider генерирует модель текстового блока "Описания" и возвращает ее.
    /// </summary>
    public CTextBlockVM Description
    {
        get 
        {
            string desc = coin.data["description", "text"] != null ? coin.data["description", "text"].value : commonModel["coin desc", "text"]?.value;
            string title = coin.data["description", "title"] != null ? coin.data["description", "title"].value : commonModel["coin desc", "title"]?.value;

            return new CTextBlockBuilder()
                .SetTitle(title)
                .SetTitleValues(coin.data["desc tpl", "title"]?.value)
                .SetTitleData(coin.data)
                .SetText(desc)
                .SetTextValues(coin.data["desc tpl", "text"]?.value)
                .SetTextData(coin.data)
                .Build();
        }
    }

    /// <summary>
    ///     Используя метод GetTextBlock генерирует перечисление моделей текстовых блоков и возвращает их.
    /// </summary>
    public IEnumerable<CTextBlockVM> TextBlocks => GetTextBlocks("coin texts", "texts");

    /// <summary>
    ///     Используя метод GetTextBlock генерирует перечисление моделей текстовых блоков "Faq" и возвращает их.
    /// </summary>
    public IEnumerable<CTextBlockVM> Faq => GetTextBlocks("coin faq", "faq");

    /// <summary>
    ///     Генерирует перечисление моделей текстовых блоков по имени группы и возвращает их.
    /// </summary>
    public IEnumerable<CTextBlockVM> GetTextBlocks(string group, string coin_group)
    {
        for (var i = 0; i < commonModel[group].Count() / 2; i++)
        {
            string coin_title = coin.data[coin_group, $"title{i + 1}"]?.value ?? "";
            string coin_text = coin.data[coin_group, $"text{i + 1}"]?.value ?? "";

            string title = coin_title != "" ? coin_title : commonModel[group, $"title{i + 1}"]?.value ?? "";
            string text = coin_text != "" ? coin_text : commonModel[group, $"text{i + 1}"]?.value ?? "";

            yield return new CTextBlockBuilder()
                .SetTitle(title)
                .SetTitleValues(coin.data["info tpl", "title"]?.value)
                .SetTitleData(coin.data)
                .SetText(text)
                .SetTextValues(coin.data["info tpl", "text"]?.value)
                .SetTextData(coin.data)
                .Build();
        }
    }

    /// <summary>
    ///     Конструктор. заполняет  необходимые поля при создании модели.
    /// </summary>
    public CCoinVM (CCoinsM model, IConfiguration conf, CCommonM common_model)
    {
        this.model = model;
        this.conf = conf;
        this.commonModel = common_model;
    }

    /// <summary>
    ///     Ручная инициализация (заполнение нужных полей).
    /// </summary>
    public void Init (HttpContext context)
    {
        string? name = (string?)context.GetRouteValue("coin");
        coin = model.GetCoinByName(name);
    }
}
