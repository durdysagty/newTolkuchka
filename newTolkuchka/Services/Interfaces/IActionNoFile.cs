namespace newTolkuchka.Services.Interfaces
{
    public interface IActionNoFile<T, TAdmin> : IAction<T, TAdmin>
    {
        Task AddModelAsync(T model, bool save = true);
        void EditModel(T model);
        Task<Result> DeleteModelAsync(int id, T model);
        Task SaveChangesAsync();
    }
}
