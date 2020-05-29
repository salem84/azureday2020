using Historic.API.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HistoricEvents.API.Services
{
    public interface IEventiService
    {
        List<Evento> Read();
    }


    public class EventiService : IEventiService
    {

        private List<object> servizi;
        private string jsonObj;
        //private List<Evento> dati;

        public EventiService(List<object> servizi)
        {
            this.servizi = servizi;
            jsonObj = File.ReadAllText("Data/italian_events.json");
            
        }

        public List<Evento> Read()
        {
            var list = new List<Evento>();
            //list.Add(new Evento() { firstname = "Dale", lastname = "Bingham", title = "Mr.", middlename = "E." });
            //list.Add(new People() { firstname = "Richard", lastname = "Cranium", title = "Mr.", middlename = "B." });
            //list.Add(new People() { firstname = "Christine", lastname = "Smith", title = "Ms.", middlename = "L." });
            //list.Add(new People() { firstname = "Jessica", lastname = "Lampard", title = "Mrs.", middlename = "Q." });

            return list;
        }
    }
}
