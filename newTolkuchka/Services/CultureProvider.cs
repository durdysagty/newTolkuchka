using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;

namespace newTolkuchka.Services
{
    public class CultureProvider : RequestCultureProvider
    {
        public enum Culture { Ru, En, Tm }
        public static string Lang { get; set; }
        public static string LangState { get; set; }
        public static string Host { get; set; }
        public static string Path { get; set; }
        public static string SiteName { get; set; }
        public static string SiteUrlRu { get; set; }
        public static string SiteUrlEn { get; set; }
        public static string SiteUrlTm { get; set; }
        public static Culture CurrentCulture { get; set; }

        public static string GetLocalName(string ru, string en, string tk)
        {
            return CurrentCulture switch
            {
                Culture.En => en,
                Culture.Tm => tk,
                _ => ru,
            };
        }

        public static int GetLocalNumberEx()
        {
            return (int)CurrentCulture;
        }

        // we use this method in IProduct, we can't use localizer there, becouse of static method
        public static string GetPromotionsLocal()
        {
            return Lang switch
            {
                ConstantsService.EN => "Promotions",
                ConstantsService.TK => "Aksiýalar",
                _ => "Акции",
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
                CurrentCulture = Culture.En;
                SiteName = host[0].Replace($"{ConstantsService.EN}.", "");
            }
            else if (Host.Contains($"{ConstantsService.TM}."))
            {
                Lang = ConstantsService.TK;
                LangState = ConstantsService.TMST;
                CurrentCulture = Culture.Tm;
                SiteName = host[0].Replace($"{ConstantsService.TM}.", "");
            }
            else
            {
                Lang = ConstantsService.RU;
                LangState = ConstantsService.RUST;
                CurrentCulture = Culture.Ru;
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
