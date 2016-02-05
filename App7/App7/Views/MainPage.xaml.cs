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

namespace App7.Views
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

        }

        /*   private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
           {
               // Clear the status block when navigating scenarios.
               NotifyUser(String.Empty, NotifyType.StatusMessage);

               ListBox scenarioListBox = sender as ListBox;
               Scenario s = scenarioListBox.SelectedItem as Scenario;
               if (s != null)
               {
                   ScenarioFrame.Navigate(s.ClassType);
                   if (Window.Current.Bounds.Width < 640)
                   {
                       Splitter.IsPaneOpen = false;
                   }
               }
           }

           public List<Scenario> Scenarios
           {
               get { return this.scenarios; }
           }

           /// <summary>
           /// Used to display messages to the user
           /// </summary>
           /// <param name="strMessage"></param>
           /// <param name="type"></param>
           public void NotifyUser(string strMessage, NotifyType type)
           {
               switch (type)
               {
                   case NotifyType.StatusMessage:
                       StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                       break;
                   case NotifyType.ErrorMessage:
                       StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                       break;
               }
               StatusBlock.Text = strMessage;

               // Collapse the StatusBlock if it has no text to conserve real estate.
               StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
               if (StatusBlock.Text != String.Empty)
               {
                   StatusBorder.Visibility = Visibility.Visible;
                   StatusPanel.Visibility = Visibility.Visible;
               }
               else
               {
                   StatusBorder.Visibility = Visibility.Collapsed;
                   StatusPanel.Visibility = Visibility.Collapsed;
               }
           }
           */


        /*  private void BackRadioButton_Click(object sender, RoutedEventArgs e)
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
          */


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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuPage.IsSelected)
            {
                this.Frame.Navigate(typeof(MenuPage));
            }
            else if (SettingPage.IsSelected)
            {
                this.Frame.Navigate(typeof(SettingPage));
            }
            else { }
        }

        /*  public enum NotifyType
          {
              StatusMessage,
              ErrorMessage
          };

          public class ScenarioBinding : IValueConverter
          {
              public object Convert(object value, Type targetType, object parameter, string language)
              {
                  Scenario s = value as Scenario;
                  return (MainPage.Current.Scenarios.IndexOf(s) + 1) + ") " + s.Title;
              }

              public object ConvertBack(object value, Type targetType, object parameter, string language)
              {
                  return true;
              }
          }*/
    }
}
