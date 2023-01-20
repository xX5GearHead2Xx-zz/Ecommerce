using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Ecommerce.TheCourierGuy.Models;

namespace Ecommerce.TheCourierGuy
{
    public class API
    {
        public static async Task<RateResponse> GetRates(RateRequest RateRequest)
        {
            Request req = new Request(HttpMethod.Post, "https://api.shiplogic.com/rates", new StringContent(JsonConvert.SerializeObject(RateRequest), Encoding.UTF8, "application/json"));
            string Result = await req.Send();
            return JsonConvert.DeserializeObject<RateResponse>(Result);
        }

        public static async Task<ShipmentResponse> CreateShipment(Shipment Shipment)
        {
            Request req = new Request(HttpMethod.Post, "https://api.shiplogic.com/shipments", new StringContent(JsonConvert.SerializeObject(Shipment), Encoding.UTF8, "application/json"));

            string Result = await req.Send();
            return JsonConvert.DeserializeObject<ShipmentResponse>(Result);
        }
    }
}
