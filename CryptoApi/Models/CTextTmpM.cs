using System.Text.RegularExpressions;

namespace CryptoApi.Models;
/// <summary>
///     Класс предназначен для работы с парсингом текста, а так же конкатенации словарей.
/// </summary>
static public class CTextTmpM
{
    static private Dictionary<string, string> ParsePairs (string pairs)
    {
        Dictionary<string, string> result = new Dictionary<string, string> ();
        if (pairs == null) return result;
        string[] pairs_data = pairs.Split(";");

        foreach (string pair in pairs_data)
        {
            if (pair.Trim() == "") continue;
            string[] pair_data = pair.Split("=");
            
            result.Add(pair_data[0].Trim(), pair_data[1].Trim().Replace("\"", ""));
        }

        return result;
    }
    static private Dictionary<string, string> ParseFields(object? fields)
    {
        var result = new Dictionary<string, string> ();
        if (fields == null) return result;
        Type type = fields.GetType();
        var fields_info = type.GetProperties();
        
        foreach (var field in fields_info)
        {
            if (
                field.Name == "id" ||
                /*field.Name == "donor" ||
                field.Name == "donor_id" ||
                field.Name == "name_full" ||
                field.Name == "name" ||
                field.Name == "slug" ||
                field.Name == "image" ||
                field.Name == "last_updated" ||
                field.Name == "enable" ||
                field.Name == "day_change" ||*/
                /*field.Name == "day_percent_change" ||
                field.Name == "week_percent_change" ||
                field.Name == "month_percent_change" ||
                field.Name == "usd_price" ||
                field.Name == "market_cap" ||
                field.Name == "low" ||
                field.Name == "high" ||
                field.Name == "circulating_supply" ||
                field.Name == "max_supply" ||
                field.Name == "cmc_rank" ||
                field.Name == "volume_24h" ||
                field.Name == "change_1h" ||*/
                field.Name == "Item" ||
                field.Name == "l_ext" || 
                field.Name == "true_ext" || 
                field.Name == "ext" || 
                field.Name == "meta") 
                    continue;

            result.Add(field.Name, (string)(field.GetValue(fields)?.ToString() ?? ""));
        }

        return result;
    }
    static private string ParseText(string text, Dictionary<string, string> pairs)
    {
        if (text == null) return "";
        string new_text = text;

        new_text = Regex.Replace(text, @"{{(.+?)}}", match =>
        {
            if (!pairs.ContainsKey(match.Groups[1].Value)) return match.Groups[1].Value;
            return pairs[match.Groups[1].Value];
        });

        return new_text;
    }
    static private Dictionary<string, string> Concat(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
    {
        foreach (var pair in dict2)
        {
            if (dict1.ContainsKey(pair.Key))
                dict1[pair.Key] = pair.Value;
            else
                dict1.Add(pair.Key, pair.Value);
        }

        return dict1;
    }
    static public string Parse (string text, string? pairs, object? fields = null)
    {
        var pairs_data = ParsePairs(pairs);
        var fields_data = ParseFields(fields);
        pairs_data = Concat(pairs_data, fields_data);
        string new_text = ParseText(text, pairs_data);
        
        return new_text;
    }
}
