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
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace realBluetoothTest__
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class dbTest : Page
    {
        string path;
        SQLite.Net.SQLiteConnection conn;

        public dbTest()
        {
            this.InitializeComponent();
            Connect_DB();
           
        }

        private void Connect_DB()
        {
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.splite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<Client_Token>();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var add = conn.Insert(new Client_Token() { token_num = textBox.Text });
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            var query = conn.Table<Client_Token>();
            string result = String.Empty;
            foreach(var item in query)
            {
                result = String.Format("{0} : {1}", item.ID, item.token_num);
                Debug.WriteLine(result);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            conn.DropTable<Client_Token>();
            conn.CreateTable<Client_Token>();
            conn.Dispose();
            conn.Close();
            Connect_DB();
        }
    }
}
