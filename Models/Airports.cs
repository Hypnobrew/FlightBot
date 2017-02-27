using System;

namespace EzyFlightBot.Models
{
    [Serializable]
    public class FromAirports
    {
        public Airport[] Airports { get; set; }
    }

    [Serializable]
    public class Airport
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}