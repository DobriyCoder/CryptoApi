using CryptoApi.Models.DB;

namespace CryptoApi.Services;

public class CBaseDbM
{
    protected CDbM db;
    protected CDbM dbSign;
    /// <summary>
    ///     Конструктор модели сервиса базы данных.
    /// </summary>
    public CBaseDbM (CDbM db)
    {
        this.db = db;
        this.dbSign = dbSign;
    }
}
