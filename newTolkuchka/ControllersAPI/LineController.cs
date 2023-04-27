using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class LineController : AbstractController<Line, AdminLine, ILine>
    {
        public LineController(IEntry entry, ILine line, IMemoryCache memoryCache, ICacheClean cacheClean) : base(entry, Entity.Line, line, memoryCache, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Line> Get(int id)
        {
            Line line = await _service.GetModelAsync(id);
            return line;
        }
        //[HttpGet]
        //public IEnumerable<AdminLine> Get([FromQuery] int[] brandId)
        //{
        //    IEnumerable<AdminLine> lines = _service.GetAdminLines(brandId);
        //    return lines;
        //}
        [HttpPost]
        public async Task<Result> Post(Line line)
        {
            bool isExist = _service.IsExist(line, _service.GetModels(new Dictionary<string, object>() { { ConstantsService.BRAND, line.BrandId } }));
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(line);
            await AddActAsync(line.Id, line.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Line line)
        {
            bool isExist = _service.IsExist(line, _service.GetModels(new Dictionary<string, object>() { { ConstantsService.BRAND, line.BrandId } }).Where(x => x.Id != line.Id));
            if (isExist)
                return Result.Already;
            _service.EditModel(line);
            await EditActAsync(line.Id, line.Name);
            _cacheClean.CleanProductPage();
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Line line = await _service.GetModelAsync(id);
            if (line == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(line.Id, line);
            if (result == Result.Success)
                await DeleteActAsync(id, line.Name);
            return result;
        }
    }
}