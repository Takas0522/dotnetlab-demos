using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

/// <summary>
/// MyDataControllerはデータの抽出を行うコントローラーです。
/// </summary>
[ApiController]
[Route("[controller]")]
public class MyDataController : ControllerBase
{

    private readonly ILogger<MyDataController> _logger;

    public MyDataController(ILogger<MyDataController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "MyListDataGet")]
    public ActionResult GetListData()
    {
        // "FeatureAddColumn"はリストに表示される項目を追加します
        // "FeatureMaterial"はFrontendのみですが、Material Designを使用します。
    }

    [HttpGet(Name = "MyListDatasGetAndSearch")]
    public ActionResult GetAndSearchData()
    {
        // "FeatureSearch"は、検索機能を実行可能にするFeatureFlagです。
        // "FeatureAddColumn"はリストに表示される項目を追加します
        // "FeatureMaterial"はFrontendのみですが、Material Designを使用します。

    }
}
