using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IContent
    {
        Task<Content> GetContent();
        Task EditContent(Content content);
    }
}
