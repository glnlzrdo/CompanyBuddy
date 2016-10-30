using SQLite;
using CompanyBuddy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;


namespace CompanyBuddy.Helpers
{
    public class DatabaseHelperClass
    {
        SQLiteConnection dbConn;

        //Create Table 
        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {
                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<Employees>();
                        dbConn.CreateTable<Attendance>();
                        //dbConn.CreateTable<Company>();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Retrieve the specific Employee from the database. 
        public Employees ReadEmployee(int Employeeid)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingemployee = dbConn.Query<Employees>("select * from Employees where Id =" + Employeeid).FirstOrDefault();
                return existingemployee;
            }
        }        

        // Retrieve Specific Date of all Employees in Attendance
        public ObservableCollection<Attendance> ReadAttendanceDate(string _atte)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Attendance> specificDate = dbConn.Table<Attendance>().Where(v => v.AttDate == _atte).ToList<Attendance>();                
                ObservableCollection<Attendance> AttenDate = new ObservableCollection<Attendance>(specificDate);
                if (specificDate.Count == 0)
                {
                    AddAttendance(_atte);
                    List<Attendance> newDate = dbConn.Table<Attendance>().Where(v => v.AttDate == _atte).ToList<Attendance>();
                    ObservableCollection<Attendance> NewDate = new ObservableCollection<Attendance>(newDate);
                    return NewDate;
                }
                else
                {
                    CheckAndAddNewEmployeeAttendance(AttenDate);
                    List<Attendance> newDate = dbConn.Table<Attendance>().Where(v => v.AttDate == _atte).ToList<Attendance>();
                    ObservableCollection<Attendance> NewDate = new ObservableCollection<Attendance>(newDate);
                    return NewDate;                    
                }
                    
            }           
        }

        // Retrieve attendance of specific employee
        public ObservableCollection<Attendance> ReadAttendanceName(string _atte)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Attendance> specificName = dbConn.Table<Attendance>().Where(v => v.AttName == _atte).ToList<Attendance>();
                ObservableCollection<Attendance> AttenName = new ObservableCollection<Attendance>(specificName);
                return AttenName;
            }
        }

        // Retrieve all Employees list from the database. 
        public ObservableCollection<Employees> ReadEmployees()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Employees> myCollection = dbConn.Table<Employees>().ToList<Employees>();
                ObservableCollection<Employees> EmployeesList = new ObservableCollection<Employees>(myCollection);
                return EmployeesList;
            }
        }

        // Retrieve all Employees list of Attendance from the database for updating 
        public ObservableCollection<Attendance> ReadAttendance()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                List<Attendance> myCollection = dbConn.Table<Attendance>().ToList<Attendance>();
                ObservableCollection<Attendance> AttendanceList = new ObservableCollection<Attendance>(myCollection);
                return AttendanceList;
            }
        }

        //Read and update employees Salary
        public ObservableCollection<Employees> UpdateSalary()
        {
            var dbConn = new SQLiteConnection(App.DB_PATH);            
            List<Employees> myCollection = dbConn.Table<Employees>().ToList<Employees>();
            ObservableCollection<Employees> EmployeesList = ReadEmployees();
            int empCount = EmployeesList.Count;

            for (int i = 0; i < empCount; i++)
            {
                string nameOf = myCollection[i].Name;
                List<Attendance> specificName = dbConn.Table<Attendance>().Where(v => v.AttName == nameOf && v.IsAbsent == true).ToList<Attendance>();
                    // ObservableCollection<Attendance> AttenName = new ObservableCollection<Attendance>(specificName);   
                int absents = specificName.Count;
                for (int g = 0; g < specificName.Count; g++)
			    {
                    if (Convert.ToDateTime(specificName[g].AttDate).Month != DateTime.Now.Month)
                        absents--;                    
                }
                    if (DateTime.Now.Day <= 15)
                        EmployeesList[i].Salary = string.Format("{0:C}", double.Parse((int.Parse(EmployeesList[i].Rate) * (DateTime.Now.Day - absents)).ToString()));
                    else
                        EmployeesList[i].Salary = string.Format("{0:C}", double.Parse((int.Parse(EmployeesList[i].Rate) * ((DateTime.Now.Day - absents) - 15)).ToString()));
            }
                return EmployeesList;
 
        }

        //Update existing employee from Employees and Attendance Table
        public void UpdateEmployee(Employees Employee)
        {
            string oldname;
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {                
                var existingemployee = dbConn.Query<Employees>("select * from Employees where Id =" + Employee.Id).FirstOrDefault();
                oldname = existingemployee.Name;
                if (existingemployee != null)
                {                    
                    existingemployee.Name = Employee.Name;
                    existingemployee.Position = Employee.Position;
                    existingemployee.Address = Employee.Address;
                    existingemployee.Age = Employee.Age;
                    existingemployee.PhoneNumber = Employee.PhoneNumber;
                    existingemployee.Rate = Employee.Rate;
                    existingemployee.CreationDate = Employee.CreationDate;
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingemployee);
                    });                     
                }
            }                   
            //Update Attendance Name if Employee Name is changed
            if (Employee.Name != oldname)
            {
                ObservableCollection<Attendance> myCollection = ReadAttendance();
                var dbAtt = new SQLiteConnection(App.DB_PATH);
                List<Attendance> foundName = dbAtt.Table<Attendance>().Where(v => v.AttName == oldname).ToList<Attendance>();
                for (int i = 0; i < foundName.Count; i++)
                {
                    foundName[i].AttName = Employee.Name;
                    dbAtt.RunInTransaction(() => { dbAtt.Update(foundName[i]); });
                }
            }
        }

        //Update employee if present or absent
        public void UpdateAttendance(int selectedId, string presentOrAbsent)
        {
            var dbAtt = new SQLiteConnection(App.DB_PATH);
            List<Attendance> foundName = dbAtt.Table<Attendance>().Where(v => v.Id == selectedId).ToList<Attendance>();
            if (presentOrAbsent == "present")
            {
                foundName[0].IsPresent = true;
                foundName[0].IsAbsent = false;
            }
            else
            {
                foundName[0].IsPresent = false;
                foundName[0].IsAbsent = true;            
            }
                dbAtt.RunInTransaction(() => { dbAtt.Update(foundName[0]); });            
        }

        // Insert the new Employee in the Employees table. 
        public void Insert(Employees newEmployee)
        {      
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(newEmployee);
                });
            }
        }

        //Generate Attendance for New Employee
        public void GenerateAttendance(string _empName)
        {
            MyConverters attDate = new MyConverters();
            if (DateTime.Now.Day <= 15)
            {
                for (int i = 0; i < 15; i++)
                {                        
                    using (var dbConn = new SQLiteConnection(App.DB_PATH))
                    {
                        Attendance newAtt = new Attendance(attDate.AttendanceDate(DateTime.Now.Month.ToString(), (i + 1).ToString(), DateTime.Now.Year.ToString()),
                            _empName, true);
                        dbConn.RunInTransaction(() => { dbConn.Insert(newAtt); });
                    }
                }
            }
            else
            {
                DateTime tempDate = DateTime.Now;
                tempDate = Convert.ToDateTime(attDate.AttendanceDate(tempDate.Month.ToString(), "1", tempDate.Year.ToString()));
                int tempDays = 0;
                //Get the last number of the Month
                for (int i = 0; i < tempDate.Day; i++)
                {
                    tempDate = tempDate.AddDays(1);
                    tempDays = i;
                }
                for (int i = 0; i <= (tempDays - 15); i++)
                {
                    using (var dbConn = new SQLiteConnection(App.DB_PATH))
                    {
                        Attendance newAtt = new Attendance(attDate.AttendanceDate(DateTime.Now.Month.ToString(), (i + 16).ToString(), DateTime.Now.Year.ToString()),
                            _empName, true);
                        dbConn.RunInTransaction(() => { dbConn.Insert(newAtt); });
                    }                       
                }
            }
            
        }

        //My logic in adding attendance in a month for all employees
        //And it takes too long to generate :)
        public void AddOneMonthAttendance(string _date)
        {
            string _month = Convert.ToDateTime(_date).Month.ToString();
            string _year = Convert.ToDateTime(_date).Year.ToString();
            MyConverters attDate = new MyConverters();
            var dbConn = new SQLiteConnection(App.DB_PATH);
            List<Employees> myCollection = dbConn.Table<Employees>().ToList<Employees>();
            ObservableCollection<Employees> EmployeesList = new ObservableCollection<Employees>(myCollection);
            int empCount = EmployeesList.Count;
 
                DateTime tempDate = DateTime.Now;
                tempDate = Convert.ToDateTime(attDate.AttendanceDate(_month, "1", _year));
                int tempDays = 0;
                for (int i = 0; i < tempDate.Day; i++)
                {
                    tempDate = tempDate.AddDays(1);
                    tempDays = i;
                }

                for (int e = 0; e < empCount; e++)
                {
                    for (int i = 0; i <= tempDays; i++)
                    {
                        //using (var dbConn = new SQLiteConnection(App.DB_PATH))
                        //{
                        Attendance newAtt = new Attendance(attDate.AttendanceDate(_month, (i + 1).ToString(), _year),
                            EmployeesList[e].Name, true);
                        dbConn.RunInTransaction(() => { dbConn.Insert(newAtt); });
                        //}
                    }
                }
            

        }

        public void CheckAndAddNewEmployeeAttendance(ObservableCollection<Attendance> AttenDate)
        {
            ObservableCollection<Employees> EmployeesList = ReadEmployees();
            var dbConn = new SQLiteConnection(App.DB_PATH);
            if (AttenDate.Count != EmployeesList.Count)
            {               
                for (int i = 0; i < EmployeesList.Count; i++)
                {
                    bool existing = false;
                    for (int j = 0; j < AttenDate.Count; j++)
                    {
                        if (EmployeesList[i].Name == AttenDate[j].AttName)
                        {
                            existing = true;
                        }
                    }

                    if (existing == false)
                    {
                        Attendance newAtt = new Attendance(AttenDate[0].AttDate, EmployeesList[i].Name, true);
                        dbConn.RunInTransaction(() => { dbConn.Insert(newAtt); });
                    }
                }


                    //List<Attendance> specificName = dbConn.Table<Attendance>().Where(v => v.AttName == EmployeesList[i].Name && v.AttDate==AttenDate[0].AttDate).ToList<Attendance>();
                    //if (specificName == null)
                    //{
                    //    Attendance newAtt = new Attendance(AttenDate[0].AttDate, EmployeesList[i].Name, true);
                    //    dbConn.RunInTransaction(() => { dbConn.Insert(newAtt); });
                    //}

                
            }
        }

        public void AddAttendance(string _date)
        {            
            var dbConn = new SQLiteConnection(App.DB_PATH);
            List<Employees> myCollection = dbConn.Table<Employees>().ToList<Employees>();
            ObservableCollection<Employees> EmployeesList = new ObservableCollection<Employees>(myCollection);
            int empCount = EmployeesList.Count;
            
            for (int e = 0; e < empCount; e++)
            {            
                    Attendance newAtt = new Attendance(_date,
                        EmployeesList[e].Name, true);
                    dbConn.RunInTransaction(() => { dbConn.Insert(newAtt); });             
            }
        }

        //Delete specific Employee 
        public void DeleteEmployee(int Id)
        {
            string Employee;
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingemployee = dbConn.Query<Employees>("select * from Employees where Id =" + Id).FirstOrDefault();
                Employee = existingemployee.Name;
                if (existingemployee != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingemployee);
                    });
                }
            }
            var dbAtt = new SQLiteConnection(App.DB_PATH);
            List<Attendance> foundName = dbAtt.Table<Attendance>().Where(v => v.AttName == Employee).ToList<Attendance>();
            for (int i = 0; i < foundName.Count; i++)
            {
                dbAtt.RunInTransaction(() => { dbAtt.Delete(foundName[i]); });
            }            
        }

        //Delete all Employeelist in all tables
        public void DeleteAllEmployee()
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {                
                dbConn.DropTable<Employees>();
                dbConn.CreateTable<Employees>();
                dbConn.Dispose();
                dbConn.Close();                
            }
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.DropTable<Attendance>();
                dbConn.CreateTable<Attendance>();
                dbConn.Dispose();
                dbConn.Close();
            }
        }

        // *** Insert the Payroll for the new Employee
        public void InsertPayroll(Payroll newPayroll)
        {
            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                dbConn.RunInTransaction(() => { dbConn.Insert(newPayroll); });
            }
        }


    } 

}
