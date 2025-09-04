using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Timetable
{
    public class Day
    {
        public List<ModuleSession> Classes { get; set; } = new List<ModuleSession>();
        public List<Pas> Assingments { get; set; } = new List<Pas>();

        public DateTime Date { get; set; }

        public Day() { }
        public Day(DateTime date)
        {
            Date = date;
        }

        public override string? ToString() => $"{Date.Day}\n";
    }
}

