using System.ComponentModel.DataAnnotations;

namespace newTolkuchka.Models.DTO
{
    public class InputStandart
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Required { get; set; }
        public string OnInput { get; set; }
        public string OnInvalid { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Wrong { get; set; }
        public string Classes { get; set; }
        public string Disabled { get; set; }
        public string Minlength { get; set; }
        public string Maxlength { get; set; }
    }
}
