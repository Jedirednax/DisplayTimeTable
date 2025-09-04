using System.Diagnostics;
using System.Text.Json;
using Tabula;
using Tabula.Detectors;
using Tabula.Extractors;
using UglyToad.PdfPig;

namespace Timetable
{
    //eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjM5MjYzNDc0NCwiYWFpIjoxMSwidWlkIjo1NzA1NzgxMCwiaWFkIjoiMjAyNC0wOC0wNFQxMDo1MzozNC44NTRaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6MjE4MTU2NzAsInJnbiI6ImV1YzEifQ.26Kf6RPWJtTRb6pojfDrcNAzlO5vhnFlw2TkiTdQ1IQ
    public record Settings
    {

        public string PasVersion { get; set; }
        public DateTime PasVersionDate { get; set; }
        public string CttVersion { get; set; }
        public DateTime CttVersionDate { get; set; }

        public Settings()
        {

        }
        public string GetVersionCtt()
        {
            return $"TimeTable: V{CttVersion.Substring(CttVersion.Length - 5, 1)}({CttVersionDate.ToShortDateString()})";
        }
        public string GetVersionPas()
        {
            return $"Pas: V{PasVersion.Substring(PasVersion.Length - 5, 1)}({PasVersionDate.ToShortDateString()})";
        }
        public override string ToString()
        {
            return $"Pas: {PasVersion}({PasVersionDate.Date})\t Timtable: {CttVersion}({CttVersionDate.Date})";
        }
    }

    public class DocumentHandler
    {
        private static DirectoryInfo path = new DirectoryInfo("C:\\Users\\jedix\\Desktop");
        private static string ModCode = "BCAD0701";

        private static string basePath      = "C:\\Users\\jedix\\Desktop";
        private static string storagePath   = Directory.GetCurrentDirectory() + "\\Docs";

        private static string pasFileName   = "\\ProgramAssessmentShedule.csv";
        private static string cttFileName   = "\\SemesterTimeTable.csv";
        private static string modFileName   = "\\Modules.csv";

        private static string cttVersion    = "2";
        private static string pasVersion    = "6";

        private static string cttPathPDF   = $"\\IIE-BCA3-GR03-S2 Timetable V{cttVersion}.pdf";
        private static string pasPathPDF    = $"\\IIE VC BCAD0701 Year 3 Programme Assessment Schedule 2024 V{pasVersion}.pdf";

        private static string SettingsStorage   = $"\\Settings.json";

        private static char[] _Delim    = {',', ';'};
        public static Settings sets;
        public DocumentHandler()
        {
            DirectoryInfo dir = new DirectoryInfo(storagePath);
            if(!dir.Exists)
            {
                dir.Create();
            }

            if(!File.Exists(storagePath + SettingsStorage))
            {
                //setDir.Create();
                File.Create(storagePath + pasFileName);
                File.Create(storagePath + cttFileName);
                File.Create(storagePath + modFileName);
                File.Create(storagePath + SettingsStorage);
            }
            else
            {
                _TempPop();
            }
            try
            {
                Pas.AllPas = (List<Pas>)_FetchPas().OrderBy(x => x.AssessmentDate);
                ModuleSession.Sessions = (List<ModuleSession>)_FetchModules().OrderBy(x => x.SessionDateTime.Date);
            }
            catch { }
            try
            {
                sets = new Settings();
                ReadSettings();
            }
            catch { }
        }
        private static void _TempPop()
        {
            Module.Modules.Add(new Module("APDS7311 ", "Application Development Security", "Molokomme,Itumeleng"));
            Module.Modules.Add(new Module("OPSC7312 ", "Open Source Coding Intermediate ", "Moses Olaifa       "));
            Module.Modules.Add(new Module("PROG7312 ", "Programming 3B                  ", "Francois Wessels   "));
            Module.Modules.Add(new Module("XBCAD7319", "Work Integrated Learning        ", "Khoza, Lucas       "));
        }
        public static async Task AddCTT(string pdfPath)
        {
            ReadSettings();
            string nam =Path.GetFileName(pdfPath);
            sets.CttVersion = nam;
            sets.CttVersionDate = DateTime.Now;
            WriteSettings();
            _WriteNewCttCSV(pdfPath);
            //ModuleSession.Sessions.Clear();
            ModuleSession.Sessions = _FetchModules();
        }
        public static async Task AddPAS(string fullPath)
        {
            ReadSettings();
            string nam = Path.GetFileName(fullPath);
            sets.PasVersion = nam;
            sets.PasVersionDate = DateTime.Now;
            WriteSettings();
            _WriteNewPasCSV(fullPath);
            //Pas.AllPas.Clear();
            Pas.AllPas = _FetchPas();
        }

        public static async Task ReadSettings()
        {
            DirectoryInfo dir = new DirectoryInfo(storagePath + SettingsStorage);

            using(Stream sr = File.OpenRead(dir.FullName))
            {
                sets = JsonSerializer.Deserialize<Settings>(sr);
            }
        }

        public static async Task WriteSettings()
        {
            DirectoryInfo dir = new DirectoryInfo(storagePath + SettingsStorage);

            using(StreamWriter sr = new StreamWriter(dir.FullName))
            {
                sr.WriteLine(JsonSerializer.Serialize(sets));
            }
        }
        private static List<Table> _PdfTableExtract(string passedDoc)
        {
            using(PdfDocument pdf = PdfDocument.Open(passedDoc))
            {
                ObjectExtractor tableGetter = new ObjectExtractor(pdf);

                List<Table> tables = new List<Table>();

                for(int i = 1; i <= pdf.NumberOfPages; i++)
                {
                    PageArea page = tableGetter.ExtractPage(i);

                    SimpleNurminenDetectionAlgorithm detector = new ();
                    List<TableRectangle> regions = detector.Detect(page);
                    IExtractionAlgorithm ea = new SpreadsheetExtractionAlgorithm ();
                    PageArea g;
                    if(i == 1 && passedDoc.Contains(cttPathPDF))
                    {
                        g = page.GetArea(top: 50, left: 50, bottom: 700, right: 2000);
                    }
                    else
                    {
                        g = page.GetArea(regions[0].BoundingBox);
                    }
                    tables.AddRange(ea.Extract(g));
                }
                return tables;
            }
        }

        private static List<List<string>> _PdfExportConvert(string filePath)
        {
            List<Table> newTable = _PdfTableExtract(filePath);
            List<List<string>> result = new List<List<string>>();

            foreach(Table tb in newTable)
            {
                foreach(IReadOnlyList<Cell>? row in tb.Rows)
                {
                    List<string> line = new List<string>();
                    foreach(Cell? cell in row)
                    {
                        Debug.WriteLine(cell.GetText());
                        line.Add(cell.GetText());
                    }
                    result.Add(line);
                }
            }
            return result;
        }

        private static List<Pas> _PasFormat(string filePath)
        {
            List<Pas> assingments = new List<Pas>();

            foreach(List<string> line in _PdfExportConvert(filePath))
            {
                if(line[0] == ModCode)
                {
                    Pas pas = new Pas();

                    pas.SISQualificationCode = line[0];
                    pas.ProgrammeCode = line[1];
                    pas.ModuleCode = line[2];
                    pas.ModuleName = line[3];
                    pas.AssessmentName = line[4];
                    pas.Semester = line[5];
                    pas.AssessmentTime = line[7];
                    pas.SafeAssignRequirement = line[8];
                    pas.Formative = line[9];

                    try
                    {
                        pas.AssessmentDate = Convert.ToDateTime(line[6]);
                    }
                    catch
                    {
                        pas.AssessmentDate = DateTime.MinValue;
                    }
                    assingments.Add(pas);
                }
            }
            return assingments;
        }

        private static List<ModuleSession> _ModuleSessionFormat(string filePath)
        {
            List<ModuleSession> session = new List<ModuleSession>();
            List<string> dates = new List<string>();
            foreach(List<string> week in _PdfExportConvert(filePath))
            {
                if(week[0].StartsWith("w"))
                {
                    dates = week;
                }
                else
                {
                    string time = week[0];
                    for(int i = 1; i < week.Count; i++)
                    {
                        if(week[i].Length > 0)
                        {
                            try
                            {
                                string item = week[i];

                                string subject = item.Substring(0,9);
                                string venue = item.Substring(9);

                                DateTime dateOfDay = DateTime.MinValue;
                                dateOfDay = Convert.ToDateTime(dates[i]);

                                int intHour = Convert.ToInt32(time.Substring(0, 2));

                                dateOfDay = dateOfDay.AddHours(intHour);

                                Module? module = Module.FetchModule(subject);

                                if(module != null)
                                {
                                    ModuleSession newMod = new ModuleSession(module,venue,dateOfDay);
                                    session.Add(newMod);
                                }
                            }
                            catch(Exception ex)
                            {
                                //Debug.Write(ex);
                            }
                        }
                    }
                }
            }
            return session;
        }

        private static void _WriteNewCttCSV(string PDFpath)
        {
            File.WriteAllText(storagePath + cttFileName, String.Empty);
            using(StreamWriter outputFile = new StreamWriter(storagePath + cttFileName))
            {

                foreach(ModuleSession les in _ModuleSessionFormat(PDFpath).OrderBy(x => x.SessionDateTime.Date))
                {
                    outputFile.WriteLine(_CttWriteLine(les));
                }
            }
        }
        private static void _WriteNewPasCSV(string PDFpath)
        {
            File.WriteAllText(storagePath + pasFileName, String.Empty);
            using(StreamWriter outputFile = new StreamWriter(storagePath + pasFileName))
            {
                foreach(Pas pas in _PasFormat(PDFpath).OrderBy(x => x.AssessmentDate.Date))
                {
                    if(pas.AssessmentDate > DateTime.MinValue)
                    {
                        outputFile.WriteLine(_PasWriteLine(pas));
                    }
                }
            }
        }

        public static List<Pas> _FetchPas()
        {
            List<Pas> assingments = new List<Pas>();
            DirectoryInfo dir = new DirectoryInfo(storagePath + pasFileName);

            using(StreamReader sr = new StreamReader(dir.FullName))
            {
                while(!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split(_Delim);
                    if(line[0] == ModCode)
                    {
                        try
                        {

                            Pas pas = new Pas();

                            pas.SISQualificationCode = line[0];
                            pas.ProgrammeCode = line[1];
                            pas.ModuleCode = line[2];
                            pas.ModuleName = line[3];
                            pas.AssessmentName = line[4];
                            pas.Semester = line[5];

                            pas.AssessmentDate = Convert.ToDateTime(line[6]);

                            pas.AssessmentTime = line[7];
                            pas.SafeAssignRequirement = line[8];
                            pas.Formative = line[9];

                            assingments.Add(pas);
                        }
                        catch
                        {
                            //pas.AssessmentDate = DateTime.MinValue;
                        }
                    }
                }
            }

            return assingments;
        }

        public static List<ModuleSession> _FetchModules()
        {
            List<ModuleSession> lesson = new List<ModuleSession>();
            DirectoryInfo dir = new DirectoryInfo(storagePath + cttFileName);

            StreamReader sr = new StreamReader(dir.FullName);
            List<string> m = new List<string>();

            foreach(Module y in Module.Modules)
            {
                m.Add(y.ModuleCode);
            }

            using(sr)
            {
                while(!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine().Split(_Delim);
                    if(m.Contains(line[0]))
                    {
                        string ModCode = Convert.ToString(line[0]);
                        Module? t = Module.FetchModule(ModCode);
                        DateTime dat = Convert.ToDateTime(line[2]);
                        string venu = Convert.ToString(line[2]);
                        ModuleSession session = new ModuleSession(t, venu,dat);

                        lesson.Add(session);
                    }
                }
            }
            return lesson;
        }

        private static string _PasWriteLine(Pas item)
        {
            return
                $"{item.SISQualificationCode}," +
                $"{item.ProgrammeCode}," +
                $"{item.ModuleCode}," +
                $"{item.ModuleName}," +
                $"{item.AssessmentName}," +
                $"{item.Semester}," +
                $"{item.AssessmentDate}," +
                $"{item.AssessmentTime}," +
                $"{item.SafeAssignRequirement}," +
                $"{item.Formative}";
        }
        private static string _CttWriteLine(ModuleSession module)
        {
            return
                $"{module.Module.ModuleCode}, " +
                $"{module.Venue}, " +
                $"{module.SessionDateTime.ToString("dd/MM/yyyy HH:MM:ss")}";
        }
    }
}
