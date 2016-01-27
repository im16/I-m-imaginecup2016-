using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.Background;


namespace realBluetoothTest__
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        // The background task registration for the background advertisement publisher
        private IBackgroundTaskRegistration taskRegistration;
        // The publisher trigger used to configure the background task registration
        private BluetoothLEAdvertisementPublisherTrigger trigger;
        
        // A name is given to the task in order for it to be identifiable across context.
        private string taskName = "Background_publisher";
        // Entry point for the background task.
        private string taskEntryPoint = "BackgroundTasks.AdvertisementPublisherTask";

        private BluetoothLEAdvertisementWatcher watcher;
        private DispatcherTimer dispatcherTimer;
        private Client_List current_client= new Client_List();

       

        public MainPage()
        {
            this.InitializeComponent();

            // Create and initialize a new watcher instance.
            watcher = new BluetoothLEAdvertisementWatcher();

            // Create and initialize a new trigger to configure it.
            trigger = new BluetoothLEAdvertisementPublisherTrigger();

            // Add a manufacturer-specific section:
            // First, create a manufacturer data section
            var manufacturerData = new BluetoothLEManufacturerData();

            // Then, set the company ID for the manufacturer data. Here we picked an unused value: 0xFF00
            manufacturerData.CompanyId = 0xFF00;


            // Finally set the data payload within the manufacturer-specific section
            // Here, use a 16-bit UUID: 0x1234 -> {0x34, 0x12} (little-endian)
            var writer = new DataWriter();
          //  UInt16 uuidData = 0x1234;
           // writer.WriteUInt16(uuidData);
            string s = "mk";

            writer.WriteString(s);
  
        

            // Make sure that the buffer length can fit within an advertisement payload. Otherwise you will get an exception.
            manufacturerData.Data = writer.DetachBuffer();
         

            //(publisher)
            // Add the manufacturer data to the advertisement publisher: 
            trigger.Advertisement.ManufacturerData.Add(manufacturerData);

            // add filter (watcher)
            // watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);


            // Configure the signal strength filter to only propagate events when in-range
            // Please adjust these values if you cannot receive any advertisement 
            // Set the in-range threshold to -70dBm. This means advertisements with RSSI >= -70dBm 
            // will start to be considered "in-range".
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;

            // Set the out-of-range threshold to -75dBm (give some buffer). Used in conjunction with OutOfRangeTimeout
            // to determine when an advertisement is no longer considered "in-range"
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -75;

            // Set the out-of-range timeout to be 2 seconds. Used in conjunction with OutOfRangeThresholdInDBm
            // to determine when an advertisement is no longer considered "in-range"
           watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(1000);

            // By default, the sampling interval is set to zero, which means there is no sampling and all
            // the advertisement received is returned in the Received event

            // End of watcher configuration. There is no need to comment out any code beyond this point.
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // Get the existing task if already registered
            if (taskRegistration == null)
            {
                // Find the task if we previously registered it
                foreach (var task in BackgroundTaskRegistration.AllTasks.Values)
                {
                    if (task.Name == taskName)
                    {
                        taskRegistration = task;
                        taskRegistration.Completed += OnBackgroundTaskCompleted;
                        break;
                    }
                }
            }
            else
            {
                taskRegistration.Completed += OnBackgroundTaskCompleted;
            }


            // Attach a handler to process the received advertisement. 
            // The watcher cannot be started without a Received handler attached
            watcher.Received += OnAdvertisementReceived;

            // Attach a handler to process watcher stopping due to various conditions,
            // such as the Bluetooth radio turning off or the Stop method was called
            watcher.Stopped += OnAdvertisementWatcherStopped;

            // Attach handlers for suspension to stop the watcher when the App is suspended.
            App.Current.Suspending += App_Suspending;
            App.Current.Resuming += App_Resuming;

        }
        private void Click_dbTest(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(dbTest));
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Remove local suspension handlers from the App since this page is no longer active.
            App.Current.Suspending -= App_Suspending;
            App.Current.Resuming -= App_Resuming;

            // Since the publisher is registered in the background, the background task will be triggered when the App is closed 
            // or in the background. To unregister the task, press the Stop button.
            if (taskRegistration != null)
            {
                // Always unregister the handlers to release the resources to prevent leaks.
                taskRegistration.Completed -= OnBackgroundTaskCompleted;
            }

            // Make sure to stop the watcher when leaving the context. Even if the watcher is not stopped,
            // scanning will be stopped automatically if the watcher is destroyed.
            watcher.Stop();
            // Always unregister the handlers to release the resources to prevent leaks.
            watcher.Received -= OnAdvertisementReceived;
            watcher.Stopped -= OnAdvertisementWatcherStopped;

            
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            //publisher stop
            if (taskRegistration != null)
            {
                // Always unregister the handlers to release the resources to prevent leaks.
                taskRegistration.Completed -= OnBackgroundTaskCompleted;
            }

            Debug.WriteLine("App Suspending.....");

            // Make sure to stop the watcher on suspend.
            watcher.Stop();
            // Always unregister the handlers to release the resources to prevent leaks.
            watcher.Received -= OnAdvertisementReceived;
            watcher.Stopped -= OnAdvertisementWatcherStopped;
        }

        /// <summary>
        /// Invoked when application execution is being resumed.
        /// </summary>
        /// <param name="sender">The source of the resume request.</param>
        /// <param name="e"></param>
        private void App_Resuming(object sender, object e)
        {
            watcher.Received += OnAdvertisementReceived;
            watcher.Stopped += OnAdvertisementWatcherStopped;


        }

      

        /***********************************  publisher!!  ***************************************************/

            /*
        private async void Background_button_text_change(bool status)
        {
            // button text change!
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if(status)
                    BackGroundButton.Content = "Background_Start!!";
                else
                    BackGroundButton.Content = "Background_Stop!!";

            });

        }

            */

        private async void Background_Start(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Background start.");

            // Registering a background trigger if it is not already registered. It will start background advertising.
            // First get the existing tasks to see if we already registered for it
            if (taskRegistration != null)
                {
                    Debug.WriteLine("Background publisher already registered.");
                    return;
                }

                else
                {

                Debug.WriteLine("Background start1");
                // Applications registering for background trigger must request for permission.
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                // Here, we do not fail the registration even if the access is not granted. Instead, we allow 
                // the trigger to be registered and when the access is granted for the Application at a later time,
                // the trigger will automatically start working again.

                Debug.WriteLine("Background start1-2");
                // At this point we assume we haven't found any existing tasks matching the one we want to register
                // First, configure the task entry point, trigger and name
                var builder = new BackgroundTaskBuilder();
                    builder.TaskEntryPoint = taskEntryPoint;
                    builder.SetTrigger(trigger);
                    builder.Name = taskName;

                    // Now perform the registration.
                    taskRegistration = builder.Register();
                Debug.WriteLine("Background start2");

                // For this scenario, attach an event handler to display the result processed from the background task
                taskRegistration.Completed += OnBackgroundTaskCompleted;

                    // Even though the trigger is registered successfully, it might be blocked. Notify the user if that is the case.
                    if ((backgroundAccessStatus == BackgroundAccessStatus.Denied) || (backgroundAccessStatus == BackgroundAccessStatus.Unspecified))
                    {
                        Debug.WriteLine("Not able to run in background. Application must given permission to be added to lock screen.");
                    }
                    else
                    {
                        Debug.WriteLine("Background publisher registered.");
                    }
                }
            

         
        }
        private void Background_Stop(object sender, RoutedEventArgs e)
        {
            // Unregistering the background task will stop advertising if this is the only client requesting
            // First get the existing tasks to see if we already registered for it
            if (taskRegistration != null)
            {
                taskRegistration.Unregister(true);
                taskRegistration = null;
           
            }
            else
            {
               
            }
        }


        //publisher status
        private void OnBackgroundTaskCompleted(BackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs eventArgs)
        {
            // We get the status changed processed by the background task
            if (ApplicationData.Current.LocalSettings.Values.Keys.Contains(taskName))
            {
                string backgroundMessage = (string)ApplicationData.Current.LocalSettings.Values[taskName];
                Debug.WriteLine(backgroundMessage);
            }
        }


        /***********************************  watcher!!  ***************************************************/
      
        /// <summary>
        /// Invoked as an event handler when the Run button is pressed.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            // Calling watcher start will start the scanning if not already initiated by another client
            watcher.Start();
            ReceivedAdvertisementListBox.Items.Clear();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }

        async void dispatcherTimer_Tick(object sender, object e)
        {
            Debug.WriteLine("hello");
            current_client.remove_client();


            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ReceivedAdvertisementListBox.Items.Clear();

                // Display these information on the list
                for (int i = 0; i < current_client.client_num; i++)
                    ReceivedAdvertisementListBox.Items.Add(string.Format("Found client=[{0}]", current_client.client_id(i)));
            });
        }
        /// <summary>
        /// Invoked as an event handler when the Stop button is pressed.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        /// 


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Stopping the watcher will stop scanning if this is the only client requesting scan
            watcher.Stop();

       
        }

        /// <summary>
        /// Invoked as an event handler when an advertisement is received.
        /// </summary>
        /// <param name="watcher">Instance of watcher that triggered the event.</param>
        /// <param name="eventArgs">Event data containing information about the advertisement event.</param>
        private async void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            
            // We can obtain various information about the advertisement we just received by accessing 
            // the properties of the EventArgs class

            // The timestamp of the event
            DateTimeOffset timestamp = eventArgs.Timestamp;

            // The type of advertisement
            BluetoothLEAdvertisementType advertisementType = eventArgs.AdvertisementType;

            // The received signal strength indicator (RSSI)
            Int16 rssi = eventArgs.RawSignalStrengthInDBm;

            // The local name of the advertising device contained within the payload, if any
            string localName = eventArgs.Advertisement.LocalName;

            // Check if there are any manufacturer-specific sections.
            // If there is, print the raw data of the first manufacturer section (if there are multiple).
            var manufacturerSections = eventArgs.Advertisement.ManufacturerData;
            int client_num = manufacturerSections.Count;
 
            string [] client_ids = new string[client_num];

            if (client_num > 0)
            {
                for (int i=0; i< client_num; i++)
                {
                    var manufacturerData = manufacturerSections[i];
                    var data = new byte[manufacturerData.Data.Length];
                    using (var reader = DataReader.FromBuffer(manufacturerData.Data))
                    {
                        reader.ReadBytes(data);
                    }
                    // Print the company ID + the raw data in hex format
                    client_ids[i] = string.Format("{0}",
                        BitConverter.ToString(data));
                }
            }
            // compare pre_member with current member
            current_client.check_exist(client_ids);

            // Serialize UI update to the main UI thread
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                 ReceivedAdvertisementListBox.Items.Clear();
                
                // Display these information on the list
                for (int i=0; i< current_client.client_num; i++)
                 ReceivedAdvertisementListBox.Items.Add(string.Format("Found client=[{0}]", current_client.client_id(i)));
            });
        }


        /// <summary>
        /// Invoked as an event handler when the watcher is stopped or aborted.
        /// </summary>
        /// <param name="watcher">Instance of watcher that triggered the event.</param>
        /// <param name="eventArgs">Event data containing information about why the watcher stopped or aborted.</param>
        private async void OnAdvertisementWatcherStopped(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementWatcherStoppedEventArgs eventArgs)
        {
            // Notify the user that the watcher was stopped
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
               
            });
        }
   

}
}
