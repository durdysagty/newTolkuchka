namespace newTolkuchka.Models.DTO
{
    public class LoginResponse
    {
        public enum R { Success, Fail, New, NotConfirmed }
        public R Result { get; set; }
        public string Data { get; set; }
        public string Text { get; set; }
    }
}
