using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using File = System.IO.File;
using IImage = newTolkuchka.Services.Interfaces.IImage;

namespace newTolkuchka.Services
{
    public class ImageService : IImage
    {

        public async Task SetImage(string path, IFormFile image, int w, int h)
        {
            Image file = await Image.LoadAsync(image.OpenReadStream());
            if (w == 0 && h == 0)
            {
                file.Mutate(m => m.Resize(0, 300));
            }
            else
            {
                bool height = (double)file.Width / w < (double)file.Height / h;
                file.Mutate(m => m.Resize(height ? 0 : w, height ? h : 0).BackgroundColor(Color.White).Pad(w, h, Color.White));
            }
            await file.SaveAsJpegAsync($"{path}{ConstantsService.JPG}");
            await file.SaveAsWebpAsync($"{path}{ConstantsService.WEBP}", new WebpEncoder { Method = WebpEncodingMethod.BestQuality, NearLossless = true, Quality = 80 });
            file.Dispose();
        }

        public void DeleteImages(Stack<string> paths)
        {
            while (paths.Count > 0)
                File.Delete(paths.Pop());
        }
    }
}
