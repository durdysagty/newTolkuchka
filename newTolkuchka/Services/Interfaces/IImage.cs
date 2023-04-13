namespace newTolkuchka.Services.Interfaces
{
    public interface IImage
    {
        Task SetImage(string path, IFormFile image, int w, int h);
        void DeleteImages(Stack<string> paths);

        public static string GetImageHtml(string source, int width, int height, string styleWidth, string styleHeight, string alt, string classes = null, string maxWidth = null, string functions = null, string id = null, string title = null)
        {
            return $"<picture><source {(id == null ? null : $"id=\"{$"{id}webp"}\"")} type=\"image/webp\" srcset=\"{$"{source}{ConstantsService.WEBP}"}\"><source {(id == null ? null : $"id=\"{$"{id}jpg"}\"")} type=\"image/jpeg\" srcset=\"{$"{source}{ConstantsService.JPG}"}\"><img src=\"{$"{source}{ConstantsService.JPG}"}\" width=\"{(width > 0 ? width : "auto")}\" height=\"{(height > 0 ? height : "auto")}\" style=\"width: {styleWidth}; height: {styleHeight}{(maxWidth == null ? null : $"; max-width: {maxWidth}")} \" alt=\"{alt}\" {(classes == null ? null : $"class=\"{classes}\"")} {(functions == null ? null : $"{functions}")} {(id == null ? null : $"id=\"{id}\"")} {(title == null ? null : $"title=\"{title}\"")} ></picture>";
        }
    }
}
