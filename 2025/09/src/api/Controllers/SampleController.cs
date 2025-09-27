using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using api.Services;
using api.DTOs;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{

    [HttpGet]
    public ActionResult Sample()
    {
        // あえてエラー発生させたいので0除算を仕込んでおく
        int zero = 0;
        int result = 1 / zero;
        return Ok(new { Text = "Sample" });
    }

}
