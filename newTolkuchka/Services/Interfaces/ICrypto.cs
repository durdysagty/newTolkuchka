using System.Security.Cryptography;

namespace newTolkuchka.Services.Interfaces
{
    public interface ICrypto
    {
        string EncryptString(string toEncrypt);
        string DecryptString(string value);
        static int GetNumber(int x, int y) => RandomNumberGenerator.GetInt32(x, y);
    }
}
