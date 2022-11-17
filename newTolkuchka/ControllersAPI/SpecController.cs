using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class SpecController : AbstractController
    {
        private readonly ISpec _spec;
        public SpecController(IEntry entry, ISpec spec) : base(entry, Entity.Spec)
        {
            _spec = spec;
        }

        [HttpGet("{id}")]
        public async Task<Spec> Get(int id)
        {
            Spec spec = await _spec.GetModelAsync(id);
            return spec;
        }
        [HttpGet]
        public IEnumerable<AdminSpec> Get()
        {
            IEnumerable<AdminSpec> specs = _spec.GetAdminSpecs();
            return specs;
        }
        [HttpGet("value")]
        public IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> GetSpecsWithValues(int modelId = 0)
        {
            IEnumerable<ModelWithList<ModelWithList<AdminSpecsValueMod>>> specWithValues = _spec.GetSpecWithValues(modelId);
            return specWithValues;
        }
        [HttpPost]
        public async Task<Result> Post(Spec spec)
        {
            bool isExist = _spec.IsExist(spec, _spec.GetModels());
            if (isExist)
                return Result.Already;
            await _spec.AddModelAsync(spec);
            await AddActAsync(spec.Id, spec.NameRu);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Spec spec)
        {
            bool isExist = _spec.IsExist(spec, _spec.GetModels().Where(x => x.Id != spec.Id));
            if (isExist)
                return Result.Already;
            _spec.EditModel(spec);
            await EditActAsync(spec.Id, spec.NameRu);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Spec spec = await _spec.GetModelAsync(id);
            if (spec == null)
                return Result.Fail;
            Result result = await _spec.DeleteModelAsync(spec.Id, spec);
            if (result == Result.Success)
                await DeleteActAsync(id, spec.NameRu);
            return result;
        }
        [HttpGet("isimaged/{id}")]
        public async Task<bool> IsSpecImaged(int id)
        {
            bool isImaged = await _spec.IsSpecImaged(id);
            return isImaged;
        }
    }
}