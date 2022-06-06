using Microsoft.EntityFrameworkCore;

namespace CryptoApi.Models.DB;

/// <summary>
///     Модель БД.
/// </summary>
public class CDbSingM: CDbM
{
    /// <summary>
    ///     Конструктор. Создает базу данных при первом обращении.
    /// </summary>
    public CDbSingM(DbContextOptions<CDbM> options)
        : base(options)
    {
    }
}
