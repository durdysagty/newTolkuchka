using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services.Abstracts
{
    [ApiController, Route("api/[controller]")]
    public abstract class AbstractController : ControllerBase
    {
        private protected readonly IEntry _entry;
        private readonly Entity _entity;
        public AbstractController(IEntry entry, Entity entity)
        {
            _entry = entry;
            _entity = entity;
        }

        private protected async Task AddActAsync(int entityId, string entityName)
        {
            await _entry.AddEntryAsync(Act.Add, _entity, entityId, entityName);
        }
        private protected async Task EditActAsync(int entityId, string entityName)
        {
            await _entry.AddEntryAsync(Act.Edit, _entity, entityId, entityName);
        }
        private protected async Task DeleteActAsync(int entityId, string entityName)
        {
            await _entry.AddEntryAsync(Act.Delete, _entity, entityId, entityName);
        }
    }
}
