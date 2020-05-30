using Historic.API.Entities;
using HistoricEvents.API.Data;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace HistoricEvents.API.Services
{
    public interface ISeedDataService
    {
        Task Initialize(EventsDbContext context);
    }

    public class SeedDataService : ISeedDataService
    {
        public async Task Initialize(EventsDbContext context)
        {
            if(context.Eventi.Any())
            {
                return;
            }

            var jsonObj = File.ReadAllText("Data/italian_events.json");
            var dati = JsonConvert.DeserializeObject<Result>(jsonObj).Events;

            dati.ForEach(x =>
            {
                context.Eventi.Add(x);
            });
            await context.SaveChangesAsync();
        }
    }
}
