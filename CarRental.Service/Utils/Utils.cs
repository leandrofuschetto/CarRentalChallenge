using System.Runtime.CompilerServices;

namespace CarRental.Service
{
    public static class Utils
    {
        public static string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;

        public static Func<DateOnly, DateOnly, DateOnly, bool> IsInRange = (from, to, dateToCheck) =>
        {
            return dateToCheck >= from && dateToCheck <= to;
        };
    }
}
