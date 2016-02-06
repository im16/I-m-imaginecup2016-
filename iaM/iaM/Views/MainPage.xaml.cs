using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿은 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 에 문서화되어 있습니다.

namespace iaM.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool flag = true;
        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void ProgressRing_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

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

        private void PointEnter(object sender, PointerRoutedEventArgs e)
        {

            this.Background = new SolidColorBrush(Colors.LightCyan);
        }
    }
}
