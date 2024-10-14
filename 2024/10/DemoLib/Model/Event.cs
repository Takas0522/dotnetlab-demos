using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.Model
{
    public class Event
    {
        public string Subject { get; set; } = "";
        public Body? Body { get; set; }
        public DateTimeSet? Start { get; set; }
        public DateTimeSet? End { get; set; }

    }

    public class Body
    {
        public string ContentType { get; set; } = "";
        public string Content { get; set; } = "";
    }

    public class DateTimeSet
    {
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string TimeZone { get; set; } = "";
    }
}
