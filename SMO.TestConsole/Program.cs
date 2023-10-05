using SMO.SAPINT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAP.Middleware.Connector;
using SMO.Core.Entities;
using SMO;

namespace SMO.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<SMO.AppCode.Class.TimePeriod> timePeriods = SMOUtilities.GenerateTimeWeek(new DateTime(2021,2,12), new DateTime(2022,9,12));

            var startDate = new DateTime(2021, 12, 2);
            var endDate = new DateTime(2022, 3, 12);

            int days = startDate.DayOfWeek - DayOfWeek.Monday;
            DateTime tmpStartDate = startDate.AddDays(-days).Date;
            while (tmpStartDate < endDate)
            {
                if (tmpStartDate < startDate)
                {
                    Console.Write(startDate + " - ");
                }
                else
                {
                    Console.Write(tmpStartDate + " - ");
                }

                if (tmpStartDate.AddDays(6) > endDate)
                {
                    Console.Write(endDate);
                }
                else
                {
                    Console.Write(tmpStartDate.AddDays(6));
                }
                tmpStartDate = tmpStartDate.AddDays(7);
                Console.WriteLine("");
            }

            Console.ReadLine();
        }
    }
}
