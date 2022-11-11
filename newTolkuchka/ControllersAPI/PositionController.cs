using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level3")]
    public class PositionController : AbstractController
    {
        private readonly IPosition _position;
        public PositionController(IEntry entry, IPosition position): base(entry, Entity.Position)
        {
            _position = position;
        }

        [HttpGet("{id}")]
        public async Task<Position> Get(int id)
        {
            Position position = await _position.GetModelAsync(id);
            return position;
        }
        [HttpGet]
        public IEnumerable<AdminPosition> Get()
        {
            IEnumerable<AdminPosition> positions = _position.GetAdminPositions();
            return positions;
        }
        [HttpPost]
        public async Task<Result> Post(Position position)
        {
            bool isExist = _position.IsExist(position, _position.GetModels());
            if (isExist)
                return Result.Already;
            await _position.AddModelAsync(position); // no further action required
            await AddActAsync(position.Id, position.Name);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put(Position position)
        {
            bool isExist = _position.IsExist(position, _position.GetModels().Where(x => x.Id != position.Id));
            if (isExist)
                return Result.Already;
            _position.EditPosition(position);
            await EditActAsync(position.Id, position.Name);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Position position = await _position.GetModelAsync(id);
            if (position == null)
                return Result.Fail;
            Result result = await _position.DeleteModel(position.Id, position);
            if (result == Result.Success)
                await DeleteActAsync(id, position.Name);
            return result;
        }
    }
}