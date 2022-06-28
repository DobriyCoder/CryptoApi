using CryptoApi.Sitemap;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApi.Controllers;
public class SitemapController : Controller
{

    [Route("/sitemap.xml")]
    public string? Index([FromServices] ISitemap sitemapModel)
    {
        string? result = sitemapModel.GetMainSitemap();
        if (result == null) return "ERROR"/*throw new HttpException(404, "Page you requested is not found")*/;

        return result;
    }

    [Route("/sitemap-{index:int}.xml")]
    public string? Page([FromServices] ISitemap sitemapModel, int index) 
    {
        string? result = sitemapModel.GetSubSitemap(index);
        if (result == null) return "ERROR"/*throw new HttpException(404, "Page you requested is not found")*/;

        return result;
    }
}
