using System;

namespace Historic.API.Entities
{
    public class Evento
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string Lang { get; set; }
        public string Granularity { get; set; }
    }

    public class Result
    {
        public long Count { get; set; }
        public Evento[] Events { get; set; }
    }
}
