using Microsoft.AspNetCore.Mvc;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ILogger<MemberController> _logger;
        private readonly IKlubAdminService _klubAdminService;

        public MemberController(ILogger<MemberController> logger, IKlubAdminService klubAdminService)
        {
            _logger = logger;
            _klubAdminService = klubAdminService;
        }

        [HttpGet]
        [Route("Person")]
        public async Task<IActionResult> GetPersonInfo([FromQuery] int personId)
        {
            var personInfo = await _klubAdminService.GetPersonInfo(personId);
            return Ok(personInfo);
        }

        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var searchResult = await _klubAdminService.SearchPerson(name);
            return Ok(searchResult);
        }

        [HttpGet]
        [Route("SearchAll")]
        public async Task<IActionResult> SearchAll([FromQuery] string name)
        {
            var searchResult = await _klubAdminService.SearchAll(name);
            return Ok(searchResult);
        }
    }
}