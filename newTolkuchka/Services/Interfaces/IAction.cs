namespace newTolkuchka.Services.Interfaces
{
    public enum Result { Success, Fail, Already, MaxLength, NoConnections, DeleteError, NoImage }
    public interface IAction<T, TAdmin>
    {        
        Task<T> GetModelAsync(int id);
        IQueryable<T> GetModels(Dictionary<string, object> paramsList = null);
        IQueryable<T> GetFullModels(Dictionary<string, object> paramsList = null);
        IList<TAdmin> GetAdminModels(int page, int pp, out int lastPage, out string pagination, Dictionary<string, object> paramsList = null);
        bool IsExist(T model, IEnumerable<T> list);
        Task<bool> IsBinded(int id);
        string GetPagination(int pp, int total, int pageCount, int toSkip, out int lastPage);
    }
}
