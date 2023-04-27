using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Reces;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;

namespace newTolkuchka.Services
{
    public class ArticleService : ServiceFormFile<Article, AdminArticle>, IArticle
    {
        public ArticleService(AppDbContext con, IStringLocalizer<Shared> localizer, IPath path, ICacheClean cacheClean, IImage image) : base(con, localizer, path, cacheClean, image, ConstantsService.UMAXIMAGE)
        {
        }

        public async Task<HeadingArticle> GetHeadingArticleAsync(int headingId, int articleId)
        {
            return await _con.HeadingArticles.FindAsync(headingId, articleId);
        }
        public IQueryable<HeadingArticle> GetHeadingArticles()
        {
            return _con.HeadingArticles;
        }
        public async Task AddHeadingArticle(HeadingArticle headingArticle)
        {
            await _con.HeadingArticles.AddAsync(headingArticle);
        }

        public void DeleteHeadingArticle(HeadingArticle headingArticle)
        {
            _con.HeadingArticles.Remove(headingArticle);
        }
    }
}
