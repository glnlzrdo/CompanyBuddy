using CompanyBuddy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyBuddy.Helpers
{
    public class ReadAllEmployeesList
    {
        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
        public ObservableCollection<Employees> GetAllEmployees()
        {
            return Db_Helper.ReadEmployees();
        }

        public ObservableCollection<Employees> SetSalary()
        {
            return Db_Helper.UpdateSalary();
        }

        public ObservableCollection<Attendance> GetAttendance()
        {
            return Db_Helper.ReadAttendance();
        }

    }
}
