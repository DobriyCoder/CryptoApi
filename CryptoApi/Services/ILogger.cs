using CryptoApi.Models.DB;

namespace CryptoApi.Services;

public enum ELogMode
{
    Add,
    Update
}
public interface ILogger
{
    void Write(CCoinDataM coin, ELogMode mode);
}
