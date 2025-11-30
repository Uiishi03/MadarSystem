namespace Madar.Common.Middleware
{
    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _timeoutMinutes;

        public SessionTimeoutMiddleware(RequestDelegate next, int timeoutMinutes = 120)
        {
            _next = next;
            _timeoutMinutes = timeoutMinutes;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var lastActivity = context.Session.GetString("LastActivityTime");

                if (!string.IsNullOrEmpty(lastActivity))
                {
                    var lastActivityTime = DateTime.Parse(lastActivity);
                    if (DateTime.Now.Subtract(lastActivityTime).TotalMinutes > _timeoutMinutes)
                    {
                        context.Session.Clear();
                        context.Response.Redirect("/Auth/Login?timeout=true");
                        return;
                    }
                }

                context.Session.SetString("LastActivityTime", DateTime.Now.ToString());
            }

            await _next(context);
        }
    }

    // Extension method to make it easier to register
    public static class SessionTimeoutMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionTimeout(
            this IApplicationBuilder builder,
            int timeoutMinutes = 120)
        {
            return builder.UseMiddleware<SessionTimeoutMiddleware>(timeoutMinutes);
        }
    }
}