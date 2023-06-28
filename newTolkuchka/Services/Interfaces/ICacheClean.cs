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
        void CleanIndexArticles();
        void CleanAllModeledProducts();
        void CleanProductPage(int id = 0);
        void CleanAllReports();
        void CleanAdminModels(string model);
    }
}
