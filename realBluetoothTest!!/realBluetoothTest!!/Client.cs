﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace realBluetoothTest__
{
    class Client_List
    {
        private ArrayList clients=null;
        
        public Client_List()
        {
            clients= new ArrayList();
            client_num = 0;
        }

        public int client_num{get;set;}

        public string client_id(int i)
        {
            Client member = (Client)clients[i];

            return member.id;
            
        }

        public void add_client(string id)
        {   

            Client new_client = new Client(id);
            clients.Add(new_client);

            this.client_num++;
        }

        public void check_exist(string[] ids)
        {
            Debug.WriteLine("start");

            if (client_num > 0)
            {
                
                foreach (string s in ids)
                {
                    Debug.WriteLine("id:{0}",s);

                    // check existing member
                    Boolean falg = false;

                    foreach (Client i in clients)
                    {
                        if (i.equals(s))
                        { i.use_bit = 10; falg = true; }
                    }

                    // add new member
                    if (!falg)
                        add_client(s);

                }

            }

            else
            {
                foreach (string s in ids)
                {
                    add_client(s);
                }
            }

            Debug.WriteLine("end");

        }

        // remove existing member(because already left)
        public void remove_client()
        {
            Debug.WriteLine("remover start!");

            foreach (Client i in clients)
                i.use_bit -= 1;

            for (int i=0; i<client_num; i++)
            {
                Client member =(Client) clients[i];

                Debug.WriteLine("id: {0} , {1}", member.id, member.use_bit);

                if (member.use_bit<=0)
                { clients.Remove(member); client_num--;  }
            }

            System.Diagnostics.Debug.WriteLine("remove end");
        }

    }


    class Client
    {
       
        public string id { get; set; }
        public int use_bit { get; set; }

        public Client(string id)
        {
            this.id = id;
            use_bit = 10;
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
