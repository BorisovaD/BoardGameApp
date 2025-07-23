namespace BoardGameApp.Web.Infrastructure.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ManagerAccessMiddleware
    {
        private readonly RequestDelegate next;

        public ManagerAccessMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
                        
            if (path != null && path.StartsWith("/manager"))
            {
                if (!(context.User.Identity?.IsAuthenticated ?? false))
                {
                    context.Response.Redirect("/Home/Error?statusCode=403");
                    return;
                }

                if (!context.User.IsInRole("Manager"))
                {
                    context.Response.Redirect("/Home/Error?statusCode=403");
                    return;
                }
            }

            await next(context);
        }
    }
}
