namespace newTolkuchka.Services.Interfaces
{
    public interface IImage
    {
        Task SetImage(string path, IFormFile image, int w, int h);
        void DeleteImages(Stack<string> paths);

        public static string GetImageHtml(string source, int width, int height, string styleWidth, string styleHeight, string alt, string classes = null, string functions = null, string id = null, string title = null)
        {
            return $"<picture><source {(id == null ? null : $"id=\"{$"{id}webp"}\"")} type=\"image/webp\" srcset=\"{$"{source}{ConstantsService.WEBP}"}\"><source {(id == null ? null : $"id=\"{$"{id}jpg"}\"")} type=\"image/jpeg\" srcset=\"{$"{source}{ConstantsService.JPG}"}\"><img src=\"{$"{source}{ConstantsService.JPG}"}\" {(height > 0 ? $"width=\"{width}\"" : null)} {(height > 0 ? $"height=\"{height}\"" : null)} style=\"max-width: {styleWidth}; height: {styleHeight}; {(width == 0 ? "width: auto": null)}\" alt=\"{alt}\" {(classes == null ? null : $"class=\"{classes}\"")} {(functions == null ? null : $"{functions}")} {(id == null ? null : $"id=\"{id}\"")} {(title == null ? null : $"title=\"{title}\"")} ></picture>";
        }
    }
}
