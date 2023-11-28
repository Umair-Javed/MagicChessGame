using Common.Library.Dtos;
using Microsoft.AspNetCore.Http;

namespace Common.Library.Interfaces
{
    public interface ICookieService
    {
        void SetSessionCookie(HttpContext context, string groupId, string sessionId = "", string connectionId = "");
        void SetCookie(HttpContext context, string cookieName, string cookieValue);
        string GetCookie(HttpContext context, string cookieName);
        CookieDto GetExistingToken(HttpContext httpContext);
    }
}
