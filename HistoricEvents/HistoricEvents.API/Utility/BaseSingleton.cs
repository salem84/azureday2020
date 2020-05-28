using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricEvents.API.Utility
{
    public abstract class BaseSingleton<TSingleton> where TSingleton : class
    {
        public static TSingleton Instance
        {
            get
            {
                return DependencyInjector.GetInstance<TSingleton>(_instanceFactory);
            }
        }

        private static TSingleton _instanceFactory(Type type)
        {
            return Activator.CreateInstance<TSingleton>();
        }
    }
}
