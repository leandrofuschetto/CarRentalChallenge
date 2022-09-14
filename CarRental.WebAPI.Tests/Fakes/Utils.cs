using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebAPI.Tests.Fakes
{
    internal static class Utils
    {
        public static T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }

    }
}
