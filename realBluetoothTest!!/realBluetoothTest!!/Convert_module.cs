using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace realBluetoothTest__
{
    class Convert_module
    {


        // base 64 to bitmapimage
        public static async Task<BitmapImage> convert_base64_to_bitmapImage(string base64string)
        {
            byte[] mem = Convert.FromBase64String(base64string);

            var ims = new InMemoryRandomAccessStream();

            var dataWriter = new DataWriter(ims);
            dataWriter.WriteBytes(mem);
            await dataWriter.StoreAsync();
            ims.Seek(0);
            var img = new BitmapImage();
            img.SetSource(ims);

            return img;

        }

        //stream to base64
        public static async Task<string> streamToString(IRandomAccessStream fileStream)
        {
            string Base64String = "";
            var reader = new DataReader(fileStream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)fileStream.Size);
            byte[] byteArray = new byte[fileStream.Size];
            reader.ReadBytes(byteArray);
            Base64String = Convert.ToBase64String(byteArray);
            //string s = ByteToString(byteArray);

            return Base64String;


        }

        // 바이트 배열을 String으로 변환 
        public static string ByteToString(byte[] strByte)
        {
            string str = Encoding.UTF8.GetString(strByte);
            return str;
        }
        // String을 바이트 배열로 변환 
        public static byte[] StringToByte(string str)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(str);
            return StrByte;
        }



        //save bitmap to file
        public static async void SaveSoftwareBitmapToFile(WriteableBitmap bmp, uint width, uint height)
        {

            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string>() { ".png" });
            fileSavePicker.SuggestedFileName = "image";

            StorageFile outputFile = await fileSavePicker.PickSaveFileAsync();

            if (outputFile == null)
            {
                // The user cancelled the picking operation
                return;
            }

            BitmapImage dd = new BitmapImage();

            SoftwareBitmap outputBitmap = SoftwareBitmap.CreateCopyFromBuffer(
                        bmp.PixelBuffer,
                        BitmapPixelFormat.Bgra8,
                        bmp.PixelWidth,
                        bmp.PixelHeight
                        );


            using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(outputBitmap);

                // Set additional encoding parameters, if needed
                encoder.BitmapTransform.ScaledWidth = width;
                encoder.BitmapTransform.ScaledHeight = height;
                encoder.BitmapTransform.Rotation = Windows.Graphics.Imaging.BitmapRotation.None;
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

    }
}
