using Historic.API.Entities;
using Historic.API.Services;
using Microsoft.AspNetCore.Mvc;
using peopleapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HistoricEvents.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        /// <summary>
        /// GET The list of events
        /// </summary>
        /// <returns>
        /// HTTP Status showing it was found or that there is an error. And the list of events records.
        /// </returns>
        /// <response code="200">Returns the list of events records</response>
        [HttpGet]
        public ActionResult<IEnumerable<Evento>> Get()
        {
            var list = new List<Evento>();

            list = ServiceProvider.EventiService.Read();

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
        public ActionResult<Evento> Get(string id)
        {
            var p = new Evento();
           
            return Ok(p);
        }

    }
}
