using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Timetable
{
    public class ModuleSession
    {
        //public static List<ModuleSession> Sessions = CustomUtilities.CustomUtilities.Sessions;
        public static List<ModuleSession> Sessions = new List<ModuleSession>();

        public Module Module { get; set; }
        public string Venue { get; set; }
        public DateTime SessionDateTime { get; set; }

        public ModuleSession() { }
        public ModuleSession(Module module, string venu, DateTime sessionTime)
        {
            Module = module;
            Venue = venu;
            SessionDateTime = sessionTime;
        }

        public List<ModuleSession> this[DateTime date]
        {
           
            get
            {
                return Sessions.FindAll(ms=> ms.SessionDateTime.Date == date.Date);
            }
        }
        public override string ToString() => $"{SessionDateTime.Hour}: {Module.ToString()}:{Venue}";
    }
}
