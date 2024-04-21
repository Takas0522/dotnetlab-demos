using api.Filters;
using api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace api.Controllers;

/// <summary>
/// MyDataControllerはデータの抽出を行うコントローラーです。
/// </summary>
[ApiController]
public class MyDataController : ControllerBase
{

    private readonly ILogger<MyDataController> _logger;
    private readonly IMyDataDomain _myDataDomain;
    private readonly INewMyDataDomain _newMyDataDomain;
    private readonly IFeatureManager _featureManager;

    public MyDataController(
        ILogger<MyDataController> logger,
        IMyDataDomain myDataDomain,
        INewMyDataDomain newMyDataDomain,
        IFeatureManager featureManager
    )
    {
        _logger = logger;
        _myDataDomain = myDataDomain;
        _featureManager = featureManager;
        _newMyDataDomain = newMyDataDomain;
    }

    [HttpGet("api/[controller]")]
    public async Task<ActionResult> GetListData()
    {
        if (await _featureManager.IsEnabledAsync(FeatureFlags.FeatureAddColumn)) // "FeatureAddColumn"はリストに表示される項目を追加します
        {
            var ans = _newMyDataDomain.GetMyListData();
            return Ok(ans);
        }
        var res = _myDataDomain.GetMyListData();
        return Ok(res);
    }

    [HttpGet("/api/[controller]/{id}")]
    [FeatureGate("FeatureSearch")]  // "FeatureSearch"は、検索機能を実行可能にするFeatureFlagです。
    public async Task<ActionResult> GetAndSearchData(string id)
    {
        if (await _featureManager.IsEnabledAsync(FeatureFlags.FeatureAddColumn))
        {
            var ans = _newMyDataDomain.GetMyListData(id);
            return Ok(ans);
        }
        var res = _myDataDomain.GetMyListData(id);
        return Ok(res);

    }
}
