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

// 빈 페이지 항목 템플릿은 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 에 문서화되어 있습니다.

namespace App7
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool flag = true;

        public MainPage()
        {
            this.InitializeComponent();
            
        }

     /*   private void click_menu(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MenuPage));
        }
        private void click_find(object sender, RoutedEventArgs e)
        {
            bool A = true;
            if (A)
            {
                this.second.Visibility = Visibility.Collapsed;
                A = false;
            }
            if(A)
            {
                this.second.Visibility = Visibility.Visible;
                A = true;
            }
        }
        private void click_setting(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingPage));
        }

    */

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          /*  LoaderStoryBoard.Begin();
            LoaderRing.Visibility = Visibility.Visible;*/
            
        }

        private void BackRadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HamburgerRadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HomeRadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsRadioButton_Click(object sender, RoutedEventArgs e)
        {

        }

      

        private void ProgressRing_Click(object sender, RoutedEventArgs e)
        {
            
            if (flag)
            {
                Storyboard1.Pause();
                flag = false;
            }
            else
            {
                Storyboard1.Begin();
                flag = true;
            }
        }
    }
}
