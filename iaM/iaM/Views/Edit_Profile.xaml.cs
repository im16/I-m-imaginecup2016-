using System;
using System.Collections.Generic;
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
    public sealed partial class Edit_Profile : Page
    {
        bool flag_togglebutton_ispublic = true;
        bool menu_flag = true;

        public Edit_Profile()
        {
            Debug.WriteLine("edit!");
            this.InitializeComponent();
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
        private void Button_Show_Album(object sender, RoutedEventArgs e)
        {
            Profile_Album.Visibility = Visibility.Visible;
            Profile_Video.Visibility = Visibility.Collapsed;
            Profile_Contact.Visibility = Visibility.Collapsed;
            Profile_Share.Visibility = Visibility.Collapsed;

            Sign_1.Visibility = Visibility.Collapsed;
            Sign_2.Visibility = Visibility.Visible;
            Sign_3.Visibility = Visibility.Collapsed;
            Sign_4.Visibility = Visibility.Collapsed;
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
            Sign_4.Visibility = Visibility.Visible;
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
