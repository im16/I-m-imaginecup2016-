
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;
using System;
using System.Net.Http;
using System.Net;
using System.Text;

namespace realBluetoothTest__
{
    class Message
    {
        public string user { get; set; }
    }

    class Connect_Server
    {
        public static async void Request_Json()
        {
            try
            {
                string url = "http://40.76.6.186:8888/member_register";

                HttpClient httpclient = new HttpClient();
                HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpclient.DefaultRequestHeaders.Add("name", "value");
                httpclient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                webrequest.Method = "POST";
                HttpWebResponse response = (HttpWebResponse)await webrequest.GetResponseAsync();
                StreamReader streamReader1 = new StreamReader(response.GetResponseStream());
                Debug.WriteLine(streamReader1.ReadLine());
            }

            catch (Exception ex)

            {
                // Need to convert int HResult to hex string
                Debug.WriteLine("Error = " + ex.HResult.ToString("X") +
                    "  Message: " + ex.Message);
            }

         
            


        }
    }
}