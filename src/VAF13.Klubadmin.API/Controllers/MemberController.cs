﻿using Microsoft.AspNetCore.Mvc;
using VAF13.Klubadmin.Domain.Services.Klubadmin.Interfaces;

namespace VAF13.Klubadmin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ILogger<MemberController> _logger;
        private readonly IKlubAdminService _klubAdminService;

        public MemberController(ILogger<MemberController> logger,IKlubAdminService klubAdminService)
        {
            _logger = logger;
            _klubAdminService = klubAdminService;
        }
        
        [HttpGet("/Person")]
        public async Task<IActionResult> GetPersonInfo(string personId, [FromQuery] string? club)
        {
            var personInfo = await _klubAdminService.GetPersonInfo(personId, club ?? string.Empty);
            return Ok(personInfo);
        }

        [HttpGet("/Search")]
        public async Task<IActionResult> Search(string name)
        {
            var searchResult = await _klubAdminService.SearchPerson(name);
            return Ok(searchResult);
        }

        [HttpGet("/SearchAll")]
        public async Task<IActionResult> SearchAll(string name)
        {
            var searchResult = await _klubAdminService.SearchAll(name);
            return Ok(searchResult);
        }
    }
}