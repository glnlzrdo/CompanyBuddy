using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CompanyBuddy.Model
{
    class PayrollCalculate : INotifyPropertyChanged
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        private string empName;

        public string EmpName
        {
            get { return empName; }
            set 
            {
                if (value != empName)
                {
                    empName = value;
                    OnPropertyChanged("EmpName");
                }
            }
        }
        

        private string startDate;

        public string StartDate
        {
            get { return startDate; }
            set
            {
                if (value != startDate)
                {
                    startDate = value;
                    OnPropertyChanged("StartDate");
                }

            }
        }

        private string rate;

        public string Rate
        {
            get { return rate; }
            set 
            {
                if (value != rate)
                {
                    rate = value;
                    OnPropertyChanged("Rate");
                }
            }
        }

        private string overtime;

        public string Overtime
        {
            get { return overtime; }
            set 
            {
                if (value != overtime)
                {
                    overtime = value;
                    OnPropertyChanged("Overtime");
                }    
            }
        }


        private string absent;

        public string Absent
        {
            get { return absent; }
            set 
            {
                if (value != absent)
                {
                    absent = value;
                    OnPropertyChanged("Absent");
                }
            }
        }

        private string cashAdvance;

        public string CashAdvance
        {
            get { return cashAdvance; }
            set 
            {
                if (value != cashAdvance)
                {
                    cashAdvance = value;
                    OnPropertyChanged("CashAdvance");
                }
            }
        }

        private string pay;

        public string Pay
        {
            get { return pay; }
            set 
            {
                if (value != pay)
                {
                    pay = value;
                    OnPropertyChanged("Pay");
                }
            }
        }

        

        
        
        public event PropertyChangedEventHandler PropertyChanged;
        
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
