using DocnetCorePractice.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DocnetCorePractice.Attribute
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute()
            : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
