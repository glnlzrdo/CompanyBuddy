using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CompanyBuddy.Model
{

   
    public class Employees //: INotifyPropertyChanged
    {
        //The Id property is marked as the Primary Key
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string CreationDate { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string Age { get; set; }
        public string ImgUrl { get; set; }
        public string MyAbsents { get; set; }
        public string Rate { get; set; }
        public string Salary { get; set; }
        
        public static bool EmployeeAdded = false; // Add and Update Employee marker

        public Employees()
        {
            //empty constructor
        }

        public Employees(string name, string phone_no, string position, string address, string age, string rate)
        {
            Name = name;
            PhoneNumber = phone_no;
            Position = position;
            Address = address;
            Age = age;
            Rate = rate;
            Salary = "0";
            MyAbsents = "0";
            
        }
       
    }

    public class Payroll : Employees //For Main Payroll Menu
    {
        //public string Basis { get; set; }
        public string OverTime { get; set; }
        public string DaysOff { get; set; }
        public string CashAdvance { get; set; }
        public string Payables { get; set; }
        public string TotalDays { get; set; }
        //public string Salary { get; set; }

        public Payroll()
        {
            OverTime = "0";
            DaysOff = "0";
            CashAdvance = "0";
            Payables = "0";
            TotalDays = "0";
            Salary = "0";
        }


        public Payroll(string _totalDays, string _overTime, string _daysOff, string _cashAdvance, string _payables, string _salary)
        {
            OverTime = _overTime;
            DaysOff = _daysOff;
            CashAdvance = _cashAdvance;
            Payables = _payables;
            TotalDays = _totalDays;
            Salary = _salary;
        }
    }

    
	public class OvertimePay : Employees
	{
        public string OvertimeRate { get; set; }
        public string OvertimeHours { get; set; }
        public string OvertimeTotal { get; set; }

        public OvertimePay()
        {

        }

        public OvertimePay(string _otRate, string _otHours, string _otTotal)
        {
            OvertimeRate = _otRate;
            OvertimeHours = _otHours;
            OvertimeTotal = _otTotal;
        }
	}


    public class Absents : Employees
    {
        public string DateofAbsent { get; set; }

        public Absents()
        {

        }

        public Absents(string _dateAbsent)
        {
            DateofAbsent = _dateAbsent;
        }
    }

    public class Attendance : INotifyPropertyChanged
    {
        private string _attName;
        private string _attDate;
        private bool _isPresent;
        private bool _isAbsent;

        public event PropertyChangedEventHandler PropertyChanged; //Event Declaration

        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }        
        
        public string AttDate
        {
            get { return _attDate; }
            set 
            {
                if (value != _attDate)
                {
                    _attDate = value;
                    OnPropertyChanged("AttDate");
                }
            }
        }
        
        public string AttName 
        {
            get { return _attName; }
            set 
            {
                if (value != _attName)
                {
                    _attName = value;                   
                    OnPropertyChanged("AttName");
                }
            }
        }

        public bool IsPresent 
        {
            get { return _isPresent; }
            set
            {
                if (value != _isPresent)
                {
                    _isPresent = value;
                    OnPropertyChanged("IsPresent");
                }
            }
        }

        public bool IsAbsent 
        {
            get { return _isAbsent; }
            set
            {
                if (value != _isAbsent)
                {
                    _isAbsent = value;
                    OnPropertyChanged("IsAbsent");
                }
            }
        }

        public Attendance()
        {

        }

        public Attendance(string date, string eName, bool isPresent)
        {
            _attName = eName;
            _attDate = date;
            _isPresent = isPresent;
            if (isPresent == true)
                _isAbsent = false;
            else
                _isAbsent = true;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    

   

}
