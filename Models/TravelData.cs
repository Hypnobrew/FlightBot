using System;

namespace EzyFlightBot.Models
{
    [Serializable]
    public class TravelData
    {
        public string From { get; set; }
        public string FromCode { get; set; }
        public string To { get; set; }
        public string ToCode { get; set; }
        public DateTime TravelDate { get; set; }
        public string Token { get; set; }
    }
}