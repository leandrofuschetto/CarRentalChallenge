using CarRental.Domain.Exceptions;
using CarRental.WebAPI.Exceptions;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace CarRental.WebAPI.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                await HandleExceptionAsync(context, error);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            var customErrorCode = GetCustomPropertyCode(error);

            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case FieldMandatoryException:
                case ArgumentNullException:
                case BadHttpRequestException:
                case MaxLenghtException:
                case DatesInvalidException:
                case EmailinUseException:
                case ClientInactiveException:
                case ModelVehicleInUseException:
                case VehicleInactiveException:
                case VehicleUnavailableException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case EntityNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case DataBaseContextException:
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer
                .Serialize(new { message = error?.Message, code = customErrorCode });

            return response.WriteAsync(result);
        }

        /// <summary>
        /// Get "Code" custom property with Reflection. 
        /// Property add to all custom exceptions to add information
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private string GetCustomPropertyCode(Exception error)
        {
            var prop = error.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name == "Code")
                .FirstOrDefault();

            string code = "GENERAL_ERROR";
            if (prop != null)
                code = prop.GetValue(error, null).ToString();

            return code;
        }
    }
}
