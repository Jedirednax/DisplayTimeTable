using System.Diagnostics;
using System.Windows.Controls;
using Timetable;

namespace DisplayTimetable
{
    /// <summary>
    /// Interaction logic for pgDaysDisplay.xaml
    /// </summary>
    public partial class pgDaysDisplay : Page
    {
        public static List<Day> days = new List<Day>();
        public pgDaysDisplay()
        {
            DateTime dateTime = Convert.ToDateTime("07/01/2024");

            Pas.AllPas = DocumentHandler._FetchPas();
            ModuleSession.Sessions = DocumentHandler._FetchModules();

            List<Pas>.Enumerator ps = Pas.AllPas.GetEnumerator();
            List<ModuleSession>.Enumerator ms = ModuleSession.Sessions.GetEnumerator();

            ps.MoveNext();
            ms.MoveNext();

            InitializeComponent();
            for(int i = 0; i < 365; i++)
            {
                Day newDay = new Day(dateTime.AddDays(i));
                Debug.WriteLineIf(newDay.Date >= Convert.ToDateTime("01/07/2024"), $"Date: {newDay.Date}:");
                Debug.IndentSize++;
                while(newDay.Date == ps.Current.AssessmentDate.Date)
                {
                    Debug.IndentLevel++;
                    Debug.Write(ps.Current);
                    newDay.Assingments.Add(ps.Current);
                    ps.MoveNext();
                    Debug.IndentLevel--;
                }
                //Debug.WriteLine(ps.Current);
                while(ms.Current !=null && newDay.Date == ms.Current.SessionDateTime.Date)
                {

                    Debug.IndentLevel++;
                    Debug.WriteLine(ms.Current);
                    newDay.Classes.Add(ms.Current);
                    ms.MoveNext();
                    Debug.IndentLevel--;

                }
                Debug.IndentSize--;

                calDis.Items.Add(newDay);
            }

            //Columns = 7;
            //FirstColumn = (int)days.First().Date.DayOfWeek;
            /*
            foreach(Day day in days)
            {
                Grid t = DayFormat(day, 200);
                Grid.SetColumn(t, (int)day.Date.DayOfWeek);
                //Children.Add(t);
            }
            */
            //calDis.ItemsSource = days;
            //calDis.FirstColumn = (int)dateTime.DayOfWeek;
        }
    }
}
