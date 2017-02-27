using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EzyFlightBot.Models;

namespace EzyFlightBot.Services
{
    [Serializable]
    public class EzyFlight : IEzyFlight
    {
        public async Task<string> GetAuthToken(string username, string password, string url)
        {
            var client = new HttpClient();
            var data = await client.PostAsync($"{url}/auth/token", new StringContent($"grant_type=password&username={username}&password={password}"));
            var token = await data.Content.ReadAsStringAsync();
            var parsedData = JsonConvert.DeserializeObject<Auth>(token);

            return parsedData.access_token;
        }

        public async Task<FromAirports> GetFromAirports(string token, string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
            var response = await client.GetAsync($"{url}/airport/origins");
            var data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FromAirports>(data);
        }

        public async Task<FromAirports> GetToAirports(string token, string origin, string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
            var response = await client.GetAsync($"{url}/airport/{origin}/destinations");
            var data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FromAirports>(data);
        }

        public async Task<Flight[]> GetAvailability(string token, string from, string to, DateTime when, string url)
        {
            var securityToken = await GetSecurityToken(token, url);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
            client.DefaultRequestHeaders.Add("X-SecurityToken", securityToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var criteria = new Criteria()
            {
                Routes = new Route[] { new Route() {
                    FromAirport = from,
                    ToAirport = to,
                    StartDate = when.ToString("yyyy-MM-dd"),
                    EndDate = when.ToString("yyyy-MM-dd")
                }}
            };
            var postData = JsonConvert.SerializeObject(criteria);
            var response = await client.PostAsync($"{url}/availability/search", new StringContent(postData, Encoding.UTF8, "application/json"));
            var data = await response.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<Rootobject>(data);

            return root.Flights;
        }

        public async Task<string> GetSecurityToken(string token, string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
            var response = await client.GetAsync($"{url}/security/token");
            var data = await response.Content.ReadAsStringAsync();
            var parsedData = JsonConvert.DeserializeObject<SecurityToken>(data);

            return parsedData.Token;
        }
    }
}