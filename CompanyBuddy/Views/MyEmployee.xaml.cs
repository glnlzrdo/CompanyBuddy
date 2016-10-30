using CompanyBuddy.Common;
using CompanyBuddy.Helpers;
using CompanyBuddy.Model;
using CompanyBuddy.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CompanyBuddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyEmployee : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        ObservableCollection<Employees> DB_EmployeeList = new ObservableCollection<Employees>();
        int Selected_EmployeeId = 0; //Specific Employee ID
        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
        Employees currentEmployee = new Employees();
        private MyViewModel ViewModelInit = new MyViewModel();


        public MyEmployee()
        {
            this.InitializeComponent();
            this.Loaded += MyEmployee_Loaded;
            this.DataContext = ViewModelInit;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void MyEmployee_Loaded(object sender, RoutedEventArgs e)
        {
            ReadAllEmployeesList dbEmployees = new ReadAllEmployeesList();
            DB_EmployeeList = dbEmployees.SetSalary();//Get all DB Employees 
            ListPayroll.ItemsSource = DB_EmployeeList.OrderByDescending(i => i.Id).ToList();//Binding DB data to LISTBOX and Latest Employee ID can Display first. 
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            Selected_EmployeeId = int.Parse(e.Parameter.ToString());
            currentEmployee = Db_Helper.ReadEmployee(Selected_EmployeeId);
            NametxtBx.Text = currentEmployee.Name;
            PositiontxtBx.Text = currentEmployee.Position;
            AddresstxtBx.Text = currentEmployee.Address;
            AgetxtBx.Text = currentEmployee.Age;
            PhonetxtBx.Text = currentEmployee.PhoneNumber;
            RatetxtBx.Text = currentEmployee.Rate;

            ListAttendance.ItemsSource = Db_Helper.ReadAttendanceName(currentEmployee.Name);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            Saving.IsActive = true;
            if (NametxtBx.Text != "" & PhonetxtBx.Text != "" & PositiontxtBx.Text != "" & AddresstxtBx.Text != "" & AgetxtBx.Text != "" & RatetxtBx.Text != "")
            {
                currentEmployee.Name = NametxtBx.Text;
                currentEmployee.Position = PositiontxtBx.Text;
                currentEmployee.Address = AddresstxtBx.Text;
                currentEmployee.Age = AgetxtBx.Text;
                currentEmployee.PhoneNumber = PhonetxtBx.Text;
                currentEmployee.Rate = RatetxtBx.Text;
                Db_Helper.UpdateEmployee(currentEmployee);

                Employees.EmployeeAdded = true; //reference that the navigation came from updating or adding employee
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1); //removing the back button's previous history
                Frame.Navigate(typeof(EmployeeBuddy));//after updating Employee redirect to EmployeeBuddy page                         
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Please fill all the fields");//Text should not be empty 
                await messageDialog.ShowAsync();
            }

        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            Saving.IsActive = true;
            var dialog = new MessageDialog("Are you sure you want to remove this employee ?");
            dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(Command)));
            dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(Command)));
            await dialog.ShowAsync();
            Saving.IsActive = false;
        }

        private void Command(IUICommand command)
        {
            if (command.Label.Equals("Yes"))
            {
                Db_Helper.DeleteEmployee(Selected_EmployeeId);
                Employees.EmployeeAdded = true; //reference that the navigation came from adding employee
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1); //removing the back button's previous history
                Frame.Navigate(typeof(EmployeeBuddy));//after adding Employee redirect to EmployeeBuddy page             
            }
        }

    }
}
