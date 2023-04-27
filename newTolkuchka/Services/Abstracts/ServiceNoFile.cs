using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using Type = System.Type;

namespace newTolkuchka.Services.Abstracts
{
    public abstract class ServiceNoFile<T, TAdmin> : Service<T, TAdmin>, IActionNoFile<T, TAdmin> where T : class where TAdmin : class
    {
        public ServiceNoFile(AppDbContext con, IStringLocalizer<Shared> localizer, ICacheClean cacheClean) : base(con, localizer, cacheClean)
        {
        }

        public virtual async Task AddModelAsync(T model, bool save = false)
        {
            Type type = typeof(T);
            int id = GetModelId(type, model);
            if (id != 0)
            {
                type.GetProperty("Id").SetValue(model, 0);
            }
            await _con.Set<T>().AddAsync(model);
            if (save)
                await _con.SaveChangesAsync();
            _cacheClean.CleanAdminModels(type.Name);
        }

        public virtual void EditModel(T model)
        {
            Type type = typeof(T);
            PropertyInfo propertyInfo = type.GetProperty("Version");
            if (propertyInfo != null)
            {
                int value = (int)propertyInfo.GetValue(model);
                propertyInfo.SetValue(model, ++value);
            }
            _con.Entry(model).State = EntityState.Modified;
            _cacheClean.CleanAdminModels(type.Name);
        }

        public async Task<Result> DeleteModelAsync(int id, T model)
        {
            bool isBinded = await IsBinded(id);
            if (isBinded)
                return Result.Fail;
            _con.Set<T>().Remove(model);
            _cacheClean.CleanAdminModels(typeof(T).Name);
            return Result.Success;
        }

        public async Task SaveChangesAsync()
        {
            await _con.SaveChangesAsync();
        }
    }
}
