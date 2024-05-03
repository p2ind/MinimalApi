using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace MinimalAPIsMovies.Utilities
{
    public static class HttpContextExtensionsUtilities
    {
        public static T ExtractValueOrDefault<T>(this HttpContext context, string field, T defaultValue) where T : IParsable<T>
        {
            var value = context.Request.Query[field];

            if(value.IsNullOrEmpty())
            {
                return defaultValue;
            }

            return T.Parse(value!, null);
        }
    }
}
