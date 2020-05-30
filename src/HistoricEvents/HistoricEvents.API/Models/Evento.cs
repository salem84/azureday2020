using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Historic.API.Entities
{
    public class Evento
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string Lang { get; set; }
        public string Granularity { get; set; }
    }

    public class Result
    {
        public long Count { get; set; }
        public List<Evento> Events { get; set; }
    }

    public class JsonData
    {
        public Result Result { get; set; }
    }
}
