namespace EzyFlightBot.Models
{
    public class Criteria
    {
        public int Adults => 1;
        public int Children => 0;
        public int Infants => 0;
        public Route[] Routes { get; set; }
        public string PromoCode => "";
        public int CreditId => -1;
        public string Currency => "COP";
    }

    public class Route
    {
        public string FromAirport { get; set; }
        public string ToAirport { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}