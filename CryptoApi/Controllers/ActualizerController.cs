using CryptoApi.Models.DB;
using CryptoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApi.Controllers;

/// <summary>
///     Контроллер API Актуалайзера. Задача актулизатора - актулизировать данные по курсу валют существующих монет и добавления новых.
/// </summary>
public class ActualizerController : Controller
{
    CActualizerM actualizer;
    private CCoinsM coinsModel;
    private CCoinPairsM coinPairsModel;

    /// <summary>
    ///     Конструктор. заполняет  необходимые поля при создании модели.
    /// </summary>
    public ActualizerController (CActualizerM act, CCoinsM coins, CCoinPairsM pair)
    {
        this.actualizer = act;
        this.coinsModel = coins;
        this.coinPairsModel = pair;
    }

    /// <summary>
    ///     Отображает главную страницу API актуалайзера.
    /// </summary>
    [Route("/actualizer")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    ///     Запускает работу API актуалайзера.
    /// </summary>
    [Route("/actualizer/run")]
    public string Run()
    {
        actualizer.RunAsync();
        return "run";
    }

    [Route("/actualizer/test-meta")]
    public string TestMeta([FromServices] CDbM model, [FromServices] CCoinPairsM pairs)
    {
        int i = 0;
        foreach (var pair in pairs.GetPairsData())
        {
            if (i++ > 10000) break;

            model.CoinPairsMeta.Add(new CCoinPairsMetaDataM()
            {
                coin_1_id = pair.coin1_id,
                coin_2_id = pair.coin2_id,
                group = "pagehead",
                option = "title",
                value = "test title {{name_1}} " + i
            });
        }

        model.SaveChanges();

        return "TestMeta";
    }

    /// <summary>
    ///     Останавливает работу актуалайзера.
    /// </summary>
    [Route("/actualizer/stop")]
    public string Stop()
    {
        actualizer.StopAsync();
        return "stop";
    }

    /// <summary>
    ///     Запускает работу API актуалайзера.
    /// </summary>
    [Route("/actualizer/run-now")]
    public string RunNow()
    {
        actualizer.RunNowAsync();
        return "RunNow";
    }

    /// <summary>
    ///     Очищает данные API актуалайзера.
    /// </summary>
    [Route("/actualizer/clear")]
    public string Clear()
    {
        actualizer.ClearAllAsync();
        return "Clear";
    }

    /// <summary>
    ///     Выводит количество монет используя API актуалайзер.
    /// </summary>
    [Route("/actualizer/coins-count")]
    public string CoinsCount()
    {
        return $"Coins count: {coinsModel.Count()}";
    }

    /// <summary>
    ///     Выводит количество пар используя API актуалайзер.
    /// </summary>
    [Route("/actualizer/pairs-count")]
    public string PairsCount()
    {
        return $"Pairs count: {coinPairsModel.Count()}";
    }

    /// <summary>
    ///     Тест API актуалайзера.
    /// </summary>
    [Route("/actualizer/test")]
    public string Test()
    {
        actualizer.TestAsync();
        return "test";
    }
}
