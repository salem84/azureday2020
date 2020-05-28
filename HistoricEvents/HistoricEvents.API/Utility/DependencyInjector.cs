using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricEvents.API.Utility
{
    public sealed class DependencyInjector
    {
        private static readonly ConcurrentDictionary<string, Func<Type, object>> _factories = new ConcurrentDictionary<string, Func<Type, object>>();
        private static readonly ConcurrentDictionary<string, object> _instances = new ConcurrentDictionary<string, object>();

        private static bool _resetRequested = false;
        private static readonly object _resetRequestedLock = new object();
        private static readonly object _instancesLock = new object();

        private static void _checkReset()
        {
            if (_resetRequested)
            {
                lock (_resetRequestedLock)
                {
                    if (_resetRequested)
                    {
                        lock (_instancesLock)
                        {
                            _instances.Clear();
                        }

                        _resetRequested = false;
                    }
                }
            }
        }

        public static T GetInstance<T>(Func<Type, T> instanceFactory) where T : class
        {
            _checkReset();

            lock (_instancesLock)
            {
                return _instances.GetOrAdd(typeof(T).FullName, x => instanceFactory(typeof(T))) as T;
            }
        }

        public static T GetInstance<T>() where T : class
        {
            _checkReset();

            lock (_instancesLock)
            {
                return _instances.GetOrAdd(typeof(T).FullName, x =>
                {
                    if (!_factories.ContainsKey(x))
                        throw new Exception(string.Format("Cannot find the factory for the type {0}.", x));
                    else
                        return _factories[x](typeof(T));
                }) as T;
            }
        }

        public static void SetFactory<T>(Func<Type, object> instanceFactory)
        {
            _factories.GetOrAdd(typeof(T).FullName, instanceFactory);
        }

        public static void Reset()
        {
            lock (_resetRequestedLock)
            {
                _resetRequested = true;
            }
        }
    }
}
