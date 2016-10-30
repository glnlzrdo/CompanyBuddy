using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using CompanyBuddy.Helpers;
using CompanyBuddy.Model;
using System.Collections.ObjectModel;

namespace CompanyBuddy.ViewModel
{
    public class MyViewModel 
    {
        public MyViewModel()
        {
            PresentCommand = new DelegateCommand(IsPresent, param => this.canExecute);
            AbsentCommand = new DelegateCommand(IsAbsent, param => this.canExecute);
            SalaryCommand = new DelegateCommand(Compute, param => this.canExecute);
        }

        ReadAllEmployeesList readAtte = new ReadAllEmployeesList();
        DatabaseHelperClass dbAtte = new DatabaseHelperClass();
        SalaryComputations calculate = new SalaryComputations();

        public ObservableCollection<Attendance> PassAttendanceContent(string sender)
        {
           return dbAtte.ReadAttendanceDate(sender);
        }

        public void IsPresent(object sender)
        {
            dbAtte.UpdateAttendance(int.Parse(sender.ToString()), "present");
        }

        public void IsAbsent(object sender)
        {
            dbAtte.UpdateAttendance(int.Parse(sender.ToString()), "absent");
        }

        public void Compute(object sender)
        {
            calculate.MySalary(sender.ToString());
        }

        private ICommand salaryCommand;

        public ICommand SalaryCommand
        {
            get { return salaryCommand; }
            set { salaryCommand = value; }
        }
        

        private ICommand presentCommand;
        public ICommand PresentCommand
        {
            get { return presentCommand; }
            set { presentCommand = value; }
        }

        private ICommand absentCommand;
        public ICommand AbsentCommand
        {
            get { return absentCommand; }
            set { absentCommand = value; }
        }

        private bool canExecute = true;
        public bool CanExecute
        {
            get { return this.canExecute; }
            set
            {
                if (this.canExecute == value) { return;}
                this.canExecute = value;
            }
        }
    }


    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute)
            : this(execute, null) { }

        public DelegateCommand(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            { return true; }
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
