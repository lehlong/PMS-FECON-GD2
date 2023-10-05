using SAP.Middleware.Connector;
using System.Collections.Generic;

namespace SMO.SAPINT
{
    public class SAPDestinationConfig : IDestinationConfiguration
    {
        public string UserName;
        public string Password;
        Dictionary<string, RfcConfigParameters> g_destination;
        RfcDestinationManager.ConfigurationChangeHandler g_handler;

        public SAPDestinationConfig()
        {
            g_destination = new Dictionary<string, RfcConfigParameters>();
        }

        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged
        {
            add
            {
                g_handler = value;
            }
            remove
            {
            }
        }

        public bool ChangeEventsSupported()
        {
            return true;
        }

        public RfcConfigParameters GetParameters(string destinationName)
        {
            RfcConfigParameters para;
            g_destination.TryGetValue(destinationName, out para);
            return para;
        }

        public void AddOrEditDestination(string name,
                                          string number,
                                          string client,
                                          string host,
                                          string language = ""
                                        )
        {
            RfcConfigParameters l_para = new RfcConfigParameters();
            l_para.Add(RfcConfigParameters.Name, name);
            l_para.Add(RfcConfigParameters.SystemNumber, number);
            l_para.Add(RfcConfigParameters.Client, client);
            l_para.Add(RfcConfigParameters.AppServerHost, host);
            l_para.Add(RfcConfigParameters.User, this.UserName);
            l_para.Add(RfcConfigParameters.Password, this.Password);
            l_para.Add(RfcConfigParameters.Language, language);
            RfcConfigParameters l_configuration_exist;

            //if a destination of that name existed before, we need to fire a change event
            if (g_destination.TryGetValue(name, out l_configuration_exist))
            {
                g_destination[name] = l_para;
                RfcConfigurationEventArgs eventArgs = new RfcConfigurationEventArgs(RfcConfigParameters.EventType.CHANGED, l_para);
                g_handler(name, eventArgs);
            }
            else
            {
                g_destination[name] = l_para;
            }
        }

        public void RemoveDestination(string name)
        {
            if (name != null && g_destination.Remove(name))
            {
                g_handler(name, new RfcConfigurationEventArgs(RfcConfigParameters.EventType.DELETED));
            }
        }
    }
}