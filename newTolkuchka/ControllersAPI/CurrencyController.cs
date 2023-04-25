using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level2")]
    public class CurrencyController : AbstractController<Currency, AdminCurrency, IActionNoFile<Currency, AdminCurrency>>
    {
        public CurrencyController(IEntry entry, IActionNoFile<Currency, AdminCurrency> currency, ICacheClean cacheClean) : base(entry, Entity.Currency, currency, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Currency> Get(int id)
        {
            Currency currency = await _service.GetModelAsync(id);
            return currency;
        }
        //[HttpGet]
        //public IEnumerable<Currency> Get()
        //{
        //    IEnumerable<Currency> currencies = _service.GetModels();
        //    return currencies;
        //}
        [HttpPost]
        public async Task<Result> Post(Currency currency)
        {
            bool isExist = _service.IsExist(currency, _service.GetModels());
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(currency);
            await AddActAsync(currency.Id, currency.CodeName);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Currency currency)
        {
            bool isExist = _service.IsExist(currency, _service.GetModels().Where(x => x.Id != currency.Id));
            if (isExist)
                return Result.Already;
            _service.EditModel(currency);
            await EditActAsync(currency.Id, currency.CodeName);
            //to update current static currency property
            if (currency.Id == 2)
                CurrencyService.Currency = _service.GetModels().AsNoTracking().FirstOrDefault(c => c.Id == 2);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Currency currency = await _service.GetModelAsync(id);
            if (currency == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(currency.Id, currency);
            if (result == Result.Success)
                await DeleteActAsync(id, currency.CodeName);
            return result;
        }
    }
}