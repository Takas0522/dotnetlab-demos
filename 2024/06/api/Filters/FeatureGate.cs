using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.FeatureManagement;

namespace api.Filters;

public class FeatureGateAttribute : ActionFilterAttribute
{
    private readonly string _featureName;

    public FeatureGateAttribute(string featureName) => _featureName = featureName;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var featureManager = (IFeatureManager)context.HttpContext.RequestServices.GetService(typeof(IFeatureManager));
        if (!featureManager.IsEnabledAsync(_featureName).Result)
        {
            context.Result = new NotFoundResult();
        }
    }
}
