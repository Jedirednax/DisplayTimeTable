using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Timetable;

namespace DisplayTimetable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            new DocumentHandler();
            InitializeComponent();

            pgDaysDisplay pgDays = new pgDaysDisplay();
            frmDisplay.Content = pgDays;

            var sett = DocumentHandler.sets;
            StatusBarItem statusBarItemPas = new StatusBarItem();

            statusBarItemPas.Content = sett.GetVersionPas();//$"Pas:{sett.PasVersion}({sett.PasVersionDate.Date})";

            StatusBarItem statusBarItemCtt = new StatusBarItem();
            statusBarItemCtt.Content = sett.GetVersionCtt();//$"Pas:{sett.CttVersion}({sett.CttVersionDate.Date})";

            stbDet.Items.Add(statusBarItemPas);
            stbDet.Items.Add(new Separator());
            stbDet.Items.Add(statusBarItemCtt);
        }

        private async void btnCTT_ClickAsync(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dio = new OpenFileDialog();
            Nullable<bool> Got = dio.ShowDialog();
            if(Got == true)
            {
                string path = dio.FileName;
                await DocumentHandler.AddCTT(path);
            }
                frmDisplay.Refresh();
        }
        private async void btnPAS_ClickAsync(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dio = new OpenFileDialog();
            Nullable<bool> Got = dio.ShowDialog();
            if(Got == true)
            {
                string path = dio.FileName;
                await DocumentHandler.AddPAS(path);
            }
                frmDisplay.Refresh();
        }
    }
}