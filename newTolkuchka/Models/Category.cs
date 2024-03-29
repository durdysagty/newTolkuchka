﻿using newTolkuchka.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public class Category: MRP
    {
        public int Order { get; set; }
        //public int Level { get; set; }
        public int ParentId { get; set; }
        [Required, MaxLength(100)]
        public string NameRu { get; set; }
        [Required, MaxLength(100)]
        public string NameEn { get; set; }
        [Required, MaxLength(100)]
        public string NameTm { get; set; }
        public bool IsForHome { get; set; }
        public bool NotInUse { get; set; }

        // to create additional conttections of categories
        public ICollection<CategoryAdLink> CategoryAdLinks { get; set; }
        public ICollection<Model> Models { get; set; }
        public ICollection<CategoryModelAdLink> CategoryModelAdLinks { get; set; }
    }
}
