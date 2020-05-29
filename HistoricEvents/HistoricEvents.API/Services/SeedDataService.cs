using HistoricEvents.API.Data;
using System;
using System.Threading.Tasks;

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
            
            await context.SaveChangesAsync();
        }
    }
}
