using System.Diagnostics;
using System.Globalization;

namespace Timetable
{
    public class Pas
    {
        public static List<Pas> AllPas = new List<Pas>();

        public string   SISQualificationCode    { get; set; }
        public string   ProgrammeCode           { get; set; }
        public string   ModuleCode              { get; set; }
        public string   ModuleName              { get; set; }
        public string   AssessmentName          { get; set; }
        public string   Semester                { get; set; }
        public DateTime AssessmentDate          { get; set; }
        public string   AssessmentTime          { get; set; }
        public string   SafeAssignRequirement   { get; set; }
        public string   Formative               { get; set; }


        public Pas() { }

        public Pas(string sISQualificationCode, string programmeCode, string moduleCode, string moduleName, string assessmentName, string semester, DateTime assessmentDate, string assessmentTime, string safeAssignRequirement, string formative)
        {
            SISQualificationCode = sISQualificationCode;
            ProgrammeCode = programmeCode;
            ModuleCode = moduleCode;
            ModuleName = moduleName;
            AssessmentName = assessmentName;
            Semester = semester;
            AssessmentDate = assessmentDate;
            AssessmentTime = assessmentTime;
            SafeAssignRequirement = safeAssignRequirement;
            Formative = formative;
        }

        /*public Pas? this[DateTime date]
        {
            get
            {
                return AllPas.Find(p=> p.AssessmentDate.Date == date.Date);
            }
        }*/

        public override string? ToString()
        {
            return $"{ModuleCode} : {AssessmentDate.Date}\n{AssessmentName} ";
        }
    }
}