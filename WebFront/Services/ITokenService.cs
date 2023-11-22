namespace WebFront.Services
{
    public interface ITokenService
    {
        string EncryptToken(object data);
        T DecryptToken<T>(string encryptedData);
    }
}
