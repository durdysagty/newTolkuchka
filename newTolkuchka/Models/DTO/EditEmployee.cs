namespace newTolkuchka.Models.DTO
{
    public class EditEmployee
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int PositionId { get; set; }
        public int Level { get; set; }
    }
}
