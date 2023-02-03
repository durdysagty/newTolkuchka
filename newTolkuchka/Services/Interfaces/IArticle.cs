using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface IArticle : IActionFormFile<Article, AdminArticle>
    {
        Task<HeadingArticle> GetHeadingArticleAsync(int headingId, int articleId);
        IQueryable<HeadingArticle> GetHeadingArticles();
        Task AddHeadingArticle(HeadingArticle headingArticle);
        void DeleteHeadingArticle(HeadingArticle headingArticle);
    }
}
