using System;
using System.Threading.Tasks;
using EzyFlightBot.Models;

namespace EzyFlightBot.Services
{
    public interface IEzyFlight
    {
        Task<string> GetAuthToken(string username, string password, string url);
        Task<Flight[]> GetAvailability(string token, string from, string to, DateTime when, string url);
        Task<FromAirports> GetFromAirports(string token, string url);
        Task<string> GetSecurityToken(string token, string url);
        Task<FromAirports> GetToAirports(string token, string origin, string url);
    }
}