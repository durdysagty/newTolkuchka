namespace newTolkuchka.Models
{
    public class HeadingArticle
    {
        public int HeadingId { get; set; }
        public Heading Heading { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
