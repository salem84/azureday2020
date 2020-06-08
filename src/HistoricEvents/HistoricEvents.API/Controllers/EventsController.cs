using API.Filters;
using Historic.API.Entities;
using Historic.API.Services;
using HistoricEvents.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using peopleapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace HistoricEvents.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly EventsDbContext _context;

        public EventsController(EventsDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// GET The list of events
        /// </summary>
        /// <returns>
        /// HTTP Status showing it was found or that there is an error. And the list of events records.
        /// </returns>
        /// <response code="200">Returns the list of events records</response>
        [HttpGet]
        [ServiceFilter(typeof(ErrorSimulatorFilter))]
        public async Task<ActionResult<IEnumerable<Evento>>> Get()
        {

            var listWrong = ServiceProvider.EventiService.Read();

            var total = _context.Eventi.Count();
            var num = new Random().Next(0, 10);

            var list = new List<Evento>();
            for (int i = 0; i < num; i++)
            {
                var index = new Random().Next(0, total - 1);
                var el = await _context.Eventi.SingleAsync(x => x.Id == index);
                list.Add(el);
            }

            return Ok(list);
        }

        /// <summary>
        /// GET a event record
        /// </summary>
        /// <returns>
        /// HTTP Status showing it was found or that there is an error. 
        /// </returns>
        /// <response code="200">Returns the event record</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Evento>> Get(string id)
        {
            var p = new Evento();
           
            return Ok(p);
        }

    }
}
