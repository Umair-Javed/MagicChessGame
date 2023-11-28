namespace Common.Library.Interfaces
{
    public interface ITokenService
    {
        string EncryptToken(object data);
        T DecryptToken<T>(string encryptedData);
    }
}
