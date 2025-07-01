using System.Text.Json;

namespace InventoryService.Response
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var error = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Ocurrio un error inesperado. Por favor intente más tarde",
                    Details = ex.Message
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            }
        }
    }
}
