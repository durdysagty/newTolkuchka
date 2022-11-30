using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services.Abstracts
{
    public abstract class ServiceNoFile<T> : Service<T>, IActionNoFile<T> where T : class
    {
        public ServiceNoFile(AppDbContext con, IStringLocalizer<Shared> localizer) : base(con, localizer)
        {
        }

        public virtual async Task AddModelAsync(T model, bool save = false)
        {
            await _con.Set<T>().AddAsync(model);
            if (save)
                await _con.SaveChangesAsync();
        }

        public virtual void EditModel(T model)
        {
            _con.Entry(model).State = EntityState.Modified;
        }

        public async Task<Result> DeleteModelAsync(int id, T model)
        {
            bool isBinded = await IsBinded(id);
            if (isBinded)
                return Result.Fail;
            _con.Set<T>().Remove(model);
            return Result.Success;
        }

        public async Task SaveChangesAsync()
        {
            await _con.SaveChangesAsync();
        }
    }
}
