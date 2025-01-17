﻿using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VAF13.Klubadmin.Domain.DTOs;

namespace VAF13.Klubadmin.Domain.Services.VAFapi;

public class VafApiIntegration : IVafApiIntegration
{
  private readonly ILogger<VafApiIntegration> _logger;
  private readonly HttpClient _client;

  public VafApiIntegration(HttpClient client,
      ILogger<VafApiIntegration> logger)
  {
    _client = client;
    _logger = logger;
  }

  public async Task<List<PersonDetailsResponse>> SearchAll(string name)
  {
    try
    {
      _logger.LogInformation("Calling API!");
      var timer = Stopwatch.StartNew();
      var response = await _client.GetAsync($"api/Member/SearchAll?name={name}");
      timer.Stop();
      _logger.LogInformation("Called API took {Milliseconds}", timer.ElapsedMilliseconds);

      var responseString = await response.Content.ReadAsStringAsync();
      _logger.LogInformation("API Response string: {JSON}", responseString);

      var deserialized = JsonConvert.DeserializeObject<List<PersonDetailsResponse>>(responseString);
      return deserialized ?? Array.Empty<PersonDetailsResponse>().ToList();
    }
    catch (Exception ex)
    {
      _logger.LogInformation(ex, "Exception during external API call - {Message}", ex.Message);
      return Array.Empty<PersonDetailsResponse>().ToList();
    }
  }
}

public class VAFApiIntegrationHttpClient
{ }