using CompanyBuddy.Helpers;
using CompanyBuddy.Model;
using CompanyBuddy.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CompanyBuddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEmployee : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public AddEmployee()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        ObservableCollection<Attendance> DB_Attendance = new ObservableCollection<Attendance>();
        ObservableCollection<Employees> DB_Employees = new ObservableCollection<Employees>();
        DatabaseHelperClass newAtte = new DatabaseHelperClass();
        private string fromMainPage;

        private async void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            Saving.IsActive = true;
            DatabaseHelperClass Db_Helper = new DatabaseHelperClass();//Creating object for DatabaseHelperClass.cs from ViewModel/DatabaseHelperClass.cs 
            if (NametxtBx.Text != "" & PhonetxtBx.Text != "" & PositiontxtBx.Text != "" & AddresstxtBx.Text != "" & AgetxtBx.Text != "" & RatetxtBx.Text != "")
            {
                Db_Helper.Insert(new Employees(NametxtBx.Text, PhonetxtBx.Text, PositiontxtBx.Text, AddresstxtBx.Text, AgetxtBx.Text, RatetxtBx.Text));

                ReadAllEmployeesList dbAtte = new ReadAllEmployeesList();
                DB_Employees = dbAtte.GetAllEmployees();
                newAtte.GenerateAttendance(NametxtBx.Text);

                Employees.EmployeeAdded = true; //reference that the navigation came from adding employee
                if (fromMainPage != "firstEmployee")
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1); //removing the back button's previous history
                Frame.Navigate(typeof(EmployeeBuddy));//after adding Employee redirect to EmployeeBuddy page   
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("Please fill all the fields");//Text should not be empty 
                await messageDialog.ShowAsync();
                Saving.IsActive = false;
            }
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
            if (e.Parameter != null)
            fromMainPage = e.Parameter.ToString();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);            
        }

        #endregion
    }
}
