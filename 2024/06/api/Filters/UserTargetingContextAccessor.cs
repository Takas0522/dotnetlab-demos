using Microsoft.FeatureManagement.FeatureFilters;

namespace api.Filters
{
    public class UserTargetingContextAccessor : ITargetingContextAccessor
    {
        private const string TargetContextLookup = "UserTargetingContextAccessor.TargetingContext";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserTargetingContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public ValueTask<TargetingContext> GetContextAsync()
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            if (httpContext.Items.TryGetValue(TargetContextLookup, out object value))
            {
                return new ValueTask<TargetingContext>((TargetingContext)value);
            }
            List<string> groups = new List<string>();
            if (httpContext.User.Identity.Name != null)
            {
                groups.Add(httpContext.User.Identity.Name.Split("@", StringSplitOptions.None)[1]);
            }
            TargetingContext targetingContext = new TargetingContext
            {
                UserId = httpContext.User.Identity.Name,
                Groups = groups
            };
            httpContext.Items[TargetContextLookup] = targetingContext;
            return new ValueTask<TargetingContext>(targetingContext);
        }
    }
}
