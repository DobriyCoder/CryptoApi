using CryptoApi.Api;
using CryptoApi.Models.DB;
using CryptoApi.Sitemap;
using Microsoft.EntityFrameworkCore;

namespace CryptoApi.Services
{
    public class CActualizerM
    {
        private CDbM db;
        private CCoinsM coinsModel;
        private CCoinPairsM coinPairsModel;
        private CApiManager api;
        private IConfiguration conf;
        private IRunnerM runner;
        private ISitemap sitemap;
        private IServiceProvider serviceProvider;

        public CActualizerM (CDbM db, CCoinsM coins, CCoinPairsM pair, CApiManager api, IRunnerM runner, ISitemap sitemap, IServiceProvider services)
        {
            this.db = db;
            this.coinsModel = coins;
            this.coinPairsModel = pair;
            this.api = api;
            this.runner = runner;
            this.sitemap = sitemap;
            this.serviceProvider = services;

            conf = new ConfigurationBuilder().AddJsonFile("ConfApi.json").Build();
            var donors = conf.GetSection("Donors");
            api.Init(donors);
        }
        public async Task RefreshDataAsync ()
        {
            await RefreshCoinsAsync();
            //await sitemap.CreateAsync();
        }
        public async Task RefreshCoinsAsync()
        {
            var coins = api.GetCoinsAsync().Result;
            Console.Write("var coins = api.GetCoinsAsync().Result;");
            await coinsModel.AddCoinsAsync(coins);
        }
        public async Task RunAsync ()
        {
            runner.Run(Int32.Parse(conf["ActualizeTime"]), () => RefreshDataAsync());
        }

        public async Task RunNowAsync ()
        {
            await Task.Run(RefreshDataAsync);
        }

        public async Task LoadCoinsAsync()
        {
            await Task.Run(RefreshCoinsAsync);
        }

        public async Task TestAsync()
        {
            await Task.Delay(5000);

            try
            {
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task StopAsync ()
        {
            runner.Stop();
        }

        public async Task ClearCoinsAsync()
        {
            db.Coins.RemoveRange(db.Coins);
            await db.SaveChangesAsync();
        }

        public async Task ClearAllAsync()
        {
            await ClearCoinsAsync();
        }
    }
}
