using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace iaM.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        //로컬 앱 데이터 저장소
        Windows.Storage.ApplicationDataContainer localSettings;

        public LoginPage()
        {
            this.InitializeComponent();
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        }

        /* 메인페이지로 네비게이션*/
        private void ToMainPage(object sender, RoutedEventArgs e)
        {
            localSettings.Values["id"] = Input_Mail.Text;
            
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Signup(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Signup));
        }

        private void Forgot_Password(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Signup));
        }
    }
}
