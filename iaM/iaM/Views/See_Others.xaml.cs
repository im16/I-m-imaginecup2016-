﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace iaM.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class See_Others : Page
    {
        Client client=null;
        bool flag_togglebutton_ispublic = true;
        bool photo_progress_flag = true;
        bool menu_flag = true;
        private BackgroundWorker bw = new BackgroundWorker();

        public See_Others()
        {
            this.InitializeComponent();

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            client = e.Parameter as Client;

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    status_message.Text = client.Status_message;
                    top_Name.Text = client.Nickname;
                    BitmapImage bi = await Convert_module.convert_base64_to_bitmapImage(client.Profile_image);
                    profile_image.Fill = new ImageBrush { ImageSource = bi };
                }
                catch(Exception err)
                {
                    Debug.WriteLine(err);
                }



            });

            Debug.WriteLine("{0} , {1} , {2}", client.Nickname, client.Phone_number, client.Status_message);

            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();

        }


        private async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine("background task start");

            BackgroundWorker worker = sender as BackgroundWorker;


            await Connect_Server.request_client_more_info(client);

            Debug.WriteLine("{0} , {1} , {2}", client.Nickname, client.Phone_number, client.Status_message);

            Debug.WriteLine("{0}", client.Images.Length);

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                photo_progress_flag = false;

                progress_photo.IsActive = false;
                progress_photo.Visibility = Visibility.Collapsed;
                progress_photo_back.Visibility = Visibility.Collapsed;

                for (int i = 1; i < client.Images.Length; i++)
                {
                    BitmapImage image = new BitmapImage();
                    image = await Convert_module.convert_base64_to_bitmapImage(client.Images.GetValue(i).ToString());


                    if (i == 1)
                        photo1.Background = new ImageBrush { ImageSource = image };
                    else if (i == 2)
                        photo2.Background = new ImageBrush { ImageSource = image };
                    else if (i == 3)
                        photo3.Background = new ImageBrush { ImageSource = image };
                    else if (i == 4)
                        photo4.Background = new ImageBrush { ImageSource = image };
                    else if (i == 5)
                        photo5.Background = new ImageBrush { ImageSource = image };
                    else if (i == 6)
                        photo6.Background = new ImageBrush { ImageSource = image };

                }

                Debug.WriteLine("success image show");
            });
        }

        private void kn_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            UIElement element = sender as UIElement;
            CompositeTransform transform = element.RenderTransform as CompositeTransform;
            if (transform != null)
            {

                transform.ScaleX *= e.Delta.Scale;
                transform.ScaleY *= e.Delta.Scale;
                transform.Rotation += e.Delta.Scale / Math.PI;
                transform.TranslateX += e.Delta.Translation.X;
                transform.TranslateY += e.Delta.Translation.Y;
            }
        }



        private void Button_Menu_Click(object sender, RoutedEventArgs e)
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

        }

        private void Button_Home_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Button_Show_Contact(object sender, RoutedEventArgs e)
        {
            Profile_Album.Visibility = Visibility.Collapsed;
            Profile_Video.Visibility = Visibility.Collapsed;
            Profile_Contact.Visibility = Visibility.Visible;
            Profile_Share.Visibility = Visibility.Collapsed;

            Sign_1.Visibility = Visibility.Visible;
            Sign_2.Visibility = Visibility.Collapsed;
            Sign_3.Visibility = Visibility.Collapsed;
            Sign_4.Visibility = Visibility.Collapsed;
        }
        private async void Button_Show_Album(object sender, RoutedEventArgs e)
        {
            Profile_Album.Visibility = Visibility.Visible;
            Profile_Video.Visibility = Visibility.Collapsed;
            Profile_Contact.Visibility = Visibility.Collapsed;
            Profile_Share.Visibility = Visibility.Collapsed;

            Sign_1.Visibility = Visibility.Collapsed;
            Sign_2.Visibility = Visibility.Visible;
            Sign_3.Visibility = Visibility.Collapsed;
            Sign_4.Visibility = Visibility.Collapsed;

            if (photo_progress_flag)
            {
                progress_photo.IsActive = true;
                progress_photo_back.Visibility = Visibility.Visible;
                progress_photo.Visibility = Visibility.Visible;
            }
            else
            {
                progress_photo.IsActive = false;
                progress_photo.Visibility = Visibility.Collapsed;
                progress_photo_back.Visibility = Visibility.Collapsed;

                for (int i = 1; i < client.Images.Length; i++)
                {
                    BitmapImage image = new BitmapImage();
                    image = await Convert_module.convert_base64_to_bitmapImage(client.Images.GetValue(i).ToString());


                    if (i == 1)
                        photo1.Background = new ImageBrush { ImageSource = image };
                    else if (i == 2)
                        photo2.Background = new ImageBrush { ImageSource = image };
                    else if (i == 3)
                        photo3.Background = new ImageBrush { ImageSource = image };
                    else if (i == 4)
                        photo4.Background = new ImageBrush { ImageSource = image };
                    else if (i == 5)
                        photo5.Background = new ImageBrush { ImageSource = image };
                    else if (i == 6)
                        photo6.Background = new ImageBrush { ImageSource = image };

                }

            }


        }
        private void Button_Show_Video(object sender, RoutedEventArgs e)
        {
            Profile_Album.Visibility = Visibility.Collapsed;
            Profile_Video.Visibility = Visibility.Visible;
            Profile_Contact.Visibility = Visibility.Collapsed;
            Profile_Share.Visibility = Visibility.Collapsed;

            Sign_1.Visibility = Visibility.Collapsed;
            Sign_2.Visibility = Visibility.Collapsed;
            Sign_3.Visibility = Visibility.Visible;
            Sign_4.Visibility = Visibility.Collapsed;
        }
        private void Button_Show_SNS(object sender, RoutedEventArgs e)
        {
            Profile_Album.Visibility = Visibility.Collapsed;
            Profile_Video.Visibility = Visibility.Collapsed;
            Profile_Contact.Visibility = Visibility.Collapsed;
            Profile_Share.Visibility = Visibility.Visible;

            Sign_1.Visibility = Visibility.Collapsed;
            Sign_2.Visibility = Visibility.Collapsed;
            Sign_3.Visibility = Visibility.Collapsed;
            Sign_4.Visibility = Visibility.Visible ;
        }

       
        private void Back(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }
        private void ToggleButton_IsPublic_Change(object sender, RoutedEventArgs e)
        {
            if (flag_togglebutton_ispublic)
            {

                Image ib = new Image();
                ib.Source = new BitmapImage(new Uri("ms-appx://iaM/Assets/MainPage/MainPage_Pane_Toggle_Off.png"));
                this.ToggleButton_IsPublic.Source = ib.Source;
                flag_togglebutton_ispublic = false;
            }
            else
            {
                Image ib = new Image();
                ib.Source = new BitmapImage(new Uri("ms-appx://iaM/Assets/MainPage/MainPage_Pane_Toggle_On.png"));
                this.ToggleButton_IsPublic.Source = ib.Source;
                flag_togglebutton_ispublic = true;
            }

        }


    }
}
