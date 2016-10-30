using SQLite;
using CompanyBuddy.Model;
using CompanyBuddy.Views;
using CompanyBuddy.Common;
using CompanyBuddy.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
//using Telerik.UI.Xaml.Controls.Data;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CompanyBuddy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            MainMenu.Add(new MyMenu("Assets/EmployeeBuddy.png", "EmployeeBuddy", "Employee management system"));
            MainMenu.Add(new MyMenu("Assets/SalaryBuddy.png", "SalaryBuddy", "Payroll management system"));
            MainMenu.Add(new MyMenu("Assets/EditBuddy.png", "AttendanceBuddy", "Daily routine monitoring"));
            MainMenu.Add(new MyMenu("Assets/SalaryCalculator.png", "CalculatorBuddy", "Calculates individual salary"));
            MainMenu.Add(new MyMenu("Assets/SetupBuddy.png", "SetupBuddy", "Details and settings of Company"));

            MainMenuList.ItemsSource = MainMenu;

        }

        public ObservableCollection<MyMenu> MainMenu = new ObservableCollection<MyMenu>();

        public class MyMenu
        {            
            public MyMenu() { }
            public MyMenu(string imageUri, string menuTitle, string menuDesc)
            {
                ImageUri = imageUri;
                MenuTitle = menuTitle;
                MenuDesc = menuDesc;
            }

            public string ImageUri { get; set; }
            public string MenuTitle { get; set; }
            public string MenuDesc { get; set; }

            public override string ToString()
            {
                return ImageUri + MenuTitle + MenuDesc;
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        ObservableCollection<Employees> DB_EmployeeList = new ObservableCollection<Employees>();
        ReadAllEmployeesList dbEmployees = new ReadAllEmployeesList();

        //private async void MainMenuList_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    var itemId = ((MyMenu)e.ClickedItem).MenuTitle;

        //    DB_EmployeeList = dbEmployees.GetAllEmployees();//Get all DB Employees 
        //    if (DB_EmployeeList.Count != 0)
        //    {
        //        if (itemId.ToString() == "SalaryBuddy")
        //            this.Frame.Navigate(typeof(SalaryBuddy));
        //        else if (itemId.ToString() == "EmployeeBuddy")
        //            this.Frame.Navigate(typeof(EmployeeBuddy));
        //        else if (itemId.ToString() == "AttendanceBuddy")
        //            this.Frame.Navigate(typeof(MyCalendar));
        //        else if (itemId.ToString() == "CalculatorBuddy")
        //            this.Frame.Navigate(typeof(SalaryBuddyPage));
        //        else
        //            this.Frame.Navigate(typeof(MyCompany));
        //    }
        //    else
        //    {
        //        if (itemId.ToString() == "SetupBuddy")
        //            this.Frame.Navigate(typeof(MyCompany));
        //        else
        //        {
        //            var dialog = new MessageDialog("No available employee, want to add now?");
        //            dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(Command)));
        //            dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(Command)));
        //            await dialog.ShowAsync();
        //        }
        //    }

            
        //}

        private void Command(IUICommand command)
        {
            if (command.Label.Equals("Yes"))
            {
                this.Frame.Navigate(typeof(AddEmployee), "firstEmployee");
            }
        }

        private async void MainMenuList_ItemTap(object sender, Telerik.UI.Xaml.Controls.Data.ListBoxItemTapEventArgs e)
        {
            var itemId = ((MyMenu)MainMenuList.SelectedItem).MenuTitle;
            DB_EmployeeList = dbEmployees.GetAllEmployees();//Get all DB Employees 
            if (DB_EmployeeList.Count != 0)
            {
                if (itemId.ToString() == "SalaryBuddy")
                    this.Frame.Navigate(typeof(SalaryBuddy));
                else if (itemId.ToString() == "EmployeeBuddy")
                    this.Frame.Navigate(typeof(EmployeeBuddy));
                else if (itemId.ToString() == "AttendanceBuddy")
                    this.Frame.Navigate(typeof(MyCalendar));
                else if (itemId.ToString() == "CalculatorBuddy")
                    this.Frame.Navigate(typeof(SalaryBuddyPage));
                else
                    this.Frame.Navigate(typeof(MyCompany));
            }
            else
            {
                if (itemId.ToString() == "SetupBuddy")
                    this.Frame.Navigate(typeof(MyCompany));
                else
                {
                    var dialog = new MessageDialog("No available employee, want to add now?");
                    dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(Command)));
                    dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(Command)));
                    await dialog.ShowAsync();
                }
            }
        }
        
        



    }
}
