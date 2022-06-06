using CryptoApi.Models.DB;

namespace CryptoApi.Services;
public class CLogger : ILogger
{
    IConfiguration conf;
    IWebHostEnvironment env;

    public CLogger (IConfiguration conf, IWebHostEnvironment env)
    {
        this.conf = conf;
        this.env = env;
    }

    public void Write(CCoinDataM coin, ELogMode mode)
    {
        string file_name = DateTime.Now.ToString("dd.MM.yyyy") + ".log";
        string path = env.ContentRootPath + conf.GetSection("Logger").GetValue<string>("Path") + file_name;
        string line = $"{DateTime.Now.ToString()}\t {coin.name_full} ({coin.name})\t|\t{mode}\n";
        File.AppendAllText(path, line);
    }
}
