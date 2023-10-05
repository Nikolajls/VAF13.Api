using Newtonsoft.Json;
using System.Collections.Generic;
using VAF13.Klubadmin.Domain.DTOs;

namespace VAF13.Klubadmin.Domain.Services.VAFapi
{
    public interface IVAFApiIntegration
    {
        Task<List<PersonDetails>> SearchAll(string name);
    }

    public class VAFApiIntegration : IVAFApiIntegration
    {
        private readonly HttpClient _client;

        public VAFApiIntegration(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<PersonDetails>> SearchAll(string name)
        {
            try
            {
                var response = await _client.GetAsync($"/SearchAll?name={name}");
                var responseString = await response.Content.ReadAsStringAsync();
                var deserialized = JsonConvert.DeserializeObject<List<PersonDetails>>(responseString);
                return deserialized ?? Array.Empty<PersonDetails>().ToList();

            }
            catch (Exception ex)
            {
                return Array.Empty<PersonDetails>().ToList();
            }
        }
    }
}
