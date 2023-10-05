using NHibernate.Event;
using NHibernate.Persister.Entity;
using SMO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SMO.Repository.Common
{
    public class NHListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        public bool OnPreInsert(PreInsertEvent @event)
        {
            var baseEntity = @event.Entity as BaseEntity;
            if (baseEntity == null)
                return false;
            var time = DateTime.Now;
            Set(@event.Persister, @event.State, "CREATE_DATE", time);
            baseEntity.CREATE_DATE = time;
            return false;
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var baseEntity = @event.Entity as BaseEntity;
            if (baseEntity == null)
                return false;
            var time = DateTime.Now;
            Set(@event.Persister, @event.State, "UPDATE_DATE", time);
            baseEntity.UPDATE_DATE = time;
            return false;
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            return false;
        }

        private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}
