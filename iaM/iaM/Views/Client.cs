﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;

namespace iaM.Views
{

    class Client : IDisposable
    {

        private BackgroundWorker bw = new BackgroundWorker();

        private string id;
        private int use_bit;
        private string nickname;
        private string phone_number;
        private string status_message;
        private string other_sns;
        private string profile_image;
        private Array images;

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public int Use_bit
        {
            get
            {
                return use_bit;
            }

            set
            {
                use_bit = value;
            }
        }

        public string Nickname
        {
            get
            {
                return nickname;
            }

            set
            {
                nickname = value;
            }
        }

        public string Phone_number
        {
            get
            {
                return phone_number;
            }

            set
            {
                phone_number = value;
            }
        }

        public string Status_message
        {
            get
            {
                return status_message;
            }

            set
            {
                status_message = value;
            }
        }

        public string Other_sns
        {
            get
            {
                return other_sns;
            }

            set
            {
                other_sns = value;
            }
        }

        public string Profile_image
        {
            get
            {
                return profile_image;
            }

            set
            {
                profile_image = value;
            }
        }

        public Array Images
        {
            get
            {
                return images;
            }

            set
            {
                images = value;
            }
        }

        public Client(string id)
        {
            this.Id = id;
            Use_bit = 10;

            //request found client information(background task)
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync();

        }

        private async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            string s = await Connect_Server.request_client_info(this);
            Debug.WriteLine(s);
            this.setClient_info(s);
        }


        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Error == null))
            {
                Debug.WriteLine("client {0} not found", this.Id);
            }

            else
            {
                Debug.WriteLine("Done!!");
            }
        }

        public void setClient_info(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jUser = jObject["user"];
            Nickname = (string)jUser["nickname"];
            Status_message = (string)jUser["status_message"];
            Profile_image = (string)jUser["profile_image"];
        }


        public void setClient_more_info(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jUser = jObject["user"];
            Nickname = (string)jUser["nickname"];
            Phone_number = (string)jUser["phone_number"];
            Status_message = (string)jUser["status_message"];
            Other_sns = (string)jUser["other_sns"];
            Images = jUser["profile_image"].ToArray();

        }


        public bool equals(string id)
        {
            if (this.Id == id)
                return true;
            else
                return false;
        }

        public void reduce_use()
        {
            Use_bit--;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool v)
        {
            throw new NotImplementedException();
        }
    }

}