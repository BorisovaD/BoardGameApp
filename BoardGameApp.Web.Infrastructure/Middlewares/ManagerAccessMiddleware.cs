namespace BoardGameApp.Web.Infrastructure.Middlewares
{
    using BoardGameApp.GCommon;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static BoardGameApp.GCommon.ApplicationConstants;
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
                var isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                if (!(context.User.Identity?.IsAuthenticated ?? false) || !context.User.IsInRole(ApplicationConstants.RoleManager))
                {
                    if (isAjax)
                    {
                        context.Response.StatusCode = 403; 
                        return;
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Error?statusCode=403");
                        return;
                    }
                }                
            }

            await next(context);
        }
    }
}
