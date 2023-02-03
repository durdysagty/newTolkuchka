using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class BreadcrumbsService : IBreadcrumbs
    {
        private readonly IStringLocalizer<Shared> _localizer;
        private readonly ICategory _category;
        private readonly List<Breadcrumb> _breadcrumbs;
        public BreadcrumbsService(IStringLocalizer<Shared> localizer, ICategory category)
        {
            _localizer = localizer;
            _category = category;
            _breadcrumbs = new()
            {
                new Breadcrumb {
                    Name = _localizer["main"],
                    Link = ConstantsService.HOME
                }
            };
        }
        public async Task<IList<Breadcrumb>> GetCategoryBreadcrumbsAsync(int parentId)
        {
            _breadcrumbs.Add(
                new Breadcrumb
                {
                    Name = _localizer[ConstantsService.CATEGORIES],
                    Link = ConstantsService.CATEGORIES
                });
            async Task GetAllPathBreadcrumbs(int parentId)
            {
                if (parentId > 0)
                {
                    Category category = await _category.GetModelAsync(parentId);
                    await GetAllPathBreadcrumbs(category.ParentId);
                    _breadcrumbs.Add(
                        new Breadcrumb
                        {
                            Name = CultureProvider.GetLocalName(category.NameRu, category.NameEn, category.NameTm),
                            Link = $"{ConstantsService.CATEGORY}/{category.Id}"
                        });
                }
            }
            await GetAllPathBreadcrumbs(parentId);
            return _breadcrumbs;
        }
        public IList<Breadcrumb> GetBrandBreadcrumbs()
        {
            _breadcrumbs.Add(
                new Breadcrumb
                {
                    Name = _localizer[ConstantsService.BRANDS],
                    Link = ConstantsService.BRANDS
                });
            return _breadcrumbs;
        }

        public async Task<IList<Breadcrumb>> GetProductBreadcrumbs(int parentId, string name, bool isBrand)
        {
            if (isBrand)
                _breadcrumbs.Add(
                    new Breadcrumb
                    {
                        Name = name,
                        Link = $"{ConstantsService.BRAND}/{parentId}"
                    });
            else
                _ = await GetCategoryBreadcrumbsAsync(parentId);
            return _breadcrumbs;
        }

        public IList<Breadcrumb> GetArticleBreadcrumbs()
        {
            _breadcrumbs.Add(
                new Breadcrumb
                {
                    Name = _localizer[ConstantsService.ARTICLES],
                    Link = ConstantsService.ARTICLES
                });
            return _breadcrumbs;
        }

        public IList<Breadcrumb> GetBreadcrumbs()
        {
            return _breadcrumbs;
        }
    }
}
