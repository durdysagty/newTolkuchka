namespace newTolkuchka.Models
{
    public class ModelSpec
    {
        public int ModelId { get; set; }
        public Model Model { get; set; }
        public int SpecId { get; set; }
        public Spec Spec { get; set; }
        public bool IsNameUse { get; set; }
    }
}
