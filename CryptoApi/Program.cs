using CryptoApi.Api;
using CryptoApi.Models.DB;
using CryptoApi.Services;
using CryptoApi.Sitemap;
using CryptoApi.ViewModels;
using Microsoft.EntityFrameworkCore;
using ILogger = CryptoApi.Services.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CDbM>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
, ServiceLifetime.Transient);

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<CCommonM>();
builder.Services.AddTransient<CCoinsM>();
builder.Services.AddTransient<CCoinPairsM>();

builder.Services.AddTransient<CBlocksHelperVM>();
builder.Services.AddTransient<CCoinsVM>();
builder.Services.AddTransient<CCoinVM>();
builder.Services.AddTransient<CCoinPairsVM>();
builder.Services.AddTransient<CCoinPairVM>();
builder.Services.AddTransient<CHomeVM>();

builder.Services.AddTransient<CActualizerM>();
builder.Services.AddTransient<CApiManager>();
builder.Services.AddTransient<IRunnerM, CRunnerM>();
builder.Services.AddTransient<ISitemap, CSitemap>();
builder.Services.AddTransient<ILogger, CLogger>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

/*app.Run(async context =>
{
    string file_name = DateTime.Now.ToString("dd.MM.yyyy") + ".log";
    string path = app.Environment.ContentRootPath + app.Configuration.GetSection("Logger").GetValue<string>("Path") + file_name;
    if (!File.Exists(path)) File.Create(path);
    //File.AppendAllText(path, "test");
    await context.Response.WriteAsync(path);
});*/

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
