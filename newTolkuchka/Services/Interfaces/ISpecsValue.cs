﻿using newTolkuchka.Models;
using newTolkuchka.Models.DTO;

namespace newTolkuchka.Services.Interfaces
{
    public interface ISpecsValue : IActionFormFile<SpecsValue>
    {
        IEnumerable<AdminSpecsValue> GetAdminSpecsValues(int specId);
    }
}
