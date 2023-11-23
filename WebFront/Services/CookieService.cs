namespace WebFront.Services
{
    public class CookieService : ICookieService
    {
        public void SetCookie(HttpContext context, string cookieName, string cookieValue)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            context.Response.Cookies.Append(cookieName, cookieValue, cookieOptions);
        }

        public string GetCookie(HttpContext context, string cookieName)
        {
            return context.Request.Cookies[cookieName];
        }
    }
}
