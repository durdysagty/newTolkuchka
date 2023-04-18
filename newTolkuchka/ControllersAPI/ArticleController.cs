using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newTolkuchka.Models;
using newTolkuchka.Models.DTO;
using newTolkuchka.Services;
using newTolkuchka.Services.Abstracts;
using newTolkuchka.Services.Interfaces;
using System.Collections.Generic;
using System.Text.Json;
using static newTolkuchka.Services.CultureProvider;

namespace newTolkuchka.ControllersAPI
{
    [Authorize(Policy = "Level1")]
    public class ArticleController : AbstractController<Article, AdminArticle, IArticle>
    {
        private const int WIDTH = 0;
        private const int HEIGHT = 250;
        private readonly IArticle _article;
        private readonly IActionNoFile<Heading, Heading> _heading;
        public ArticleController(IEntry entry, IArticle article, IActionNoFile<Heading, Heading> heading) : base(entry, Entity.Article, article)
        {
            _article = article;
            _heading = heading;
        }

        [HttpGet("{id}")]
        public async Task<Article> Get(int id)
        {
            Article article = await _article.GetModelAsync(id);
            return article;
        }
        [HttpGet("headings")]
        public IEnumerable<Heading> Get(Culture? culture, int? articleId)
        {
            if (culture == null && articleId != null)
            {
                culture = _heading.GetModels(new Dictionary<string, object> { { ConstantsService.ARTICLE, articleId } }).FirstOrDefault().Language;
            }
            Dictionary<string, object> dictionary = new();
            if (culture != null)
                dictionary.Add(ConstantsService.CULTURE, culture);
            //if (articleId != null)
            //    dictionary.Add(ConstantsService.ARTICLE, articleId);
            IEnumerable<Heading> headings = _heading.GetModels(dictionary);
            return headings;
        }
        [HttpGet("heading/ids/{articleId}")]
        public IEnumerable<int> GetHeadingIds(int articleId)
        {
            IEnumerable<int> headingIds = _heading.GetModels(new Dictionary<string, object>() { { ConstantsService.ARTICLE, articleId } }).Select(h => h.Id);
            return headingIds;
        }
        [HttpPost]
        public async Task<Result> Post([FromForm] Article article, [FromForm] IFormFile[] images, [FromForm] string headingsJson, [FromForm] IList<int> selectedHeadingIds, [FromForm] IList<int> deleteHeadingIds, [FromForm] Culture culture)
        {
            IList<Heading> newHeadings = JsonService.Deserialize<IList<Heading>>(headingsJson);
            // selectedHeadingIds used for add new link headingarticle, newHeadings used also used for that, but they do not have Id, thus we need check both and add new headings, then add new link with them
            if (!selectedHeadingIds.Any() && !newHeadings.Any())
                return Result.NoConnections;
            Result deleteResult = await DeleteHeadings(deleteHeadingIds);
            if (deleteResult == Result.DeleteError)
                return deleteResult;
            bool isExist = _article.IsExist(article, _article.GetModels(new Dictionary<string, object>() { { ConstantsService.CULTURE, culture } }));
            if (isExist)
                return Result.Already;
            article.Date = DateTime.Now.ToUniversalTime();
            await _article.AddModelAsync(article, images, WIDTH, HEIGHT);
            await ResolveHeadings(newHeadings, culture, article.Id, selectedHeadingIds);
            await AddActAsync(article.Id, article.Name, culture);
            return Result.Success;
        }
        [HttpPut]
        public async Task<Result> Put([FromForm] Article article, [FromForm] IFormFile[] images, [FromForm] string headingsJson, [FromForm] IList<int> selectedHeadingIds, [FromForm] IList<int> deleteHeadingIds, Culture culture)
        {
            IList<Heading> newHeadings = JsonService.Deserialize<IList<Heading>>(headingsJson);
            // selectedHeadingIds used for add new link headingarticle, newHeadings used also used for that, but they do not have Id, thus we need check both and add new headings, then add new link with them
            if (!selectedHeadingIds.Any() && !newHeadings.Any())
                return Result.NoConnections;
            Result deleteResult = await DeleteHeadings(deleteHeadingIds);
            if (deleteResult == Result.DeleteError)
                return deleteResult;
            bool isExist = _article.IsExist(article, _article.GetModels(new Dictionary<string, object>() { { ConstantsService.CULTURE, culture } }).Where(x => x.Id != article.Id));
            if (isExist)
                return Result.Already;
            await _article.EditModelAsync(article, images, WIDTH, HEIGHT);
            await ResolveHeadings(newHeadings, culture, article.Id, selectedHeadingIds);
            await EditActAsync(article.Id, article.Name);
            return Result.Success;
        }
        [HttpDelete("{id}")]
        public async Task<Result> Delete(int id)
        {
            Article article = await _article.GetModelAsync(id);
            if (article == null)
                return Result.Fail;
            Result result = await _article.DeleteModelAsync(article.Id, article);
            if (result == Result.Success)
                await DeleteActAsync(id, article.Name);
            return result;
        }

        private async Task ResolveHeadings(IEnumerable<Heading> headings, Culture culture, int articleId, IList<int> selectedHeadingIds)
        {
            foreach (Heading heading in headings)
            {
                bool isExistHeading = _heading.IsExist(heading, _heading.GetModels(new Dictionary<string, object>() { { ConstantsService.CULTURE, culture } }));
                if (isExistHeading)
                    heading.Id = _heading.GetModels().FirstOrDefault(h => h.Name == heading.Name).Id;
                else
                    await _heading.AddModelAsync(heading, true);
                await AddHeadingArticle(heading.Id, articleId);
            }
            foreach (int headingId in selectedHeadingIds)
            {
                await AddHeadingArticle(headingId, articleId);
            }
            IQueryable<HeadingArticle> headingArticlesToDelete = _article.GetHeadingArticles().Where(ha => ha.ArticleId == articleId && !selectedHeadingIds.Contains(ha.HeadingId));
            foreach (HeadingArticle hv in headingArticlesToDelete)
            {
                _article.DeleteHeadingArticle(hv);
            }
        }

        private async Task AddHeadingArticle(int headingId, int articleId)
        {
            HeadingArticle headingArticle = await _article.GetHeadingArticleAsync(headingId, articleId);
            if (headingArticle == null)
            {
                headingArticle = new()
                {
                    HeadingId = headingId,
                    ArticleId = articleId,
                };
                await _article.AddHeadingArticle(headingArticle); ;
            }
        }

        private async Task<Result> DeleteHeadings(IList<int> deleteHeadingIds)
        {
            if (deleteHeadingIds.Any())
            {
                foreach (int id in deleteHeadingIds)
                {
                    Heading heading = await _heading.GetModelAsync(id);
                    Result result = await _heading.DeleteModelAsync(heading.Id, heading);
                    if (result == Result.Fail)
                        return Result.DeleteError;
                }
            }
            return Result.Success;
        }
    }
}