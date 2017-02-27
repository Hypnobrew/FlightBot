
using System;

namespace EzyFlightBot.Models
{

    public class Rootobject
    {
        public Flight[] Flights { get; set; }
    }

    public class Flight
    {
        public string FlightNumber { get; set; }
        public int Id { get; set; }
        public From From { get; set; }
        public To To { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int LowestFareId { get; set; }
        public string Cabin { get; set; }
        public float LowestPrice { get; set; }
        public float LowestPriceDiscount { get; set; }
        public Fare[] Fares { get; set; }
        public Leg[] Legs { get; set; }
        public string Key { get; set; }
        public float LowestPriceWithoutTax { get; set; }
        public int FlightTime { get; set; }
        public bool Soldout { get; set; }
        public bool IsInternational { get; set; }
        public bool IsPlaceHolder { get; set; }
    }

    public class From
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class To
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class Fare
    {
        public float Discount { get; set; }
        public string FareBasis { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public float Price { get; set; }
        public string Code { get; set; }
        public Adult Adult { get; set; }
        public object Child { get; set; }
        public object Infant { get; set; }
        public float PriceWithoutTax { get; set; }
        public int SeatCount { get; set; }
        public bool Refundable { get; set; }
        public Tax1[] Taxes { get; set; }
        public Bookingfee BookingFee { get; set; }
    }

    public class Adult
    {
        public string FareBasis { get; set; }
        public int Id { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }
        public string Code { get; set; }
        public float PriceWithoutTax { get; set; }
        public int SeatCount { get; set; }
        public Tax[] Taxes { get; set; }
    }

    public class Tax
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
    }

    public class Bookingfee
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
    }

    public class Tax1
    {
        public int Id { get; set; }
        public float Price { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
    }

    public class Leg
    {
        public int Id { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int FlightTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FlightNumber { get; set; }
    }
}