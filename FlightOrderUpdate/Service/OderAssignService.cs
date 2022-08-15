using FlightOrderSimulationConsole.Models;
using FlightOrderUpdate.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightOrderUpdate.Service
{
    public class OderAssignService : IOderAssignService
    {
        private readonly IDataAccessService _dataAccessService;
        public OderAssignService(IDataAccessService dataAcessService)
        {
            _dataAccessService = dataAcessService;
        }



        private List<OrderDetailDisplay> AddOrderToFlights(List<OrderComeIn> orders, int ForHowManyDays)
        {
            //list of filght schedule, (day X , flight with their capacity)
            List<FlightSchedule> FlightSchedules = new List<FlightSchedule>();

            //order details which should be return at the end
            List<OrderDetailDisplay> orderDetails = new List<OrderDetailDisplay>();

            //1.count numbers of orders by destination, Quantity will be used to
            //  calculate how many orders have not been processed 
            var orderDestCount = orders.GroupBy(l => l.Destination)
                    .Select(cl => new OrderGroup
                    {
                        Destination = cl.First().Destination,
                        Quantity = cl.Count(),
                    }).ToList();

            //2. Schedule daily flights according to user input
            for (int i = 0; i < ForHowManyDays; i++)
            {
                FlightSchedule schedule = new FlightSchedule();
                schedule.DepartureDate = i + 1;
                schedule.Flights = _dataAccessService.GetFlightScheduleByDay(i % 2).Flights;

                //Arrange orders for each flight
                foreach (var flight in schedule.Flights)
                {
                    //get destination of flight
                    AirportAbbr destination = _dataAccessService.GetAirLineById(flight.AirLineId).ArriveAirport;

                    //Find the order of the same destination
                    var orderGroup = orderDestCount.Where(orders => orders.Destination == destination).FirstOrDefault();

                    if (orderGroup != null)
                    {
                        //assign the remaining orders to the flight

                        //Get the number of orders to be shipped
                        int arrangedOrderNumber = orderGroup.Quantity > 20 ? 20 : orderGroup.Quantity;

                        //loading orders
                        var ordersOnBord = orders.Where(o => o.Destination == destination).Skip(i * 20).Take(arrangedOrderNumber).ToList();
                        foreach (var order in ordersOnBord)
                        {
                            OrderDetailDisplay orderDetail = new OrderDetailDisplay()
                            {
                                AirLineId = flight.AirLineId,
                                OrderId = order.OrderNumber,
                                DepartureDate = schedule.DepartureDate,
                                FlightId = flight.FlightId
                            };

                            orderDetails.Add(orderDetail);
                            flight.Loading++;
                        }

                        //Get the  number of orders that are not on borad.
                        orderGroup.Quantity -= arrangedOrderNumber;

                    }



                }
                FlightSchedules.Add(schedule);

            }

            //After all the flights are arranged, get the remaining orders
            foreach (var orderGroup in orderDestCount)
            {
                if (orderGroup.Quantity > 0)
                {
                    var orderNoOnbord = orders.Where(o => o.Destination == orderGroup.Destination).TakeLast(orderGroup.Quantity).ToList();
                    foreach (var order in orderNoOnbord)
                    {
                        OrderDetailDisplay ord = new OrderDetailDisplay()
                        {
                            OrderId = order.OrderNumber
                        };
                        orderDetails.Add(ord);
                    }

                }

            }

            //sort the order detail by order number before returning 
            return orderDetails.OrderBy(o => o.OrderId).ToList();
        }



    
        public List<OrderDetailDisplay> ProcessingOrderComeIn(int forHowManyDays)
        {
          var ordersComeIn = _dataAccessService.ReadOrderFromJson();
            return AddOrderToFlights(ordersComeIn, forHowManyDays);
        }

       
    }
}
