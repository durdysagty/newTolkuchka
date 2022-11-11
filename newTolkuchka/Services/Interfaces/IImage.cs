namespace newTolkuchka.Services.Interfaces
{
    public interface IImage
    {
        Task SetImage(string path, IFormFile image, int w, int h);
        void DeleteImages(Stack<string> paths);
    }
}
