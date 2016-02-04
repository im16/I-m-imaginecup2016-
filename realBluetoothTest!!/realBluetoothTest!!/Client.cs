using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace realBluetoothTest__
{
   
    class Client
    {

        private BackgroundWorker bw = new BackgroundWorker();

        public string id { get; set; }
        public int use_bit { get; set; }
        public string nickName { get; set; }
        public string phoneNum { get; set; }
        public string status_message { get; set; }
        public string other_sns { get; set; }
        public ArrayList pictures { get; set; }
        public ArrayList videos { get; set; }

        public Client(string id)
        {
            this.id = id;
            use_bit = 10;
                       
            //request found client information(background task)
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync();

        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Connect_Server.request_client_info(this);
        }



        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Error == null))
            {
                Debug.WriteLine("client {0} not found", this.id);
            }

            else
            {
                Debug.WriteLine("Done!!");
            }
        }

        public void setClient_Info(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jUser = jObject["My"];
            nickName = (string)jUser["nickName"];
            phoneNum = (string)jUser["phoneNum"];
            status_message = (string)jUser["status_message"];
            other_sns = (string)jUser["other_sns"];

        }

        public bool equals(string id)
        {
            if (this.id == id)
                return true;
            else
                return false;
        }

        public void reduce_use()
        {
            use_bit--;
        }
    }

}
