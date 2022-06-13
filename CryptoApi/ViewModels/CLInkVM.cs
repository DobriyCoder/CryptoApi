namespace CryptoApi.ViewModels;

/// <summary>
///     Рекорд, структурная модель текстового блока.
/// </summary>
public record CLinkVM (string title, string? controller = null, string action = "Index", Dictionary<string, string>? data = null);
