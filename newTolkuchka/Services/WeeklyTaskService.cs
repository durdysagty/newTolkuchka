using Microsoft.Extensions.Caching.Memory;
using newTolkuchka.Models;
using newTolkuchka.Services.Interfaces;
using static newTolkuchka.Services.CultureProvider;

namespace newTolkuchka.Services
{

    public class WeeklyTaskService : IHostedService, IDisposable
    {
        private readonly IPath _path;
        private readonly IMemoryCache _memoryCache;
        private Timer _timer;

        public WeeklyTaskService(IPath path, IMemoryCache memoryCache)
        {
            _path = path;
            _memoryCache = memoryCache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            DateTime currentTime = DateTime.Now;
            DateTime nextRunTime = GetNextSunday1AM(currentTime);
            TimeSpan initialDelay = nextRunTime - currentTime;
            _timer = new Timer(PerformOperation, null, initialDelay, TimeSpan.FromDays(7));
            //_timer = new Timer(PerformOperation, null, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3));
            return Task.CompletedTask;
        }

        private static DateTime GetNextSunday1AM(DateTime from)
        {
            if (from.DayOfWeek == DayOfWeek.Sunday && from.TimeOfDay > new TimeSpan(1, 0, 0))
            {
                // if today is Sunday and it's after 1:00 AM, get the next Sunday
                from = from.AddDays(1);
            }

            // calculate the days until next Sunday
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)from.DayOfWeek + 7) % 7;
            // get next Sunday
            var nextSunday = from.AddDays(daysUntilSunday);
            // set the time to 1:00 AM
            return new DateTime(nextSunday.Year, nextSunday.Month, nextSunday.Day, 1, 0, 0);
        }

        private async void PerformOperation(object state)
        {
            try
            {
                string[] modelsToSiteMap = { Entity.Product.ToString().ToLower(), Entity.Promotion.ToString().ToLower(), Entity.Category.ToString().ToLower(), Entity.Brand.ToString().ToLower(), Entity.Article.ToString().ToLower() };
                foreach (var model in modelsToSiteMap)
                {
                    string path = _path.GetSiteMap(model);
                    bool isAny = _memoryCache.TryGetValue(path, out IList<(int, bool, Culture?)> entities);
                    if (isAny)
                    {
                        List<string> strings = File.ReadAllLines(path).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        foreach ((int, bool, Culture?) e in entities)
                        {
                            string modelUrl = $"/{PathService.GetModelUrl(model, e.Item1)}";
                            List<string> urls = new();
                            if (e.Item3 == null)
                            {
                                urls = new List<string> { $"{SiteUrlRu}{modelUrl}", $"{SiteUrlEn}{modelUrl}", $"{SiteUrlTm}{modelUrl}" };
                            }
                            else
                            {
                                // used for articles, i.e. they are only for current culture
                                if (e.Item3 == Culture.Ru)
                                    urls = new List<string> { $"{SiteUrlRu}{modelUrl}" };
                                if (e.Item3 == Culture.En)
                                    urls = new List<string> { $"{SiteUrlEn}{modelUrl}" };
                                if (e.Item3 == Culture.Tm)
                                    urls = new List<string> { $"{SiteUrlTm}{modelUrl}" };
                            }
                            if (e.Item2)
                            {
                                if (!strings.Any(s => s == urls[0]))
                                    strings = strings.Concat(urls).ToList();
                            }
                            else
                                foreach (string u in urls)
                                    strings.Remove(u);
                        }
                        // replace to remove
                        await File.WriteAllLinesAsync(path, strings.OrderByDescending(s => s, new CompareForSiteMapService()));
                        _memoryCache.Remove(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
