using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Timetable;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Globalization;

namespace DisplayTimetable
{
    
    public class altMonth : DataTemplateSelector
    {
        public override DataTemplate?
        SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement? element = container as FrameworkElement;

            if(element != null && item != null && item is Day)
            {
                Day dayitem = (Day)item;

                if(dayitem.Date == DateTime.Today)
                {
                    return (DataTemplate)element.FindResource("CellToDay");
                }
                else if((dayitem.Date.Month % 2) == 0)
                {
                    return (DataTemplate)element.FindResource("CellOddMonth");
                }
                else
                {
                    return (DataTemplate)element.FindResource("CellEvenMonth");
                }
            }

            return null;
        }
    }
    
    public class customDisplay : ItemsControl, IAddChild
    {
        public customDisplay()
        {
            ItemTemplateSelector = (DataTemplateSelector)FindResource("altMonthObj");
            //ItemTemplate = (DataTemplate)FindResource("cellFor");
        }

        public static Label dateDisplay(DateTime date)
        {
            Label lblDate = new Label();
            if(date.Month % 2 != 0)
            {
                lblDate.Background = Brushes.LightGray;
            }
            else
            {
                lblDate.Background = Brushes.White;
            }
            lblDate.MinHeight = 20;
            lblDate.Content =
                $"{date.DayOfWeek}:\n" +
                $"{date.ToShortDateString()}";
            return lblDate;
        }
    }
    
    public class dayClassesConntrol : ItemsControl
    {
        static dayClassesConntrol()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(dayClassesConntrol), new FrameworkPropertyMetadata(typeof(dayClassesConntrol)));
        }
        public static VirtualizingStackPanel classPanel(List<ModuleSession> Classes)
        {
            VirtualizingStackPanel ClassesPanel = new VirtualizingStackPanel();
            foreach(ModuleSession ms in Classes)
            {
                Button btnModule = new Button();
                btnModule.Content = ms;
                ClassesPanel.Children.Add(btnModule);
            }
            //ClassesPanel.Background = Brushes.DarkBlue;
            return ClassesPanel;
        }
    }
    
    public class dayAssessmentConntrol : ItemsControl
    {

        static dayAssessmentConntrol()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(dayAssessmentConntrol), new FrameworkPropertyMetadata(typeof(dayAssessmentConntrol)));
        }
        public static VirtualizingStackPanel assesmentPanel(List<Pas> pas)
        {
            VirtualizingStackPanel assesmentPanel = new VirtualizingStackPanel();
            foreach(Pas ms in pas)
            {
                Button btnAssess = new Button();
                btnAssess.Content = ms;
                assesmentPanel.Children.Add(btnAssess);
            }
            //assesmentPanel.Background = Brushes.LightBlue;
            return assesmentPanel;
        }
    }
    
    public class dayFrameConntrol : Grid
    {
        static dayFrameConntrol()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(dayFrameConntrol), new FrameworkPropertyMetadata(typeof(dayFrameConntrol)));
        }
        public static Grid DayFormat(Day day, double t)
        {
            Grid dayDisplay = new Grid();

            //dayDisplay.Height = 200;
            dayDisplay.Height = t;

            if(day.Date.Month % 2 == 0)
            {
                dayDisplay.Background = Brushes.LightGray;
            }

            RowDefinition rowA  = new RowDefinition();
            RowDefinition rowB  = new RowDefinition();
            RowDefinition rowC  = new RowDefinition();

            GridLength row1 = new GridLength(20,GridUnitType.Star);
            GridLength row2 = new GridLength(10,GridUnitType.Star);
            GridLength row3 = new GridLength(70,GridUnitType.Star);

            rowA.Height = row1;
            rowB.Height = row2;
            rowC.Height = row3;

            dayDisplay.RowDefinitions.Add(rowA);
            dayDisplay.RowDefinitions.Add(rowB);
            dayDisplay.RowDefinitions.Add(rowC);

            //Label a = dateDisplay(day.Date);
            //VirtualizingStackPanel b = assesmentPanel(day.Assingments);
            //VirtualizingStackPanel c = classPanel(day.Classes);

            //dayDisplay.Children.Add(a);
            //dayDisplay.Children.Add(b);
            //dayDisplay.Children.Add(c);

            //Grid.SetRow(a, 0);
            //Grid.SetRow(b, 1);
            //Grid.SetRow(c, 2);

            return dayDisplay;
        }
    }
}