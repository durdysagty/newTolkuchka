using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class LineController : AbstractController
    {
        private readonly ILine _line;
        public LineController(IEntry entry, ILine line) : base(entry, Entity.Line)
        {
            _line = line;
        }

        [HttpGet("{id}")]
        public async Task<Line> Get(int id)
        {
            Line line = await _line.GetModelAsync(id);
            return line;
        }
        [HttpGet]
        public IEnumerable<AdminLine> Get([FromQuery] int[] brandId)
        {
            IEnumerable<AdminLine> lines = _line.GetAdminLines(brandId);
            return lines;
        }
        [HttpPost]
        public async Task<Result> Post(Line line)
        {
            bool isExist = _line.IsExist(line, _line.GetModels());
            if (isExist)
                return Result.Already;
            await _line.AddModelAsync(line);
            await AddActAsync(line.Id, line.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Line line)
        {
            bool isExist = _line.IsExist(line, _line.GetModels().Where(x => x.Id != line.Id));
            if (isExist)
                return Result.Already;
            _line.EditModel(line);
            await EditActAsync(line.Id, line.Name);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Line line = await _line.GetModelAsync(id);
            if (line == null)
                return Result.Fail;
            Result result = await _line.DeleteModelAsync(line.Id, line);
            if (result == Result.Success)
                await DeleteActAsync(id, line.Name);
            return result;
        }
    }
}