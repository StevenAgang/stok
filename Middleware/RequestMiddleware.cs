using stok.Repository.Configurations;
using stok.Repository.Configurations.Helper;

namespace stok.Middleware
{
    public class RequestMiddleware(RequestDelegate next, ResponseHelper response)
    {
        private readonly RequestDelegate _next = next;
        private readonly ResponseHelper _response = response;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequest ex)
            {
                await Response(context, 400, "application/json", ex.Message);
            }
            catch (Exception ex)
            {
                await Response(context, 500, "application/json", ex.Message);
            }
        }

        public async Task<HttpContext> Response(HttpContext context, int statusCode, string contentType, string error)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = contentType;
            await context.Response.WriteAsJsonAsync(_response.Status(statusCode,false, error, null));

            return context;
        }
    }
}
