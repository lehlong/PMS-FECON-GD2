using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using SMO.Service.CM;

namespace SMO.Hubs
{
    public class NotificationHub : Hub
    {
        public void NotifyIsViewed(string userName)
        {
            var service = new NotifyService();
            service.UpdateNotifyIsViewed(userName);
            Clients.Group(userName).NotifyIsViewed();
        }

        public void NotifyIsReaded(string pkId, string userName)
        {
            var service = new NotifyService();
            service.UpdateNotifyIsReaded(pkId);
            Clients.Group(userName).NotifyIsReaded(pkId);
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            Groups.Add(Context.ConnectionId, name);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;
            try
            {
                Groups.Add(Context.ConnectionId, name);
            }
            catch { }
            return base.OnReconnected();
        }
    }
}