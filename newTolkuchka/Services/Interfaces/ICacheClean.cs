using newTolkuchka.Models;

namespace newTolkuchka.Services.Interfaces
{
    public interface ICacheClean
    {
        void CleanBrands();
        void CleanCategories(int id);
        void CleanIndexCategoriesPromotions();
        void CleanCategory(int id);
        void CleanIndexItems();
        void CleanSlides();
        void CleanAllModeledProducts();
        void CleanIndexArticles();
    }
}
