﻿using CryptoApi.Services;
namespace CryptoApi.ViewModels;

/// <summary>
///     Вью-Модель (сервис) домашней страницы.
/// </summary>
public class CHomeVM
{
    private IConfiguration conf;
    private CCommonM commonModel;

    /// <summary>
    ///     Возвращает заголовок страницы из БД.
    /// </summary>
    public CTextBlockVM SeoInfo
    {
        get => new CTextBlockBuilder()
                .SetTitle(commonModel["home seo", "title"]?.value)
                .SetText(commonModel["home seo", "text"]?.value)
                .Build();
    }

    /// <summary>
    ///     Возвращает заголовок страницы из БД.
    /// </summary>
    public CTextBlockVM PageHead
    {
        get => new CTextBlockBuilder()
                .SetTitle(commonModel["home pagehead", "title"]?.value)
                .SetText(commonModel["home pagehead", "text"]?.value)
                .Build();
    }

    /// <summary>
    ///     Используя паттерн Buider генерирует модель текстового блока "Описания" и возвращает ее.
    /// </summary>
    public IEnumerable<CTextBlockVM> TextBlocks => GetTextBlock("home texts");

    /// <summary>
    ///     Используя метод GetTextBlock генерирует перечисление моделей текстовых блоков и возвращает их.
    /// </summary>
    public IEnumerable<CTextBlockVM> Faq => GetTextBlock("home faq");
    public IEnumerable<string[]> Benefits => GetTextImgBlock("home benefits");

    /// <summary>
    ///     Используя метод GetTextBlock генерирует перечисление моделей текстовых блоков "Faq" и возвращает их.
    /// </summary>
    public IEnumerable<CTextBlockVM> GetTextBlock(string group)
    {
        for (var i = 0; i < commonModel[group].Count() / 2; i++)
        {
            string title = commonModel[group, $"title{i + 1}"]?.value ?? "";
            string text = commonModel[group, $"text{i + 1}"]?.value ?? "";

            yield return new CTextBlockBuilder()
                .SetTitle(title)
                .SetText(text)
                .Build();
        }
    }

    public IEnumerable<string[]> GetTextImgBlock(string group)
    {
        for (var i = 0; i < commonModel[group].Count() / 3; i++)
        {
            string title = commonModel[group, $"title{i + 1}"]?.value ?? "";
            string text = commonModel[group, $"text{i + 1}"]?.value ?? "";
            string img = commonModel[group, $"img{i + 1}"]?.value ?? "";

            yield return new string[] { title, text, img };
        }
    }

    /// <summary>
    ///     Конструктор. заполняет  необходимые поля при создании модели.
    /// </summary>
    public CHomeVM(IConfiguration conf, CCommonM common_model)
    {
        this.conf = conf;
        this.commonModel = common_model;
    }
}
