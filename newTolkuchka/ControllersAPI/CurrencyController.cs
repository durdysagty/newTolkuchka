using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IActionNoFile<Currency, AdminCurrency> _currency;
        public CurrencyController(IEntry entry, IActionNoFile<Currency, AdminCurrency> currency) : base(entry, Entity.Currency, currency)
        {
            _currency = currency;
        }

        [HttpGet("{id}")]
        public async Task<Currency> Get(int id)
        {
            Currency currency = await _currency.GetModelAsync(id);
            return currency;
        }
        //[HttpGet]
        //public IEnumerable<Currency> Get()
        //{
        //    IEnumerable<Currency> currencies = _currency.GetModels();
        //    return currencies;
        //}
        [HttpPost]
        public async Task<Result> Post(Currency currency)
        {
            bool isExist = _currency.IsExist(currency, _currency.GetModels());
            if (isExist)
                return Result.Already;
            await _currency.AddModelAsync(currency);
            await AddActAsync(currency.Id, currency.CodeName);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Currency currency)
        {
            bool isExist = _currency.IsExist(currency, _currency.GetModels().Where(x => x.Id != currency.Id));
            if (isExist)
                return Result.Already;
            _currency.EditModel(currency);
            await EditActAsync(currency.Id, currency.CodeName);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Currency currency = await _currency.GetModelAsync(id);
            if (currency == null)
                return Result.Fail;
            Result result = await _currency.DeleteModelAsync(currency.Id, currency);
            if (result == Result.Success)
                await DeleteActAsync(id, currency.CodeName);
            return result;
        }
    }
}