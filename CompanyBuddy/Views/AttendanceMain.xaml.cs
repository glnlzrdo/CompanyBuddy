using CompanyBuddy.Common;
using CompanyBuddy.Helpers;
using CompanyBuddy.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Windows.UI.Xaml.Navigation;
//using WinRTXamlToolkit.Controls;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CompanyBuddy.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AttendanceMain : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        ObservableCollection<Attendance> DB_AttendanceList = new ObservableCollection<Attendance>();
        MyConverters DaysGenerator = new MyConverters();

        
        public AttendanceMain()
        {
            this.InitializeComponent();
            this.Loaded += AttendanceBuddy_Loaded;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void AttendanceBuddy_Loaded(object sender, RoutedEventArgs e)
        {           
            ReadAllEmployeesList dbEmployees = new ReadAllEmployeesList();
            DatabaseHelperClass dbAttHelper = new DatabaseHelperClass();

            if (DateTime.Now.Day <= 15)
            {
                for (int i = 0; i <= (DateTime.Now.Day - 1); i++)
                {
                    GeneratedDays.Add(new AttendanceMenuGenerator(DaysGenerator.AttendanceDate(DateTime.Now.Month.ToString(), (i + 1).ToString(), DateTime.Now.Year.ToString())));
                }
            }
            else
            {
                for (int i = 0; (i + 16) <= DateTime.Now.Day; i++)
                {
                    GeneratedDays.Add(new AttendanceMenuGenerator(DaysGenerator.AttendanceDate(DateTime.Now.Month.ToString(), (i + 16).ToString(), DateTime.Now.Year.ToString())));
                }
            }

            listBoxobj.ItemsSource = GeneratedDays;
            CrntDate.Text = DateTime.Now.ToString("MMMM dd yyyy");
        }



        public ObservableCollection<AttendanceMenuGenerator> GeneratedDays = new ObservableCollection<AttendanceMenuGenerator>();

        public class AttendanceMenuGenerator
        {
            public AttendanceMenuGenerator() {}
            
            public string AttDays { get; set; }

            public AttendanceMenuGenerator(string _date)
            {
                AttDays = _date;
            }
            
        }

        private void listBoxobj_ItemClick(object sender, ItemClickEventArgs e)
        {

            var itemid = ((AttendanceMenuGenerator)e.ClickedItem).AttDays;
            this.Frame.Navigate(typeof(AttendanceBuddy), itemid);

        }
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

      
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

       
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

        
    }
}
