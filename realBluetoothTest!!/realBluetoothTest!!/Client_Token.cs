using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace realBluetoothTest__
{
    public class Client_Token
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string token_num { get; set; }


    }
}
