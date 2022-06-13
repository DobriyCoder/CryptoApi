using CryptoApi.Models.DB;

namespace CryptoApi.Services;
public class CLogger : ILogger
{
    IConfiguration conf;
    IWebHostEnvironment env;
    public static CLogger instance;

    public CLogger (IConfiguration conf, IWebHostEnvironment env)
    {
        this.conf = conf;
        this.env = env;
        instance = this;
    }

    public void Write(CCoinDataM coin, ELogMode mode)
    {
        string line = $"{coin.name_full} ({coin.name})\t|\t{mode}\n";
        Write(line);
    }

    public void Write(string line)
    {
        line = $"{DateTime.Now.ToString()}\t | {line}";
        string file_name = DateTime.Now.ToString("dd.MM.yyyy") + ".log";
        string path = env.ContentRootPath + conf.GetSection("Logger").GetValue<string>("Path") + file_name;
        Console.WriteLine(line);

        try
        {
            //if (!File.Exists(path)) File.Create(path);
            File.AppendAllText(path, line);
        }
        catch (Exception ex) { Console.WriteLine(ex.ToString()); }
    }
}
