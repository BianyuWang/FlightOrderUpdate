using FlightOrderSimulationConsole.Models;
using FlightOrderUpdate.Data;
using FlightOrderUpdate.IService;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightOrderUpdate.Service
{
    public class DataAccessService : IDataAccessService
    {
    
        public AirLine GetAirLineById(int id)
        {
            return FlightScheduledData.AirLines.FirstOrDefault(a => a.AirLineId == id);

        }

        public List<AirLine> GetAirLines(int id)
        {
            return FlightScheduledData.AirLines.ToList();
        }

        public FlightSchedule GetFlightScheduleByDay(int dayAt)
        {
            return GetFlightScheduleList().ElementAt(dayAt);
        }

        public List<FlightSchedule> GetFlightScheduleList()
        {
            return FlightScheduledData.FlightScheduleList;
        }

        public List<OrderComeIn> ReadOrderFromJson()
        {
            List<OrderComeIn> orders = new List<OrderComeIn>();

            //get file
            var path = Directory.GetCurrentDirectory();
            int first = path.IndexOf("bin");
            var jsonfile = path.Substring(0, first) + "files\\orders.json";

            //read file into dynamic object
            StreamReader r = new StreamReader(jsonfile);
            string jsonString = r.ReadToEnd();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            dynamic details = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, new ExpandoObjectConverter());
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            // convert to a dictionary, key is the order number, and value should be an other key value pair
            var expandoDict = details as IDictionary<string, object>;

            foreach (var i in expandoDict)
            {
                OrderComeIn order = new OrderComeIn()
                {
                    OrderNumber = i.Key,
                    Destination = (AirportAbbr)Enum.Parse(typeof(AirportAbbr), ((dynamic)i.Value).destination, true)
                };

                orders.Add(order);
            }

            //before return the result, sort order list by destination then by order number
            return orders.OrderBy(o => o.Destination).ThenBy(o => o.OrderNumber).ToList();
        }

    }
}
