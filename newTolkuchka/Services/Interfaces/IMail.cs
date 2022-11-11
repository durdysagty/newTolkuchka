namespace newTolkuchka.Services.Interfaces
{
    public interface IMail
    {
        Task<bool> SendPinAsync(string email, int pin);
    }
}
