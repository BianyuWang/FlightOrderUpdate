using FlightOrderSimulationConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightOrderUpdate.IService
{
    public interface IDataAccessService
    {

         AirLine GetAirLineById(int id);
         List<AirLine> GetAirLines(int id);
         List<FlightSchedule> GetFlightScheduleList();
         FlightSchedule GetFlightScheduleByDay(int dayAt);

         List<OrderComeIn> ReadOrderFromJson(); 

    }
}
