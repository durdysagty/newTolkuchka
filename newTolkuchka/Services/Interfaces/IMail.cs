namespace newTolkuchka.Services.Interfaces
{
    public interface IMail
    {
        Task<bool> SendPinAsync(string email, int pin);
        Task<bool> SendRecoveryAsync(string email, Guid guid);
        Task<bool> SendNewPinAsync(string email, int pin);
    }
}
