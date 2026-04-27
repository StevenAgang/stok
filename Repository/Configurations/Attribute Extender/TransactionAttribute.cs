using Microsoft.AspNetCore.Mvc.Filters;

namespace stok.Repository.Configurations.Attribute_Extender
{
    public class TransactionAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dbContext = context.HttpContext.RequestServices.GetRequiredService<DatabaseContext>();

            if(dbContext == null)
            {
                await next();
                return;
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            var result = await next();

            if(result.Exception == null)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }
        }
    }
}
