namespace newTolkuchka.Services.Interfaces
{
    public interface IImage
    {
        Task SetImage(string path, IFormFile image, int w, int h);
        void DeleteImages(Stack<string> paths);

        public static string GetImageHtml(string source, int version, int width, int height, string styleWidth, string styleHeight, string alt, string classes = null, string functions = null, string id = null, string title = null)
        {
            return $"<picture><source {(id == null ? null : $"id=\"{$"{id}{ConstantsService.WEBP}"}\"")} type=\"{ConstantsService.WEBPTYPE}\" srcset=\"{$"{source}{ConstantsService.WEBP}?v={version}"}\"><source {(id == null ? null : $"id=\"{$"{id}{ConstantsService.JPG}"}\"")} type=\"{ConstantsService.JPGTYPE}\" srcset=\"{$"{source}{ConstantsService.JPG}?v={version}"}\"><img src=\"{$"{source}{ConstantsService.JPG}?v={version}"}\" {(height > 0 ? $"width=\"{width}\"" : null)} {(height > 0 ? $"height=\"{height}\"" : null)} style=\"max-width: {styleWidth}; height: {styleHeight}; {(width == 0 ? "width: auto": null)}\" alt=\"{alt}\" {(classes == null ? null : $"class=\"{classes}\"")} {(functions == null ? null : $"{functions}")} {(id == null ? null : $"id=\"{id}\"")} {(title == null ? null : $"title=\"{title}\"")} ></picture>";
        }
    }
}
