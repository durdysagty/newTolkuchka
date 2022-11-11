﻿using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class SpecsValue
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string NameRu { get; set; }
        [Required, MaxLength(100)]
        public string NameEn { get; set; }
        [Required, MaxLength(100)]
        public string NameTm { get; set; }

        public int SpecId { get; set; }
        public Spec Spec { get; set; }
        public ICollection<SpecsValueMod> SpecsValueMods { get; set; }
        public ICollection<ProductSpecsValue> ProductSpecsValues { get; set; }
    }
}
