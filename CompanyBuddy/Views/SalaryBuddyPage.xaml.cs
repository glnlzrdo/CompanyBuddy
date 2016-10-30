using CompanyBuddy.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
////using Windows.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Threading.Tasks;
using CompanyBuddy.Helpers;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CompanyBuddy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SalaryBuddyPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        SalaryComputations compute;

        public SalaryBuddyPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            compute = new SalaryComputations();
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


       
        private void RateTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusText(sender);
        }

        private void RateTxt_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            SingleDot(sender);
        }

        private void RateTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            LeaveText(sender);
            PerformCalculation();
        }

        private void PayTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusText(sender);
        }

        private void PayTxt_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            SingleDot(sender);
        }

        private void PayTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            LeaveText(sender);
            PerformCalculation();
        }

        private void StartDate_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {         
            if (CrntDate.Date < StartDate.Date) //If user enters a future date
            {
                StartDate.Date = DateTime.Now;
            }
            PerformCalculation();
        }

   
        private void OverTimeTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusText(sender);
        }

        private void OverTimeTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            LeaveText(sender);
            PerformCalculation();
        }

        private void OverTimeTxt_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            SingleDot(sender);
        }

        private void AbsentTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AbsentTxt.Text == "")
                AbsentTxt.Text = "0";
            PerformCalculation();
        }

        private void AbsentTxt_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            SingleDot(sender);
            //Control any dot char
            string str = AbsentTxt.Text;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '.')
                {
                    str = str.Remove(i, 1); //Assigning the new value.
                    i--;
                }
            }
            AbsentTxt.Text = str;
            AbsentTxt.Select(AbsentTxt.Text.Length, 0);
        }

        private void AbsentTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            if (AbsentTxt.Text == "0")
                AbsentTxt.Text = "";

            AbsentTxt.Select(AbsentTxt.Text.Length, 0);
        }

        private void CashAdvTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            LeaveText(sender);
            PerformCalculation();
        }

        private void CashAdvTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusText(sender);
        }

        private void CashAdvTxt_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            SingleDot(sender);
        }

        //Read and Write Triggers
        private async void MySalary_Loaded(object sender, RoutedEventArgs e)
        {
            await ReadStartDateFile();

            await ReadFiles("RatePerDay.txt");
            await ReadFiles("Overtime.txt");
            await ReadFiles("Absent.txt");
            await ReadFiles("CashAdvance.txt");
            await ReadFiles("Payables.txt");

            PerformCalculation();
            SaveButton.IsEnabled = false;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditButton.Content.ToString() == "Edit")
            {
                SaveButton.IsEnabled = true;
                EditButton.Content = "Cancel";
                RateTxt.IsEnabled = true;
                StartDate.IsEnabled = true;
                OverTimeTxt.IsEnabled = true;
                AbsentTxt.IsEnabled = true;
                CashAdvTxt.IsEnabled = true;
                PayTxt.IsEnabled = true;
                
            }
            else
            {
                SaveButton.IsEnabled = false;
                EditButton.Content = "Edit";
                RateTxt.IsEnabled = false;
                StartDate.IsEnabled = false;
                OverTimeTxt.IsEnabled = false;
                AbsentTxt.IsEnabled = false;
                CashAdvTxt.IsEnabled = false;
                PayTxt.IsEnabled = false;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SavingProgress.IsActive = true;

            RateTxt.IsEnabled = false;
            StartDate.IsEnabled = false;
            OverTimeTxt.IsEnabled = false;
            AbsentTxt.IsEnabled = false;
            CashAdvTxt.IsEnabled = false;
            PayTxt.IsEnabled = false;
            EditButton.IsEnabled = true;
            SaveButton.IsEnabled = false;

            await WriteStartDateFile();

            await WriteFiles("RatePerDay.txt");
            await WriteFiles("Overtime.txt");
            await WriteFiles("Absent.txt");
            await WriteFiles("CashAdvance.txt");
            await WriteFiles("Payables.txt");

            

            EditButton.Content = "Edit";

            SavingProgress.IsActive = false;

        }

        //Saving Methods
        private async Task WriteStartDateFile()
        {
            // Get the text data from the startingdatepicker.             
            byte[] startDateFileBytes = System.Text.Encoding.UTF8.GetBytes(StartDate.Date.ToString().ToCharArray());

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new folder
            var dataFolder = await local.CreateFolderAsync("SalaryBuddyFolder",
                CreationCollisionOption.OpenIfExists);

            // Create a new file
            var startDateFile = await dataFolder.CreateFileAsync("StartingDateFile.txt",
            CreationCollisionOption.OpenIfExists);

            // Write the data from the Start Date datepicker
            using (var s = await startDateFile.OpenStreamForWriteAsync())
            {
                s.Write(startDateFileBytes, 0, startDateFileBytes.Length);
            }

            // FileWrite(AbsentTxt.)

        }

        private async Task ReadStartDateFile()
        {
            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (local != null)
            {
                // Get the text data from the startingdatepicker.                 
                byte[] startDateFileBytes = System.Text.Encoding.UTF8.GetBytes(StartDate.Date.ToString().ToCharArray());
                // Open existing folder or Create a new folder
                var dataFolder = await local.CreateFolderAsync("SalaryBuddyFolder",
                    CreationCollisionOption.OpenIfExists);
                // Create a new file
                var startingDateFile = await dataFolder.CreateFileAsync("StartingDateFile.txt",
                CreationCollisionOption.OpenIfExists);
                // Get the file.
                var startingDateFileRead = await dataFolder.OpenStreamForReadAsync("StartingDateFile.txt");

                // Read the Starting Date data.
                using (StreamReader streamReader = new StreamReader(startingDateFileRead))
                {
                    string retrieveDateFile = streamReader.ReadToEnd();
                    if (retrieveDateFile == null || retrieveDateFile == "")
                    {
                        // Write the data from the textbox.
                        using (var s = await startingDateFile.OpenStreamForWriteAsync())
                        {
                            s.Write(startDateFileBytes, 0, startDateFileBytes.Length);
                        }
                    }
                    else
                    {
                        retrieveDateFile = retrieveDateFile.Remove(retrieveDateFile.Length - 1);
                        StartDate.Date = Convert.ToDateTime(retrieveDateFile);
                    }
                }

            }
        }

        private async Task ReadFiles(string _filename)
        {
            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            TextBox tb = new TextBox();
            switch (_filename)
            {
                case "RatePerDay.txt":
                    {
                        tb = RateTxt;
                        break;
                    }
                case "Overtime.txt":
                    {
                        tb = OverTimeTxt;
                        break;
                    }
                case "Absent.txt":
                    {
                        tb = AbsentTxt;
                        break;
                    }
                case "CashAdvance.txt":
                    {
                        tb = CashAdvTxt;
                        break;
                    }
                case "Payables.txt":
                    {
                        tb = PayTxt;
                        break;
                    }
            }

            if (local != null)
            {
                //Writing
                // Get the text data                 
                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(tb.Text.ToCharArray());

                // Create a new folder
                var dataFolder = await local.CreateFolderAsync("SalaryBuddyFolder",
                    CreationCollisionOption.OpenIfExists);

                // Create a new file                
                var myFile = await dataFolder.CreateFileAsync(_filename,
                CreationCollisionOption.OpenIfExists);

                // Get the file.
                var fileRead = await dataFolder.OpenStreamForReadAsync(_filename);

                // Read the Rate Per Day data.
                using (StreamReader streamReader = new StreamReader(fileRead))
                {
                    string retrieveFile = streamReader.ReadToEnd();
                    if (retrieveFile == null || retrieveFile == "")
                    {
                        // Write the data from the textbox.
                        using (var s = await myFile.OpenStreamForWriteAsync())
                        {
                            s.Write(fileBytes, 0, fileBytes.Length);
                        }
                    }
                    else
                    {
                        tb.Text = String.Format("{0:C}", retrieveFile);
                        //StartDate.Date = Convert.ToDateTime(retrieveDateFile);
                    }
                }
            }
        }

        private async Task WriteFiles(string _filename)
        {
            TextBox tb = new TextBox();
            switch (_filename)
            {
                case "RatePerDay.txt":
                    {
                        tb = RateTxt;
                        break;
                    }
                case "Overtime.txt":
                    {
                        tb = OverTimeTxt;
                        break;
                    }
                case "Absent.txt":
                    {
                        tb = AbsentTxt;
                        break;
                    }
                case "CashAdvance.txt":
                    {
                        tb = CashAdvTxt;
                        break;
                    }
                case "Payables.txt":
                    {
                        tb = PayTxt;
                        break;
                    }
            }

            // Get the text data
            byte[] startDateFileBytes = System.Text.Encoding.UTF8.GetBytes(tb.Text.ToCharArray());

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new folder
            var dataFolder = await local.CreateFolderAsync("SalaryBuddyFolder",
                CreationCollisionOption.OpenIfExists);

            // Create a new file
            var startDateFile = await dataFolder.CreateFileAsync(_filename,
            CreationCollisionOption.OpenIfExists);


            // Write the data from the Start Date datepicker
            using (var s = await startDateFile.OpenStreamForWriteAsync())
            {
                s.Write(startDateFileBytes, 0, startDateFileBytes.Length);
            }

        }

  
        /*
        private void ControlButtons(string s)
        {
            RateButton.IsEnabled = false;
            StartDateButton.IsEnabled = false;
            DaysAbsentButton.IsEnabled = false;
            CshAdvButton.IsEnabled = false;
            PayButton.IsEnabled = false;
            OverTimeButton.IsEnabled = false;

            switch (s)
            {
                case "rate":
                    {
                        RateButton.IsEnabled = true;
                        break;
                    }
                case "date":
                    {
                        StartDateButton.IsEnabled = true;
                        break;
                    }
                case "overtime":
                    {
                        OverTimeButton.IsEnabled = true;
                        break;
                    }
                case "absent":
                    {
                        DaysAbsentButton.IsEnabled = true;
                        break;
                    }
                case "adv":
                    {
                        CshAdvButton.IsEnabled = true;
                        break;
                    }
                case "pay":
                    {
                        PayButton.IsEnabled = true;
                        break;
                    }
            }


        }
         */

        //Prevent user for entering 2 or more dots
        private void SingleDot(object sender)
        {
            TextBox tb = (TextBox)sender; //Getting the textbox which fired the event if you assigned many textboxes to the same event.
            string str = tb.Text;
            int dotCount = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '.')
                {
                    dotCount++;
                    if (dotCount > 1)
                    {
                        str = str.Remove(i, 1); //Assigning the new value.
                        i--;
                        dotCount--;
                    }
                }
            }
            tb.Text = str;
            tb.Select(tb.Text.Length, 0); //Positioning the cursor at end of textbox.
        }

        private void PerformCalculation()
        {
            compute.MySalary(StartDate.Date.ToString(), RateTxt.Text, OverTimeTxt.Text,
                AbsentTxt.Text, CashAdvTxt.Text, PayTxt.Text);
            TotalDays.Text = compute.TotalDaysWorked;
            MySalary.Text = compute.TotalSalary;
            
            //SaveButton.IsEnabled = true;
        }

        private void LeaveText(object _sender)
        {
            TextBox tb = (TextBox)_sender;
            if (tb.Text == "")
            {
                tb.Text = String.Format("{0:C}", 0);
            }
            else
            {
                tb.Text = String.Format("{0:C}", Convert.ToDouble(tb.Text));
            }
        }

        private void FocusText(object _sender)
        {
            TextBox tb = (TextBox)_sender;
            tb.Text = tb.Text.Remove(0, 1);
            tb.Select(tb.Text.Length, 0);

        }

        
    }
}
