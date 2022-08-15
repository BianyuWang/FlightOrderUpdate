using FlightOrderSimulationConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightOrderUpdate.IService
{
    public interface  IOderAssignService
    {
        List<OrderDetailDisplay> ProcessingOrderComeIn(int forHowManyDays);

    }
}
