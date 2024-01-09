using CommunicationAPI.Extension;
using CommunicationAPI.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CommunicationAPI.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContent = await next();
            if (!resultContent.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContent.HttpContext.User.getUserId();

            var repo = resultContent.HttpContext.RequestServices.GetRequiredService<IuserRepo>();
            var user = await repo.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();

        }
    }
}
