namespace CryptoApi.ViewModels;

/// <summary>
///     Рекорд, структурная модель текстового блока.
/// </summary>
public class CMenuVM
{
    public string Title { get; set; }
    public List<CLinkVM> Links { get; set; }
    public CLinkVM MoreLink { get; set; }
}

