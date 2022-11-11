namespace newTolkuchka.Services.Interfaces
{
    public interface IActionFormFile<T>: IAction<T>
    {
        Task AddModelAsync(T model, IFormFile[] images, int width, int height, int? devider = null);
        Task EditModelAsync(T model, IFormFile[] images, int width, int height, int? devider = null);
        Task SetModelImages(T model, IFormFile[] images, int width, int height, int? divider = null);
        Task<Result> DeleteModel(int id, T model, bool smallImage = false);
    }
}
