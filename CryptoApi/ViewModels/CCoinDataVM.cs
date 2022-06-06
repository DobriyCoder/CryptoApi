using CryptoApi.Models;
using CryptoApi.Models.DB;
using CryptoApi.Services;

namespace CryptoApi.ViewModels;

/// <summary>
///     Вью-Модель данных монеты.
/// </summary>
public class CCoinDataVM
{
    public CCoinDataM data { get; set; }

    public CCommonM commonModel { get; set; }

    public EAvailability? availability
    {
        get
        {
            string? text = data["ext", "availability"] != null ? data["ext", "availability"]?.value : commonModel["coin ext", "availability"]?.value;

            if (!Enum.TryParse(text, out EAvailability result)) return EAvailability.Soon;
            return result;
        }
    }
}
