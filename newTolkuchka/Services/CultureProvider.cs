using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;

namespace newTolkuchka.Services
{
    public class CultureProvider : RequestCultureProvider
    {
        public static string Lang { get; set; }
        public static string LangState { get; set; }
        public static string Host { get; set; }
        public static string Path { get; set; }
        public static string SiteName { get; set; }
        public static string SiteUrlRu { get; set; }
        public static string SiteUrlEn { get; set; }
        public static string SiteUrlTm { get; set; }

        public static string GetLocalName(string ru, string en, string tk)
        {
            return Lang switch
            {
                ConstantsService.EN => en,
                ConstantsService.TK => tk,
                _ => ru,
            };
        }

        public static int GetLocalNumberEx()
        {
            return Lang switch
            {
                ConstantsService.EN => 1,
                ConstantsService.TK => 2,
                _ => 0
            };
        }
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            StringValues host = httpContext.Request.Headers.Host;
            Host = host[0];
            Path = httpContext.Request.Path;
            if (Host.Contains($"{ConstantsService.EN}."))
            {
                Lang = ConstantsService.EN;
                LangState = ConstantsService.ENST;
                SiteName = host[0].Replace($"{ConstantsService.EN}.", "");
            }
            else if (Host.Contains($"{ConstantsService.TM}."))
            {
                Lang = ConstantsService.TK;
                LangState = ConstantsService.TMST;
                SiteName = host[0].Replace($"{ConstantsService.TM}.", "");
            }
            else
            {
                Lang = ConstantsService.RU;
                LangState = ConstantsService.RUST;
                SiteName = host[0];
            }
            SiteUrlRu = $"https://{SiteName}";
            SiteUrlEn = $"https://en.{SiteName}";
            SiteUrlTm = $"https://tm.{SiteName}";
            ProviderCultureResult result = new(Lang);
            return Task.FromResult(result);
        }
    }
}
