using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.SAPINT
{
    public static class SAPDestitination
    {
        public const string SapDestinationName = "SAP_DESTINATION";
        public static SAPDestinationConfig DestinationConfig = new SAPDestinationConfig();
        public static void Init(string host, string client, string number)
        {
            RfcDestinationManager.RegisterDestinationConfiguration(DestinationConfig);
            DestinationConfig.AddOrEditDestination(
                    SAPDestitination.SapDestinationName,
                    number,
                    client,
                    host
               );
        }

        public static RfcCustomDestination CreateCustomDestination(string username, string password)
        {
            RfcDestination destionation = RfcDestinationManager.GetDestination(SAPDestitination.SapDestinationName);
            if (destionation != null)
            {
                RfcCustomDestination customDestination = destionation.CreateCustomDestination();
                customDestination.User = username;
                customDestination.Password = password;
                return customDestination;
            }
            return null;
        }
    }
}
