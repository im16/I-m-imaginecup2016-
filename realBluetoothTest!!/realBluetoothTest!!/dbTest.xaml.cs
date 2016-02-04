using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace realBluetoothTest__
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    /// 


    public sealed partial class dbTest : Page
    {
        string path;
        SQLite.Net.SQLiteConnection conn;
        Windows.Storage.ApplicationDataContainer localSettings;


        public dbTest()
        {
            this.InitializeComponent();
            Connect_DB();

            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Object value = localSettings.Values["exampleSetting"];
            test.Text = value.ToString();


        }

        private void Connect_DB()
        {
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.splite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<Client_Token>();
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {

            localSettings.Values["exampleSetting"] = textBox.Text;
            Object value = localSettings.Values["exampleSetting"];

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                test.Text = value.ToString();
            });


            //  var add = conn.Insert(new Client_Token() { token_num = textBox.Text });
            //  Debug.WriteLine(path);
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            var query = conn.Table<Client_Token>();
            string result = String.Empty;
            foreach (var item in query)
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

        private async void Contact_Click(object sender, RoutedEventArgs e)
        {
            // Get thumbnail
            try
            {

                StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
                StorageFile file = await assets.GetFileAsync("profile.png");                
   
                // Get thumbnail
                const uint requestedSize = 100;
                const ThumbnailMode thumbnailMode = ThumbnailMode.PicturesView;
                const ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale;
                var thumbnail = await file.GetThumbnailAsync(thumbnailMode, requestedSize, thumbnailOptions);

                var img = new BitmapImage();
                img.SetSource(thumbnail);
                image_show.Source = img;
                image_show.Width=image_show.Height = requestedSize;
        
            }
            catch (FileNotFoundException err)
            {
                Debug.WriteLine(err);
            }


        }

        private async void Image_Click(object sender, RoutedEventArgs e)
        {
            //image_show.Source = await Connect_Server.request_iomage();
            byte[] mem = await Connect_Server.request_image();

            /*
            var bitmapImage = new BitmapImage();

            var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(mem.);
            stream.Seek(0);

            bitmapImage.SetSource(stream);
            image_show.Source = bitmapImage;
            */

            /*
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                Image img = image_show as Image;
                BitmapImage bitmapImage = new BitmapImage();
                img.Width = bitmapImage.DecodePixelWidth = 80;

                var stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(mem.AsBuffer());

                await bitmapImage.SetSourceAsync(stream);
                image_show.Source = bitmapImage;


            });
            */
            var ims = new InMemoryRandomAccessStream();

            var dataWriter = new DataWriter(ims);
            dataWriter.WriteBytes(mem);
            await dataWriter.StoreAsync();
            ims.Seek(0);
            var img = new BitmapImage();
            img.SetSource(ims);
            image_show.Source = img;



            var bmp = new WriteableBitmap(320, 240);
            using (var stream = bmp.PixelBuffer.AsStream())
            {
                Debug.WriteLine("길이는 {0} ", mem.Length);

                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {

                    await stream.WriteAsync(mem, 0, mem.Length);



                });


            }



            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileSavePicker.FileTypeChoices.Add("JPEG files", new List<string>() { ".jpg" });
            fileSavePicker.SuggestedFileName = "image";

            var outputFile = await fileSavePicker.PickSaveFileAsync();

            if (outputFile == null)
            {
                // The user cancelled the picking operation
                return;
            }


            SoftwareBitmap outputBitmap = SoftwareBitmap.CreateCopyFromBuffer(
                        bmp.PixelBuffer,
                        BitmapPixelFormat.Bgra8,
                        bmp.PixelWidth,
                        bmp.PixelHeight
                        );


            SaveSoftwareBitmapToFile(outputBitmap, outputFile);


        }

        private async void SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                // Set additional encoding parameters, if needed
                encoder.BitmapTransform.ScaledWidth = 320;
                encoder.BitmapTransform.ScaledHeight = 240;
                encoder.BitmapTransform.Rotation = Windows.Graphics.Imaging.BitmapRotation.Clockwise90Degrees;
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
                encoder.IsThumbnailGenerated = true;

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    switch (err.HResult)
                    {
                        case unchecked((int)0x88982F81): //WINCODEC_ERR_UNSUPPORTEDOPERATION
                                                         // If the encoder does not support writing a thumbnail, then try again
                                                         // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw err;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }


            }
        }




        private async void Click_Photo_Button(object sender, RoutedEventArgs e)
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



        


       




    }
}
