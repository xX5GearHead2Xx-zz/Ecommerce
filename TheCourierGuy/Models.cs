using System.Collections.Generic;

namespace Ecommerce.TheCourierGuy
{
    public class Models
    {
        public class Shipment
        {
            public Address collection_address { get; set; }
            public Address delivery_address { get; set; }
            public List<Parcel> parcels { get; set; }
            public string special_instructions_collection { get; set; }
            public string special_instructions_delivery { get; set; }
            public int declared_value { get; set; }
            public string collection_after { get; set; }
            public string collection_before { get; set; }
            public string delivery_after { get; set; }
            public string delivery_before { get; set; }
            public string customer_refrence { get; set; }
            public string service_level_code { get; set; }
            public Contact collection_contact { get; set; }
            public Contact delivery_contact { get; set; }

        }

        public class ShipmentResponse
        {
            public Account account { get; set; }
            public string short_tracking_reference { get; set; }
        }

        public class Account
        {
            public string id { get; set; }
            public string name { get; set; }
            public decimal balance { get; set; }
        }

        public class RateRequest
        {
            public Address collection_address { get; set; }
            public Address delivery_address { get; set; }
            public List<Parcel> parcels { get; set; }
            public int declared_value { get; set; }
        }

        public class RateResponse
        {
            public string message { get; set; }
            public List<Rate> rates { get; set; }
        }

        public class Rate
        {
            public decimal rate { get; set; }
            public decimal rate_excluding_vat { get; set; }
            public BaseRate base_rate { get; set; }
            public ServiceLevel service_level { get; set; }
        }

        public class BaseRate
        {
            public string[] charge_per_parcel { get; set; }
            public decimal charge { get; set; }
            public string group_name { get; set; }
            public decimal vat { get; set; }
            public float total_calculated_weight { get; set; }
        }

        public class ServiceLevel
        {
            public string id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public bool insurance_disabled { get; set; }
        }

        public class Address
        {
            public string type { get; set; }
            public string company { get; set; }
            public string street_address { get; set; }
            public string local_area { get; set; }
            public string city { get; set; }
            public string code { get; set; }
            public string zone { get; set; }
            public string country { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Contact
        {
            public string name { get; set; }
            public string mobile_number { get; set; }
            public string email { get; set; }
        }

        public class Parcel
        {
            public float submitted_length_cm { get; set; }
            public float submitted_width_cm { get; set; }
            public float submitted_height_cm { get; set; }
            public float submitted_weight_kg { get; set; }
        }
    }
}
