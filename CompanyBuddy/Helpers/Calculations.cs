using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CompanyBuddy.Model;
using SQLite;

namespace CompanyBuddy.Helpers
{



    public class SalaryComputations
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
                        dbConn.CreateTable<PayrollCalculate>();
                        //dbConn.CreateTable<Attendance>();
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

        //Public object declarations        
        public string TotalDaysWorked {get; set;}        
        public string TotalSalary {get; set;}


       
        public SalaryComputations()
        {            
            this.TotalDaysWorked = String.Empty;            
            this.TotalSalary = String.Empty;

        }

        public void MySalary(string _empName)
        {
            PayrollCalculate Payrolls = new PayrollCalculate();

            using (var dbConn = new SQLiteConnection(App.DB_PATH))
            {
                var existingemployee = dbConn.Query<PayrollCalculate>("select * from Employees where EmpName =" + _empName).FirstOrDefault();

                DateTime myCrntDateTime = new DateTime();
                myCrntDateTime = DateTime.Now;
                myCrntDateTime = myCrntDateTime.Subtract(myCrntDateTime.TimeOfDay);

                DateTime myStartDateTime = new DateTime();
                myStartDateTime = Convert.ToDateTime(existingemployee.StartDate);
                myStartDateTime = myStartDateTime.Subtract(myStartDateTime.TimeOfDay);

                TimeSpan totDays = new TimeSpan();

                totDays = myCrntDateTime.Date - myStartDateTime.Date;

                totDays = myCrntDateTime - myStartDateTime;

                int myTotalDays = ((int)totDays.TotalDays + 1) - int.Parse(existingemployee.Absent);

                double salary = (((double)myTotalDays * double.Parse(existingemployee.Rate)) + double.Parse(existingemployee.Overtime)) - (double.Parse(existingemployee.CashAdvance) + double.Parse(existingemployee.Pay));

                this.TotalDaysWorked = myTotalDays.ToString();
                this.TotalSalary = String.Format("{0:C}", salary);

            }
        }

        public void MySalary(string _strtDate, string _rate, string _overtime, string _absent, string _cashAdvance ,string _pay)
        {
            DateTime myCrntDateTime = new DateTime();
            myCrntDateTime = DateTime.Now;
            myCrntDateTime = myCrntDateTime.Subtract(myCrntDateTime.TimeOfDay);

            DateTime myStartDateTime = new DateTime();
            myStartDateTime = Convert.ToDateTime(_strtDate);
            myStartDateTime = myStartDateTime.Subtract(myStartDateTime.TimeOfDay);

            TimeSpan  totDays = new TimeSpan();

            totDays = myCrntDateTime.Date - myStartDateTime.Date;

            totDays = myCrntDateTime - myStartDateTime;

            int myTotalDays = ((int)totDays.TotalDays + 1) - int.Parse(_absent);

            double salary = (((double)myTotalDays * double.Parse(_rate.Remove(0,1))) + double.Parse(_overtime.Remove(0,1))) - (double.Parse(_cashAdvance.Remove(0,1)) + double.Parse(_pay.Remove(0,1)));
                       
            this.TotalDaysWorked = myTotalDays.ToString();
            this.TotalSalary = String.Format("{0:C}", salary);
        }
        

    }

}
