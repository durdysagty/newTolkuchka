namespace newTolkuchka.Services.Interfaces
{
    public enum Result { Success, Fail, Already }
    public interface IAction<T>
    {        
        Task<T> GetModelAsync(int id);
        IQueryable<T> GetModels();
        bool IsExist(T model, IEnumerable<T> list);
        Task<bool> IsBinded(int id);
        Type GetModelType();
        int GetModelId(Type type, T model);
    }
}
