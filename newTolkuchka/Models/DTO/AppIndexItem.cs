namespace newTolkuchka.Models.DTO
{
    public class AppIndexItem
    {
        public AppProductsModel ProductsModel { get; set; }
        public List<IEnumerable<UIProduct>> Products { get; set; }
    }
}
