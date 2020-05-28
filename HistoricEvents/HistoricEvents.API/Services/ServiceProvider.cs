using HistoricEvents.API.Services;
using HistoricEvents.API.Utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Historic.API.Services
{
    public sealed class ServiceProvider : BaseSingleton<ServiceProvider>
    {
        private Timer _timer;

        private IEventiService _istanzaEventiService;

        public static IEventiService EventiService
        {
            get { return Instance._istanzaEventiService; }
        }

        public ServiceProvider()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                var configuration = builder.Build();

                List<object> servizi = new List<object>();
                _istanzaEventiService = GetInstance<IEventiService>(servizi);

                int sleep = (int)(configuration.GetValue<decimal>("TimerConfigurationMinutes") * 60 * 1000);
                _timer = new Timer(sleep);
                _timer.Elapsed += _timer_Elapsed;
                _timer.Start();
            }
            catch (Exception ex)
            {

            }
        }

        private T GetInstance<T>(List<object> servizi) where T : class
        {
            object istanzaServizio = null;

            var fullName = typeof(T).FullName;
            if (fullName == typeof(IEventiService).FullName)
            {
                istanzaServizio = new EventiService(servizi);
            }
            else
            {
                throw new Exception(string.Format("Il service '{0}' non esiste.", typeof(T).FullName));
            }
           

            return (T)istanzaServizio;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DependencyInjector.Reset();
        }
    }
}
