using FlightOrderSimulationConsole.Models;
using FlightOrderUpdate.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightOrderUpdate.Service
{
    public class PrintService : IPrintService
    {
        private readonly IDataAccessService _dataAccessService;
        public PrintService(IDataAccessService dataAccessService)
        {
            _dataAccessService = dataAccessService;
        }



        private void PrintDetail(FlightSchedule schedule, bool isByFlight = false, int day = 0)
        {
            day = day == 0 ? schedule.DepartureDate : day;

            if (!isByFlight)
                Console.WriteLine($"Day {day}");

            schedule.Flights = schedule.Flights.OrderBy(f => f.FlightId).ToList();
         
            foreach (var flight in schedule.Flights)
            {
                PrintFlight(flight, isByFlight, day);
            }
        }

        private void PrintFlight(Flight flight, bool isByFlight = false, int day = 0)
        {
            var airline = _dataAccessService.GetAirLineById(flight.AirLineId);
         
            if (airline != null)
            {

                var msg = $"Flight {flight.FlightId} :" +
                   $" {(City)airline.DepartAirport}({airline.DepartAirport})" +
                   $"to " +
                   $"{(City)airline.ArriveAirport}({airline.ArriveAirport})";

                if (isByFlight)
                    msg = $"Flight {flight.FlightId} :" +
                       $" Departure :{airline.DepartAirport}," +
                     $" Arrival :{airline.ArriveAirport},  day: {day}";

                Console.WriteLine(msg);
            }
        }

        public void PrintFlightScheduleByFlight()
        {
            List<Flight> flights = new List<Flight>();

            foreach (var flightList in _dataAccessService.GetFlightScheduleList())
            {
                flightList.Flights.ForEach(item => flights.Add(item));
            }


         //var query = _dataAccessService.GetFlightScheduleList()
         //.OrderBy(x => x.Flights.Min(s => s.FlightId))
         //.ToList();
           
            flights = flights.OrderBy(item => item.FlightId).ToList();

            foreach (var flight in flights)
            {
                int dayScheduled = 0;
                foreach (var schedule in _dataAccessService.GetFlightScheduleList())

                {
                    if (schedule.Flights.Any(d => d.FlightId == flight.FlightId))
                    {
                        dayScheduled = schedule.DepartureDate;
                        break;
                    }
                }

                PrintFlight(flight, isByFlight: true, day: dayScheduled);
            }
        }

        public void PrintMenu()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Please select from the menu");
            Console.WriteLine("1// Load Flight Schedule");
            Console.WriteLine("2// Check Schedule by day");
            Console.WriteLine("3// List out flight schedule");
            Console.WriteLine("4// Give a specific number of days to schedule the order");
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("5// Exit");
        }

        public void PrintOrder(OrderDetailDisplay order)
        {
            if (order.DepartureDate != 0)
            {
                AirLine airline = _dataAccessService.GetAirLineById(order.AirLineId);
                Console.WriteLine($"order : {order.OrderId}, FlightNumber : {order.FlightId}, " +
                    $"departure : {(City)airline.DepartAirport}, arrival : {(City)airline.ArriveAirport}, day {order.DepartureDate}");
            }
            //if order has not yet been scheduled
            else
                Console.WriteLine($"order : {order.OrderId}, FlightNumber : not Scheduled ");

        }



        public void PrintScheduleDetail()
        {
          var scheduleList = _dataAccessService.GetFlightScheduleList();
            foreach (var schedule in scheduleList)
            {
                PrintDetail(schedule);
            }

        }


        public void PrintScheduleDetail(int dayOn)
        {
            var schedule = _dataAccessService.GetFlightScheduleByDay((dayOn+1) % 2);
            PrintDetail(schedule,day:dayOn);
        }


    }
}
