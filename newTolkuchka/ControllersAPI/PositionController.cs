using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level3")]
    public class PositionController : AbstractController<Position, AdminPosition, IPosition>
    {
        public PositionController(IEntry entry, IPosition position, IMemoryCache memoryCache, ICacheClean cacheClean) : base(entry, Entity.Position, position, memoryCache, cacheClean)
        {
        }

        [HttpGet("{id}")]
        public async Task<Position> Get(int id)
        {
            Position position = await _service.GetModelAsync(id);
            return position;
        }
        //[HttpGet]
        //public IEnumerable<AdminPosition> Get()
        //{
        //    IEnumerable<AdminPosition> positions = _service.GetAdminPositions();
        //    return positions;
        //}
        [HttpPost]
        public async Task<Result> Post(Position position)
        {
            bool isExist = _service.IsExist(position, _service.GetModels());
            if (isExist)
                return Result.Already;
            await _service.AddModelAsync(position); // no further action required
            await AddActAsync(position.Id, position.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Position position)
        {
            bool isExist = _service.IsExist(position, _service.GetModels().Where(x => x.Id != position.Id));
            if (isExist)
                return Result.Already;
            _service.EditPosition(position);
            await EditActAsync(position.Id, position.Name);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Position position = await _service.GetModelAsync(id);
            if (position == null)
                return Result.Fail;
            Result result = await _service.DeleteModelAsync(position.Id, position);
            if (result == Result.Success)
                await DeleteActAsync(id, position.Name);
            return result;
        }
    }
}