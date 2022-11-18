using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using newTolkuchka.Models;
using newTolkuchka.Reces;
using newTolkuchka.Services.Interfaces;
using System.Reflection;
using System.Runtime.CompilerServices;
using Type = System.Type;

namespace newTolkuchka.Services.Abstracts
{
    public abstract class BaseService
    {
        private readonly IStringLocalizer<Shared> _localizer;
        public BaseService(IStringLocalizer<Shared> localizer)
        {
            _localizer = localizer;
        }       

        public string GetPagination(int pp, int total, int pageCount, int toSkip, out int lastPage)
        {
            lastPage = total % pp == 0 ? (total / pp) - 1 : (total / pp);
            string pagination = $"{toSkip + 1} - {(pageCount < pp ? toSkip + pageCount : toSkip + pp)} {_localizer["of"]} {total}";
            return pagination;
        }
    }
}
