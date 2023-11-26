using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Interfaces
{
    public interface ITokenService
    {
        string EncryptToken(object data);
        T DecryptToken<T>(string encryptedData);
    }
}
