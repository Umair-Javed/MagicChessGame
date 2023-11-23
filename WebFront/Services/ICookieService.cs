using WebFront.Models;

namespace WebFront.Services
{
    public interface ICookieService
    {
        void SetCookie(HttpContext context, string cookieName, string cookieValue);
        string GetCookie(HttpContext context, string cookieName);
    }
}
