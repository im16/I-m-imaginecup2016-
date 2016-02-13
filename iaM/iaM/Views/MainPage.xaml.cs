using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth.Advertisement;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using System.Threading;
using Windows.UI;



// 빈 페이지 항목 템플릿은 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 에 문서화되어 있습니다.

namespace iaM.Views
{

    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings;
        
        bool menu_flag = true;
        bool flag = true;
        bool visibility_flag = true;
        public static MainPage Current;

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
        private DispatcherTimer seacrch_IsActive;
        private Client_List current_client = new Client_List();

        private int timer = 1;

        private My_Info my_info = new My_Info();
       

        public List<List_item> items { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            items = new List<List_item>();
            my_info.id = localSettings.Values["id"].ToString();

            // Create and initialize a new watcher instance.
            watcher = new BluetoothLEAdvertisementWatcher();

            // Create and initialize a new trigger to configure it.
            trigger = new BluetoothLEAdvertisementPublisherTrigger();

            // Add a manufacturer-specific section:
            // First, create a manufacturer data section
            var manufacturerData_publisher = new BluetoothLEManufacturerData();
            var manufacturerData_watcher = new BluetoothLEManufacturerData();

            // Then, set the company ID for the manufacturer data. Here we picked an unused value: 0xFF00
            manufacturerData_publisher.CompanyId = 0xFF00;
            manufacturerData_watcher.CompanyId = 0xFF00;


            // Finally set the data payload within the manufacturer-specific section
            // Here, use a 16-bit UUID: 0x1234 -> {0x34, 0x12} (little-endian)
            var writer = new DataWriter();

            string id = my_info.id;
            writer.WriteString(id);

            
            // Make sure that the buffer length can fit within an advertisement payload. Otherwise you will get an exception.
            manufacturerData_publisher.Data = writer.DetachBuffer();


            //(publisher)
            // Add the manufacturer data to the advertisement publisher: 
            trigger.Advertisement.ManufacturerData.Add(manufacturerData_publisher);

            // 여기 필터추가
            watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData_watcher);


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
            dispatcherTimer.Stop();
            seacrch_IsActive.Stop();
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
        private void ProgressRing_Click(object sender, RoutedEventArgs e)
        {
            /* if (flag)
           {
               Storyboard1.Pause();
               flag = false;
           }
           else
           {
               Storyboard1.Begin();
               flag = true;
           }*/
           
            /*일단여기서 불러오기*/
            //Image test_img = new Image { Source = new BitmapImage(new Uri(@"/Assets/mario.jpeg", UriKind.Relative)) };
            //ReceivedAdvertisementListBox.Items.Add(test_img);
            //test_img.Visibility = Visibility.Visible;
            if (flag)
            {
                // Calling watcher start will start the scanning if not already initiated by another client
                
                watcher.Start();
                // ReceivedAdvertisementListBox.Items.Clear();
               
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
                dispatcherTimer.Start();
                
            
                ScrollViewer.Visibility = Visibility.Visible;
                
               
                Scroll_2.Height = new GridLength(315);
                Circle_View.Margin = new Thickness(0,0,0,0);
               seacrch_IsActive = new DispatcherTimer();
                seacrch_IsActive.Tick += seacrch_IsActive_Tick;
                seacrch_IsActive.Interval = new TimeSpan(0, 0, 0, 0, 500);
                seacrch_IsActive.Start();
                flag = false;

               
               
            }
            else
            {
                watcher.Stop();
                Search_Circle1.Visibility = Visibility.Visible;
                Search_Circle2.Visibility = Visibility.Collapsed;
                Search_Circle3.Visibility = Visibility.Collapsed;
                Search_Circle4.Visibility = Visibility.Collapsed;
                Search_Circle5.Visibility = Visibility.Collapsed;
                Scroll_2.Height = new GridLength(0);
                Thickness margin = Circle_View.Margin;
                margin.Top = 100;
                Circle_View.Margin = margin;
                flag = true;
                seacrch_IsActive.Stop();
            }
        }

        private async void seacrch_IsActive_Tick(object sender, object e)
        {
            
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                switch(timer++)
                {
                    case 1: Search_Circle2.Visibility = Visibility.Visible;  break;
                    case 2: Search_Circle3.Visibility = Visibility.Visible;  break;
                    case 3: Search_Circle4.Visibility = Visibility.Visible;  break;
                    case 4: Search_Circle5.Visibility = Visibility.Visible; break;
                    case 5: Search_Circle2.Visibility = Search_Circle3.Visibility = Search_Circle4.Visibility = Search_Circle5.Visibility = Visibility.Collapsed;  timer = 1; break;
                }

            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (menu_flag)
            {
                Splitter.IsPaneOpen = true;
                menu_flag = false;
                
                
               
            }
            else if (!menu_flag)
            {
                Splitter.IsPaneOpen = false;
                menu_flag = true;
                
                
                

            }
            else { }

            
         /*   if (toggle_menu_flag)
            {
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("ms-appx://iaM/Assets/MainPage/MainPage_Progress_Square1.png"));
                this.Toggle_Menu.Background = ib;
                toggle_menu_flag = false;
            }
            if(!toggle_menu_flag)
            {
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("ms-appx://iaM/Assets/MainPage/MainPage_Menu.png"));
                this.Toggle_Menu.Background = ib;
                toggle_menu_flag = true;
            }*/
        }
        
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("ms-appx://iaM/Assets/MainPage/MainPage_Menu.png"));
            this.Toggle_Menu.Background = ib;
        }

        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*  if (MenuPage.IsSelected)
              {
                  this.Frame.Navigate(typeof(MenuPage));
              }
              else if (SettingPage.IsSelected)
              {
                  this.Frame.Navigate(typeof(SettingPage));
              }
              else { }*/
        }


        private void ToEditProfile(object sender, SelectionChangedEventArgs e)
        {
            this.Frame.Navigate(typeof(Edit_Profile));
        }




        /*  publisher!!  ****************/



      /*  private async void Background_Start(object sender, RoutedEventArgs e)
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



        }*/
      /*  private void Background_Stop(object sender, RoutedEventArgs e)
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
        }*/


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


        /*  watcher!!  ****************/
      
        /// <summary>
        /// Invoked as an event handler when the Run button is pressed.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
       /* private void RunButton_Click(object sender, RoutedEventArgs e)
        {
           

        }*/
        
        async void dispatcherTimer_Tick(object sender, object e)
        {
             Debug.WriteLine("remover running!!");
            current_client.remove_client();

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {

                for (int i = current_client.client_num; i > 0; i--)
                {
                    Client client = current_client.client_id(i - 1);

                    if (client != null)
                    {

                        if (client.Profile_image != null)
                        {

                            BitmapImage bi = await Convert_module.convert_base64_to_bitmapImage(client.Profile_image);


                            if (i == current_client.client_num)
                            {
                                UserList0_Image.Source = bi;
                                UserList0_Name.Text = client.Nickname;
                                UserList0_Status.Text = client.Status_message;
                            }
                            else if (i == current_client.client_num - 1)
                            {
                                UserList1_Image.Source = bi;
                                UserList1_Name.Text = client.Nickname;
                                UserList1_Status.Text = client.Status_message;
                            }
                            else if (i == current_client.client_num - 2)
                            {
                                UserList2_Image.Source = bi;
                                UserList2_Name.Text = client.Nickname;
                                UserList2_Status.Text = client.Status_message;
                            }
                            else if (i == current_client.client_num - 3)
                            {
                                UserList3_Image.Source = bi;
                                UserList3_Name.Text = client.Nickname;
                                UserList3_Status.Text = client.Status_message;
                            }
                            else
                            {
                                Debug.WriteLine("Not show more than 4 peoples");
                            }
                        }
                    }
                  }

                
            });

        }
        /// <summary>
        /// Invoked as an event handler when the Stop button is pressed.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        /// 


        /*  private void StopButton_Click(object sender, RoutedEventArgs e)
          {
              // Stopping the watcher will stop scanning if this is the only client requesting scan
              watcher.Stop();


          }*/

        /// <summary>
        /// Invoked as an event handler when an advertisement is received.
        /// </summary>
        /// <param name="watcher">Instance of watcher that triggered the event.</param>
        /// <param name="eventArgs">Event data containing information about the advertisement event.</param>
        private async void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {

            Debug.WriteLine("signal!");
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
            int client_num = manufacturerSections.Count+3;

            string[] client_ids = new string[client_num];
            
            if (client_num > 0)
            {
                client_ids[0]="shin";
                client_ids[1] = "mk";
                client_ids[2] = "jh";

                var manufacturerData = manufacturerSections[0];
                var data = new byte[manufacturerData.Data.Length];
                using (var reader = DataReader.FromBuffer(manufacturerData.Data))
                {
                    reader.ReadBytes(data);
                }
                // Print the company ID + the raw data in hex format
                client_ids[3] = string.Format("{0}", Encoding.UTF8.GetString(data));
                Debug.WriteLine(client_ids[3]);
                /*
                for (int i = 0; i < client_num; i++)
                {
                    var manufacturerData = manufacturerSections[i];
                    var data = new byte[manufacturerData.Data.Length];
                    using (var reader = DataReader.FromBuffer(manufacturerData.Data))
                    {
                        reader.ReadBytes(data);
                    }
                    // Print the company ID + the raw data in hex format
                    client_ids[i] = string.Format("{0}", Encoding.UTF8.GetString(data));

                }*/
            }
            // compare pre_member with current member
            current_client.check_exist(client_ids);

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {

                for (int i = current_client.client_num; i > 0; i--)
                {

                    Client client = current_client.client_id(i - 1);

                    //   Debug.WriteLine(client.Id);
                    
                        if (client.Profile_image != null)
                        {

                            BitmapImage bi = await Convert_module.convert_base64_to_bitmapImage(client.Profile_image);

                        if (i == current_client.client_num)
                        {
                            UserList0_Ring.IsActive = false;
                            UserList0_Ring.Visibility = Visibility.Collapsed;
                            UserList0_Image.Source = bi;
                            UserList0_Name.Text = client.Nickname;
                            UserList0_Status.Text = client.Status_message;
                        }
                        else if (i == current_client.client_num - 1)
                        {
                            UserList1_Image.Source = bi;
                            UserList1_Name.Text = client.Nickname;
                            UserList1_Status.Text = client.Status_message;
                        }
                        else if (i == current_client.client_num - 2)
                        {
                            UserList2_Image.Source = bi;
                            UserList2_Name.Text = client.Nickname;
                            UserList2_Status.Text = client.Status_message;
                        }
                        else if (i == current_client.client_num - 3)
                        {
                            UserList3_Image.Source = bi;
                            UserList3_Name.Text = client.Nickname;
                            UserList3_Status.Text = client.Status_message;
                        }
                        else
                        {
                            Debug.WriteLine("Not show more than 4 peoples");
                        }
                    }
                    }

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

        private void Back(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        private void IsOnline(object sender, RoutedEventArgs e)
        {
            
            if (visibility_flag)
                RadioButton.Visibility = Visibility.Visible;
            else
                RadioButton.Visibility = Visibility.Collapsed;
        }

        private async void Check_Online(object sender, RoutedEventArgs e)
        {
            RadioButton.Visibility = Visibility.Collapsed;
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
        private void Check_Offline(object sender, RoutedEventArgs e)
        {
            RadioButton.Visibility = Visibility.Collapsed;
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

        private void To_See_Others_Page(object sender, RoutedEventArgs e)
        {
             Button b = (Button)sender;

             Debug.WriteLine("Tag : {0}",b.Tag);

             int clients_num = current_client.client_num;

             object obj = null;

             switch (b.Tag.ToString())
             {
                 case "0": obj = current_client.client_id(clients_num - 1);  break;
                 case "1": obj = current_client.client_id(clients_num - 2); break;
                 case "2":  obj = current_client.client_id(clients_num - 3); break;
                 case "3": obj = current_client.client_id(clients_num - 4); break;
                 case "4": obj = current_client.client_id(clients_num - 5); break;
                 case "5": obj = current_client.client_id(clients_num - 6); break;
                 case "6": obj = current_client.client_id(clients_num - 7); break;
                 case "7": obj = current_client.client_id(clients_num - 8); break;

                 default: break;
             }

             
             this.Frame.Navigate(typeof(See_Others),obj);
        }

        private void To_Edit_Profile(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Edit_Profile));
        }
    }

    public class List_item
    {
        public BitmapImage UserImage { get; set; }
        public string UserName { get; set; }
        public string UserComment { get; set; }

    }
}
