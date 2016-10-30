using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CompanyBuddy.Helpers
{
    public class MyConverters
    {
        public string AttendanceDate(string _month, string _day, string _year)
        {
            string myDate = Convert.ToDateTime(_month + "/" + _day + "/" + _year).ToString("MMMM dd yyyy");
            return myDate;
        }
    }

    public class CustomCellStateSelector : CalendarCellStateSelector
    {
        protected override void SelectStateCore(CalendarCellStateContext context, RadCalendar container)
        {
            if (context.Date > DateTime.Now)
            {
                context.IsBlackout = true;
            }
        }
    }
}
