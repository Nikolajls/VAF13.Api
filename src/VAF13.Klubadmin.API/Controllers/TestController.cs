using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAF13.Klubadmin.API.Services.Interfaces;

namespace VAF13.Klubadmin.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase
  {
    private readonly ILogger<TestController> _logger;
    private readonly IKlubAdminAuthService _authService;
    private readonly IKlubAdminService _klubAdminService;

    public TestController(ILogger<TestController> logger, IKlubAdminAuthService authService, IKlubAdminService klubAdminService)
    {
      _logger = logger;
      _authService = authService;
      _klubAdminService = klubAdminService;
    }


    [HttpGet("/Person")]
    public async Task<IActionResult> GetPersonInfo(string personId)
    {
      var r = await _klubAdminService.GetPersonInfo(personId);

      return Ok(r);
    }

    [HttpGet("/Search")]
    public async Task<IActionResult> Search(string name)
    {
      var r = await _klubAdminService.SearchPerson(name);

      return Ok(r);
    }
  }
}
