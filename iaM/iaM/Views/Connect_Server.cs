using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using System;

namespace iaM.Views
{

    class Connect_Server
    {
       
        public static async Task<String> request_client_info(Client client)
        {
            string result = "";
            try
            {
                string url = "http://40.76.6.186:8888/member_find/near";

                var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
                {

                    //object to json
                    StringBuilder sb = new StringBuilder();
                    JsonWriter jw = new JsonTextWriter(new StringWriter(sb));
                    jw.Formatting = Formatting.Indented;
                    jw.WriteStartObject();
                    jw.WritePropertyName("id");
                    jw.WriteValue(client.Id);
                    jw.WriteEndObject();

                    streamWriter.Write(sb.ToString());
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                   result = await streamReader.ReadToEndAsync();
                   // Debug.WriteLine(result);   
                }


            }


            catch (Exception ex)
            {
                // Need to convert int HResult to hex string
                Debug.WriteLine("Error = " + ex.HResult.ToString("X") +
                    "  Message: " + ex.Message);
            }



            return result;


        }

        public static async Task<String> request_image()
        {
            string result = "";
            //result.DecodePixelWidth = 320;

            try
            {
                string url = "http://40.76.6.186:8888/member_find/near/image";

                var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
                {
                    string json = "{\"id\":\"shin\"}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();


                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //  IRandomAccessStream imageStream = (IRandomAccessStream) streamReader.BaseStream;
                    string s = await streamReader.ReadToEndAsync();
                    //result = Encoding.ASCII.GetBytes(s);
                    result = s;

                    Debug.WriteLine(" {0}   ====  {1} ", result, s);

                }
            }

            catch (Exception ex)
            {
                // Need to convert int HResult to hex string
                Debug.WriteLine("Error = " + ex.HResult.ToString("X") +
                    "  Message: " + ex.Message);
            }

            return result;
        }


        public static async Task<bool> addImages(IRandomAccessStream filestream)
        {
            bool success = false;


            string serviceURL = "http://40.76.6.186:8888/member_register/shin";
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

            success = false;

            //Rest request
            HttpClient restClient = new HttpClient();
            restClient.BaseAddress = new Uri("http://40.76.6.186:8888/member_register/shin");
            restClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);


            //falta la autenticacion
            // setAuthorization(restClient, service, WEBSERVICE_REQUEST_TYPE_POST);

            // This is the postdata
            MultipartFormDataContent content = new MultipartFormDataContent(boundary);
            content.Add(new StringContent(boundary));
            StringContent textPart = new StringContent("1234", Encoding.UTF8);
            content.Add(textPart, "project");

            StreamContent imagePart = new StreamContent(filestream.AsStream());
            imagePart.Headers.Add("Content-Type", "image/jpeg");
            content.Add(imagePart, "profile_picture", "111");


            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, serviceURL);
            req.Content = content;
            HttpResponseMessage response = null;
            string responseBodyAsText = "";

            try
            {
                response = await restClient.SendAsync(req);
                response.EnsureSuccessStatusCode();
                responseBodyAsText = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Created)
                {

                    success = true;
                }
            }
            catch (Exception e)
            {
                string err = e.Message;
            }


            return success;
        }






    }
}