namespace newTolkuchka.Services.Interfaces
{
    public interface IActionNoFile<T>: IAction<T>
    {
        Task AddModelAsync(T model, bool save = true);
        void EditModel(T model);
        Task<Result> DeleteModel(int id, T model);
        Task SaveChangesAsync();
    }
}
