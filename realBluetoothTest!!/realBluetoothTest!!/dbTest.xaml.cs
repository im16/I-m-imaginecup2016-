using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace realBluetoothTest__
{

   

    public sealed partial class dbTest : Page
    {
       //로컬 앱 데이터 저장소
       Windows.Storage.ApplicationDataContainer localSettings;



        public dbTest()
        {
            this.InitializeComponent();
            
           localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
          

        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //id,nickname 설정
            localSettings.Values["my_id"] = textBox_id.Text;
            localSettings.Values["my_nickname"] = textBox_nickname.Text;

            //  var add = conn.Insert(new Client_Token() { token_num = textBox.Text });
            //  Debug.WriteLine(path);

           
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {   
            // 로컬에서 id,nickname 정보 가져옴
            Object value_id = localSettings.Values["my_id"];
            Object value_nickname = localSettings.Values["my_nickname"];

            id.Text = value_id.ToString();
            nickname.Text = value_nickname.ToString();

        }


        private async void Contact_Click(object sender, RoutedEventArgs e)
        {
            // Get thumbnail
            try
            {
                // 로컬 저장소에서 특정 사진 가져와서 thumnail을 만듦
                StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
                StorageFile file = await assets.GetFileAsync("p2.JPG");


                // Get thumbnail
                const uint requestedSize= 100;


                const ThumbnailMode thumbnailMode = ThumbnailMode.SingleItem;
                const ThumbnailOptions thumbnailOptions = ThumbnailOptions.ResizeThumbnail;
                var thumbnail = await file.GetThumbnailAsync(thumbnailMode, requestedSize, thumbnailOptions);
                
                // show image
                var img = new BitmapImage();
                img.SetSource(thumbnail);
                image_show.Source = img;
              //  image_show.Width= requestedSize_width;
               // image_show.Height = requestedSize_height;

                var bmp = new WriteableBitmap(1,1);
                await bmp.SetSourceAsync(thumbnail.CloneStream());


                // thumnail save
               Convert_module.SaveSoftwareBitmapToFile(bmp, thumbnail.OriginalWidth, thumbnail.OriginalHeight);


            }
            catch (FileNotFoundException err)
            {
                Debug.WriteLine(err);
            }


        }

       




        private void Image_Click(object sender, RoutedEventArgs e)
        {
             
        }



        private async void Click_Photo_Up(object sender, RoutedEventArgs e)
        {
            FileOpenPicker open = new FileOpenPicker();

            open.ViewMode = PickerViewMode.Thumbnail;
            open.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".jpeg");
            open.FileTypeFilter.Add(".png");
            
            // Open a stream for the selected file 
            StorageFile file = await open.PickSingleFileAsync();
       
            IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            
            await Connect_Server.addImages(fileStream);
        

            /*
            
            // Ensure a file was selected 
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {

                    await Connect_Server.send_Image_to_Server(fileStream);
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 600; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    image_show.Source = bitmapImage;
                }
            }
            */
        }

        private async void Click_Photo_Down(object sender, RoutedEventArgs e)
        {
            // base 64를 서버에서 받아와서
            string base64 = await Connect_Server.request_image();
            //이미지 띄우기
            image_show.Source = await Convert_module.convert_base64_to_bitmapImage(base64);
        }
    }
}
