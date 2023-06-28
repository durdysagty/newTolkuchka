using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class CacheCleanService : ICacheClean
    {
        private readonly IMemoryCache _memoryCache;

        public CacheCleanService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void CleanBrands()
        {
            _memoryCache.Remove(ConstantsService.BRANDS);
            _memoryCache.Remove(ConstantsService.HOMEBRANDS);
            _memoryCache.Remove(ConstantsService.HOMEAPPBRANDS);
        }
        public void CleanCategories(int id)
        {
            _memoryCache.Remove(ConstantsService.CATEGORIES);
            _memoryCache.Remove($"{ConstantsService.CATEGORIESGROUPBYPARENTID}{id}");
        }

        public void CleanIndexCategoriesPromotions()
        {
            foreach (string fontSize in ConstantsService.INDEXCATFONTSIZES)
            {
                CleanCulturedCaches($"{ConstantsService.INDEXCATS}{fontSize}");
            }
        }

        public void CleanCategory(int id)
        {
            _memoryCache.Remove($"{ConstantsService.CATEGORY}{id}");
        }

        public void CleanIndexItems()
        {
            int count = 6;
            // 3 b.o. index pages products count never less than 3 and not more than 6
            for (int i = 3; i <= count; i++)
            {
                CleanCulturedCaches($"{ConstantsService.INDEXITEMS}{ConstantsService.PCW}{count}");
                CleanCulturedCaches($"{ConstantsService.INDEXITEMS}{ConstantsService.PHONEW}{count}");
            }
        }

        public void CleanSlides()
        {
            int slidesCount = 3;
            for (int i = 1; i <= slidesCount; i++)
            {
                CleanCulturedCaches($"{ConstantsService.MAINSLIDES}{slidesCount}");
            }
        }

        public void CleanIndexArticles()
        {
            int count = 6;
            // 3 b.o. index pages articles count never less than 3 and not more than 6
            for (int i = 3; i <= count; i++)
            {
                CleanCulturedCaches($"{ConstantsService.HOMEARTICLES}{count}");
            }
        }

        public void CleanAllModeledProducts()
        {
            if (_memoryCache.TryGetValue(ConstantsService.MODELEDPRODUCTSHASHKEYS, out HashSet<string> modeledProductsKeys))
            {
                foreach (string key in modeledProductsKeys)
                {
                    _memoryCache.Remove(key);
                }
                _memoryCache.Remove(ConstantsService.MODELEDPRODUCTSHASHKEYS);
            }
        }

        public void CleanProductPage(int id)
        {
            if (_memoryCache.TryGetValue(ConstantsService.PRODUCTSHASHKEYS, out HashSet<string> productsKeys))
            {
                if (id == 0)
                {
                    foreach (string key in productsKeys)
                        _memoryCache.Remove(key);
                    _memoryCache.Remove(ConstantsService.PRODUCTSHASHKEYS);
                }
                else
                {
                    IEnumerable<string> neededKeys = productsKeys.Where(pk => pk.Contains($"-{id}-"));
                    foreach (string key in neededKeys)
                    {
                        _memoryCache.Remove(key);
                        productsKeys.Remove(key);
                    }
                    _memoryCache.Set(ConstantsService.PRODUCTSHASHKEYS, productsKeys, new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromDays(3)
                    });
                }
            }
        }

        public void CleanAllReports()
        {
            if (_memoryCache.TryGetValue(ConstantsService.ADMINREPORTSHASHKEYS, out HashSet<string> reportKeys))
            {
                foreach (string key in reportKeys)
                {
                    _memoryCache.Remove(key);
                }
                _memoryCache.Remove(ConstantsService.ADMINREPORTSHASHKEYS);
            }
        }

        public void CleanAdminModels(string model)
        {
            if (model.Equals(ConstantsService.SLIDE, StringComparison.CurrentCultureIgnoreCase))
                CleanSlides();
            if (_memoryCache.TryGetValue(ConstantsService.ADMINMODELSHASHKEYS, out HashSet<string> modelKeys))
            {
                IEnumerable<string> neededKeys = modelKeys.Where(mk => mk.Contains(model));
                foreach (string key in neededKeys)
                {
                    _memoryCache.Remove(key);
                    modelKeys.Remove(key);
                }
                _memoryCache.Set(ConstantsService.ADMINMODELSHASHKEYS, modelKeys, new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.NeverRemove,
                    SlidingExpiration = TimeSpan.FromDays(3)
                });
            }
        }

        private void CleanCulturedCaches(string key)
        {
            Array cultures = Enum.GetValues(typeof(CultureProvider.Culture));
            foreach (var a in cultures)
            {
                _memoryCache.Remove($"{a}{key}");
            }
        }
    }
}
