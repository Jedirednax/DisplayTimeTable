using System.Collections.ObjectModel;

namespace Timetable
{
    public class Module
    {
        public static List<Module> Modules = new List<Module>();
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public string LectureName { get; set; }

        public Module(string moduleCode, string moduleName, string lectureName)
        {
            ModuleCode = moduleCode;
            ModuleName = moduleName;
            LectureName = lectureName;
        }

        public static Module? FetchModule(string moduleName)
        {
            return Modules.Find(x => x.ModuleCode == moduleName);
        }

        public Module? this[string modCode]
        {
            get
            {
                return Modules.Find(x => x.ModuleCode == modCode);
            }
        }

        public override string? ToString() => ModuleCode;
    }
}
