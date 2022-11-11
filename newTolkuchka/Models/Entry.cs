﻿using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models
{
    public enum Act { Add, Edit, Delete }
    public enum Entity { Brand, Category, Content, Currency, Employee, Invoice, Line, Model, Position, Product, Slide, Spec, SpecsValue, SpecsValueMod, Type, Warranty, Wish }
    public class Entry
    {
        public int Id { get; set; }
        [Required, MaxLength(30)]
        public string Employee { get; set; }
        [Required]
        public Act Act { get; set; }
        [Required]
        public Entity Entity { get; set; }
        [Required]
        public int EntityId { get; set; }
        [MaxLength(100)]
        public string EntityName { get; set; }
        [Required]
        public DateTimeOffset DateTime { get; set; }
    }
}
