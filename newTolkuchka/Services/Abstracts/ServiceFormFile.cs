using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;
using Type = System.Type;

namespace newTolkuchka.Services.Abstracts
{
    public abstract class ServiceFormFile<T, TAdmin> : Service<T, TAdmin>, IActionFormFile<T, TAdmin> where T : class where TAdmin : class
    {
        private protected readonly IPath _path;
        private protected readonly IImage _image;
        private protected readonly int _imagesMax;

        public ServiceFormFile(AppDbContext con, IStringLocalizer<Shared> localizer, IPath path, IImage image, int imagesMax) : base(con, localizer)
        {
            _path = path;
            _image = image;
            _imagesMax = imagesMax;
        }

        public async Task AddModelAsync(T model, IFormFile[] images, int width, int height, int? divider = null)
        {
            int? simId = null;
            Type type = typeof(T);
            int id = GetModelId(type, model);
            if (id != 0)
            {
                simId = id;
                type.GetProperty("Id").SetValue(model, 0);
            }
            await _con.Set<T>().AddAsync(model);
            await _con.SaveChangesAsync();
            if (images.Any(i => i.Length > 0))
                await SetModelImages(model, images, width, height, divider);
            else
            {
                // if no files are downloaded, then copy the files of sponsor model
                string[] files = Directory.GetFiles($"{_path.GetImagesFolder()}/{type.Name.ToLower()}", $"{simId}-*", SearchOption.AllDirectories);
                id = GetModelId(type, model);
                int n = 0;
                int s = 0;
                if (files.Any())
                    foreach (string f in files)
                    {
                        if (f.Contains("small"))
                        {
                            File.Copy(f, _path.GetImagePath($"{type.Name}/small", id, s));
                            s++;
                        }
                        else
                        {
                            File.Copy(f, _path.GetImagePath(type.Name, id, n));
                            n++;
                        }
                    }
            }
        }

        public async Task EditModelAsync(T model, IFormFile[] images, int width, int height, int? divider = null)
        {
            _con.Entry(model).State = EntityState.Modified;
            if (images.Any(i => i.Length > 0) || images.Any(i => i.FileName == "delete"))
                await SetModelImages(model, images, width, height, divider);
        }

        public async Task SetModelImages(T model, IFormFile[] images, int width, int height, int? divider = null)
        {
            Type type = typeof(T);
            int id = GetModelId(type, model);
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].Length > 0)
                {
                    await _image.SetImage(_path.GetImagePath(type.Name, id, i), images[i], width, height);
                    if (divider != null)
                        await _image.SetImage(_path.GetImagePath($"{type.Name}/small", id, i), images[i], width / (int)divider, height / (int)divider);
                }
                if (images[i].FileName == "delete")
                {
                    _image.DeleteImages(new Stack<string>(new string[] { _path.GetImagePath(type.Name, id, i) }));
                    if (divider != null)
                        _image.DeleteImages(new Stack<string>(new string[] { _path.GetImagePath($"{type.Name}/small", id, i) }));
                }
            }
        }

        public async Task<Result> DeleteModelAsync(int id, T model, bool smallImage)
        {
            bool isBinded = await IsBinded(id);
            if (isBinded)
                return Result.Fail;
            _con.Set<T>().Remove(model);
            Type type = typeof(T);
            Stack<string> paths = new(smallImage ? _imagesMax * 2 : _imagesMax);
            for (int i = 0; i < _imagesMax; i++)
            {
                paths.Push(_path.GetImagePath(type.Name, id, i));
                if (smallImage)
                    paths.Push(_path.GetImagePath($"{type.Name}/small", id, i));
            }
            _image.DeleteImages(paths);
            return Result.Success;
        }
        public async Task SaveChangesAsync()
        {
            await _con.SaveChangesAsync();
        }
    }
}
