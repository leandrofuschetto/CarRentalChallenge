using System.Runtime.CompilerServices;

namespace CarRental.Service
{
    public static class Utils
    {
        public static string GetActualAsyncMethodName([CallerMemberName] string name = "") => name;
    }
}
