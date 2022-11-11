﻿using SixLabors.ImageSharp;
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
            bool height = (double)file.Width / w < (double)file.Height / h;
            file.Mutate(m => m.Resize(height ? 0 : w, height ? h : 0).BackgroundColor(Color.White).Pad(w, h, Color.White));
            await file.SaveAsJpegAsync(path);
            file.Dispose();
        }

        public void DeleteImages(Stack<string> paths)
        {
            while (paths.Count > 0)
                File.Delete(paths.Pop());
        }
    }
}
