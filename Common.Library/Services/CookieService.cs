using Common.Library.Dtos;
using Common.Library.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Common.Library.Services
{
    // Represents a service for managing cookies in the context of ChessGameData.
    public class CookieService : ICookieService
    {
        #region Constructor and Private Properties
        // Token service for handling token encryption and decryption.
        private readonly ITokenService _tokenService;

        // Constructor for CookieService, taking a token service through dependency injection.
        public CookieService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        #endregion

        #region Cookie Management Methods

        // Sets a cookie with the specified name and value in the provided HttpContext.
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

        // Gets the existing ChessGameData token from the HttpContext.
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

        // Gets the value of a cookie with the specified name from the HttpContext.
        public string GetCookie(HttpContext context, string cookieName)
        {
            return context.Request.Cookies[cookieName];
        }

        // Sets a session cookie with ChessGameData information in the HttpContext.
        public void SetSessionCookie(HttpContext context, string groupId, string sessionId = "", string connectionId = "")
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

        #endregion
    }

}
