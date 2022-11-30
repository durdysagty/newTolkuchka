using newTolkuchka.Models.DTO;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class ContentService : IContent
    {

        private readonly IPath _path;
        public ContentService(IPath path)
        {
            _path = path;
        }

        public async Task<Content> GetContent()
        {
            Content content = new()
            {
                AboutRu = await File.ReadAllTextAsync(_path.GetHtmlAboutBodyPath(ConstantsService.RU)),
                AboutEn = await File.ReadAllTextAsync(_path.GetHtmlAboutBodyPath(ConstantsService.EN)),
                AboutTk = await File.ReadAllTextAsync(_path.GetHtmlAboutBodyPath(ConstantsService.TK)),
                DeliveryRu = await File.ReadAllTextAsync(_path.GetHtmlDeliveryBodyPath(ConstantsService.RU)),
                DeliveryEn = await File.ReadAllTextAsync(_path.GetHtmlDeliveryBodyPath(ConstantsService.EN)),
                DeliveryTk = await File.ReadAllTextAsync(_path.GetHtmlDeliveryBodyPath(ConstantsService.TK))
            };
            return content;
        }

        public async Task EditContent(Content content)
        {
            await File.WriteAllTextAsync(_path.GetHtmlAboutBodyPath(ConstantsService.EN), content.AboutEn);
            await File.WriteAllTextAsync(_path.GetHtmlAboutBodyPath(ConstantsService.RU), content.AboutRu);
            await File.WriteAllTextAsync(_path.GetHtmlAboutBodyPath(ConstantsService.TK), content.AboutTk);
            await File.WriteAllTextAsync(_path.GetHtmlDeliveryBodyPath(ConstantsService.EN), content.DeliveryEn);
            await File.WriteAllTextAsync(_path.GetHtmlDeliveryBodyPath(ConstantsService.RU), content.DeliveryRu);
            await File.WriteAllTextAsync(_path.GetHtmlDeliveryBodyPath(ConstantsService.TK), content.DeliveryTk);
        }
    }
}