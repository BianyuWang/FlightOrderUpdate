// See https://aka.ms/new-console-template for more information
using FlightOrderSimulationConsole.Models;
using FlightOrderUpdate.IService;
using FlightOrderUpdate.Service;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var serviceProvider = new ServiceCollection()

           .AddSingleton<IPrintService, PrintService>()
           .AddSingleton<IOderAssignService, OderAssignService>()
           .AddSingleton<IDataAccessService, DataAccessService>()
           .BuildServiceProvider();

var printer = serviceProvider.GetService<IPrintService>();

var orderService = serviceProvider.GetService<IOderAssignService>();

int input = 0,dayInput=0;
bool isNumber;
List<OrderDetailDisplay> orderDetails;

while (input != 5)
{
    printer.PrintMenu();

    //Parse input, Check if the inputs are as required
    isNumber = Int32.TryParse(Console.ReadLine(), out input);

    //if not meet the requirement, let user choose from the beginning 
    if (!isNumber || input > 5)
    {
        Console.WriteLine("Please enter a number from 1 to 5");
        continue;
    }

    //Access to functions based on user selection
    switch (input)
    {
        case 1:
           printer.PrintScheduleDetail();
            break;

        case 2:
            Console.WriteLine("Please enter a specific day to check the Schedule");

            isNumber = Int32.TryParse(Console.ReadLine(), out dayInput);

            if (isNumber)  printer.PrintScheduleDetail(dayInput);
            else  continue;
            break;

        case 3:
            printer.PrintFlightScheduleByFlight();
            break;

        case 4:
            Console.WriteLine("How many days do you want to schedule?(try 1 an 2 to see the differece)");
            isNumber = Int32.TryParse(Console.ReadLine(), out dayInput);
            if (isNumber)
            {   //read orders information from json file.
                orderDetails = orderService.ProcessingOrderComeIn(dayInput);
             

                //display order details
                foreach (var order in orderDetails)
                {
                    printer.PrintOrder(order);

                }

            }
            else
                continue;
            break;
        default:
            break;
    }

}





