﻿namespace newTolkuchka.Models.DTO
{
    public class AdminOrder
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string SerialNumbers { get; set; }
        public decimal OrderPrice { get; set; }
        public int Quantity { get; set; }
    }
}