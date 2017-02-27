using System;

namespace EzyFlightBot.Models
{
    [Serializable]
    public class AirportCandidate
    {
        public AirportCandidate(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public string Name { get; private set; }
        public string Code { get; private set; }
    }
}