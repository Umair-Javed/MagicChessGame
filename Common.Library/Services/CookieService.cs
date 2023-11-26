using Common.Library.Dtos;
using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;
using Microsoft.AspNetCore.Http;

namespace Common.Library.Services
{
    public class CookieService : ICookieService
    {
        private readonly ITokenService _tokenService;
        public CookieService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        public void SetCookie(HttpContext context, string cookieName, string cookieValue)
        {
            var existingCookie = context.Request.Cookies[cookieName];

            if (existingCookie != null)
            {
                // Cookie already exists, update its value
                context.Response.Cookies.Append(cookieName, cookieValue, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddHours(1)
                });
            }
            else
            {
                // Cookie doesn't exist, create a new one
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                context.Response.Cookies.Append(cookieName, cookieValue, cookieOptions);
            }
        }

        public CookieDto GetExistingToken(HttpContext httpContext)
        {
            var encryptedToken = GetCookie(httpContext, "ChessGameData");

            if (encryptedToken != null)
            {
                var decryptedData = _tokenService.DecryptToken<CookieDto>(encryptedToken);
                if (decryptedData != null && !string.IsNullOrEmpty(decryptedData.SessionId))
                {
                    return decryptedData;
                }
            }
            return null;
        }

        public string GetCookie(HttpContext context, string cookieName)
        {
            return context.Request.Cookies[cookieName];
        }

        public void SetSessionCookie(HttpContext context, string groupId, string sessionId="", string connectionId="")
        {
            var sessionCookie = new CookieDto
            {
                SessionId = sessionId,
                GroupId = groupId,
                HubConnectionId = connectionId
            };

            var encryptedToken = _tokenService.EncryptToken(sessionCookie);
            SetCookie(context, "ChessGameData", encryptedToken);
        }
    }
}
