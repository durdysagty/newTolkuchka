using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class PathService : IPath
    {
        private readonly IWebHostEnvironment _environment;

        public PathService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string GetImagesFolder()
        {
            return Path.Combine(_environment.ContentRootPath, "wwwroot/images");
        }
        public string GetImagePath(string folder, int id, int imageNumber = 0)
        {
            return Path.Combine(GetImagesFolder(), folder.ToLower(), $"{id}-{imageNumber}.jpg");
        }
        public string GetHtmlBodyPath()
        {
            return Path.Combine(_environment.ContentRootPath, "wwwroot/html");
        }
        public string GetHtmlPinBodyPath()
        {
            return Path.Combine(GetHtmlBodyPath(), "pin.html");
        }
        public string GetHtmlRecoveryBodyPath()
        {
            return Path.Combine(GetHtmlBodyPath(), "recovery.html");
        }
        public string GetHtmlNewPinBodyPath()
        {
            return Path.Combine(GetHtmlBodyPath(), "newpin.html");
        }
        public string GetHtmlAboutBodyPath(string lang)
        {
            return lang switch
            {
                ConstantsService.EN => Path.Combine(GetHtmlBodyPath(), "abouten.html"),
                ConstantsService.TK => Path.Combine(GetHtmlBodyPath(), "abouttk.html"),
                _ => Path.Combine(GetHtmlBodyPath(), "aboutru.html")
            };
        }
        public string GetHtmlDeliveryBodyPath(string lang)
        {
            return lang switch
            {
                ConstantsService.EN => Path.Combine(GetHtmlBodyPath(), "deliveryen.html"),
                ConstantsService.TK => Path.Combine(GetHtmlBodyPath(), "deliverytk.html"),
                _ => Path.Combine(GetHtmlBodyPath(), "deliveryru.html")
            };
        }
        public string GetLogo()
        {
            return $"{CultureProvider.SiteUrlRu}/logo.png";
        }
        // statics
        public static string GetImageRelativePath(string folder, int id, int imageNumber = 0)
        {
            return Path.Combine($"/images/{folder.ToLower()}/{id}-{imageNumber}.jpg");
        }
        public static string GetSVGRelativePath(string folder, string id)
        {
            if (folder != null)
                return Path.Combine($"/svgs/{folder.ToLower()}/{id}.svg");
            else
                return Path.Combine($"/svgs/{id}.svg");
        }
        public static string GetModelUrl(string modelName, int id)
        {
            return $"{modelName}/{id}";
        }
    }
}
