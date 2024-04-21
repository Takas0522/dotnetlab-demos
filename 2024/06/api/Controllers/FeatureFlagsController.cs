using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeatureFlagsController : ControllerBase
{

    private readonly IConfiguration _config;

    public FeatureFlagsController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet()]
    public IActionResult Get()
    {
        var configValue = _config.GetSection("FeatureFlags");
        var dict = new Dictionary<string, bool>();
        configValue.Bind(dict);
        return Ok(dict);
    }
}