using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DocnetCorePractice.Extensions
{
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        private const string ApiKeyHeaderName = "X-API-Key";

        private const string ApiKeyValue = "abccieofidsfiosdfois";

        private readonly IApiKeyValidator _apiKeyValidator;

        public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
        {
            _apiKeyValidator = apiKeyValidator;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

            if (!_apiKeyValidator.IsValid(apiKey))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        public class ApiKeyValidator : IApiKeyValidator
        {
            public bool IsValid(string apiKey)
            {
                if (apiKey == ApiKeyValue)
                {
                    return true;
                }
                // Implement logic for validating the API key.
                return false;
            }
        }

        public interface IApiKeyValidator
        {
            bool IsValid(string apiKey);
        }
    }
}
