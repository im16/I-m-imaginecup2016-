using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iaM.Views
{
    class Client_List
    {
        private ArrayList clients = null;

        public Client_List()
        {
            clients = new ArrayList();
            client_num = 0;

        }

        public int client_num { get; set; }

        public void client_near_information(object sender, EventArgs e)
        {
            object[] obj = (object[])sender;


        }

        public Client client_id(int i)
        {
            Client member = (Client)clients[i];

            return member;

        }

        public void add_client(string id)
        {

            Client new_client = new Client(id);
            clients.Add(new_client);

            this.client_num++;
        }

        public void check_exist(string[] ids)
        {
            //Debug.WriteLine("start");

            if (client_num > 0)
            {

                foreach (string s in ids)
                {
                    //  Debug.WriteLine("id:{0}", s);

                    // check existing member
                    Boolean falg = false;

                    foreach (Client i in clients)
                    {
                        if (i.equals(s))
                        { i.Use_bit = 10; falg = true; }
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

            //  Debug.WriteLine("end");

        }

        // remove existing member(because already left)
        public void remove_client()
        {
            // Debug.WriteLine("remover start!");

            foreach (Client i in clients)
                i.Use_bit -= 1;

            for (int i = 0; i < client_num; i++)
            {
                Client member = (Client)clients[i];

                //  Debug.WriteLine("id: {0} , {1}", member.id, member.use_bit);

                if (member.Use_bit <= 0)
                { clients.Remove(member); client_num--; }
            }

            // System.Diagnostics.Debug.WriteLine("remove end");
        }

    }


}