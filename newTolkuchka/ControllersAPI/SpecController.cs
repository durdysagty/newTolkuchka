using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using System.Text.Json;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class SpecController : AbstractController<Spec, AdminSpec, ISpec>
    {
        public SpecController(IEntry entry, ISpec spec, IMemoryCache memoryCache, ICacheClean cacheClean) : base(entry, Entity.Spec, spec, memoryCache, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Spec> Get(int id)
        {
            Spec spec = await _service.GetModelAsync(id);
            return spec;
        }

        [HttpGet("value")]
        public IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> GetSpecsWithValues([FromQuery] string[] keys, [FromQuery] string[] values)
        {
            Dictionary<string, object> paramsList = null;
            if (keys.Any())
            {
                paramsList = new();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i] == ConstantsService.SPEC)
                        paramsList.Add(keys[i], JsonSerializer.Deserialize<IList<int>>(values[i]));
                    else
                        paramsList.Add(keys[i], values[i]);
                }
            }
            IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> specWithValues = _service.GetSpecWithValues(paramsList);
            return specWithValues;
        }
        [HttpPost]
        public async Task<Result> Post(Spec spec)
        {
            bool isExist = _service.IsExist(spec, _service.GetModels());
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(spec);
            await AddActAsync(spec.Id, spec.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Spec spec)
        {
            bool isExist = _service.IsExist(spec, _service.GetModels().Where(x => x.Id != spec.Id));
            if (isExist)
                return Result.Already;
            _service.EditModel(spec);
            await EditActAsync(spec.Id, spec.NameRu);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Spec spec = await _service.GetModelAsync(id);
            if (spec == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(spec.Id, spec);
            if (result == Result.Success)
                await DeleteActAsync(id, spec.NameRu);
            return result;
        }
        [HttpGet("isimaged/{id}")]
        public async Task<bool> IsSpecImaged(int id)
        {
            bool isImaged = await _service.IsSpecImaged(id);
            return isImaged;
        }
    }
}