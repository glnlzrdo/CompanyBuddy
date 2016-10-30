using CompanyBuddy.Helpers;
using CompanyBuddy.Model;
using CompanyBuddy.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CompanyBuddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmployeeBuddy : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ObservableCollection<Employees> DB_EmployeeList = new ObservableCollection<Employees>();

        public EmployeeBuddy()
        {
            this.InitializeComponent();
            this.Loaded += EmployeeBuddy_Loaded;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void EmployeeBuddy_Loaded(object sender, RoutedEventArgs e)
        {
            ReadAllEmployeesList dbEmployees = new ReadAllEmployeesList();
            DB_EmployeeList = dbEmployees.GetAllEmployees();//Get all DB Employees 
            if (DB_EmployeeList.Count > 0)
            {
                Btn_Delete.IsEnabled = true;
            }
            listBoxobj.ItemsSource = DB_EmployeeList.OrderByDescending(i => i.Id).ToList();//Binding DB data to LISTBOX and Latest Employee ID can Display first. 
        }
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddEmployee));           
        }
        private async void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            Saving.IsActive = true;
            var dialog = new MessageDialog("Are you sure you want to remove all your data ?");
            dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(Command)));
            dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(Command)));
            await dialog.ShowAsync();
            Saving.IsActive = false;
        }
        private void Command(IUICommand command)
        {
            if (command.Label.Equals("Yes"))
            {
                DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
                Db_Helper.DeleteAllEmployee();//delete all DB Employees 
                DB_EmployeeList.Clear();//Clear collections 
                Btn_Delete.IsEnabled = false;
                listBoxobj.ItemsSource = DB_EmployeeList;
            }
        }
        private void listBoxobj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int SelectedEmployeeID = 0;
            if (listBoxobj.SelectedIndex != -1)
            {
                Employees listitem = listBoxobj.SelectedItem as Employees;//Get slected listbox item Employee ID 
                Frame.Navigate(typeof(MyEmployee), SelectedEmployeeID = listitem.Id);

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
            if (Employees.EmployeeAdded == true) //if navigation came from adding or updating employee
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
                Employees.EmployeeAdded = false;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
