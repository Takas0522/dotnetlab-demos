using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureFlagsController : ControllerBase
{

    private readonly IConfiguration _config;
    private readonly IFeatureManager _feature;

    public FeatureFlagsController(IConfiguration config, IFeatureManager feature)
    {
        _config = config;
        _feature = feature;
    }

    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var dict = new Dictionary<string, bool>();
        var featureAddColumn = await _feature.IsEnabledAsync(Settings.FeatureFlags.FeatureAddColumn);
        var featureSearch = await _feature.IsEnabledAsync(Settings.FeatureFlags.FeatureSearch);
        var featureMaterial = await _feature.IsEnabledAsync(Settings.FeatureFlags.FeatureMaterial);
        dict.Add(Settings.FeatureFlags.FeatureAddColumn, featureAddColumn);
        dict.Add(Settings.FeatureFlags.FeatureSearch, featureSearch);
        dict.Add(Settings.FeatureFlags.FeatureMaterial, featureMaterial);

        return Ok(dict);
    }
}