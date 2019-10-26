using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using servicer.API.Data;

namespace servicer.API.Helpers
{
    public class SetLastActive : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();

            if (result.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                var userId = int.Parse(result.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var repository = result.HttpContext.RequestServices.GetService<IServicerRepository>();

                var user = await repository.GetUser(userId);
                user.LastActive = DateTime.Now;
                await repository.SaveAll();
            }
        }
    }
}