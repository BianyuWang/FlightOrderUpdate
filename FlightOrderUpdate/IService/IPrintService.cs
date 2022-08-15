using FlightOrderSimulationConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightOrderUpdate.IService
{
    public interface  IPrintService
    {
        void PrintOrder(OrderDetailDisplay order);
        void PrintMenu();

        void PrintScheduleDetail();

        void PrintScheduleDetail(int dayOn);
        void PrintFlightScheduleByFlight();
    }
}
