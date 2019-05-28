using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ex3.Models
{
    public class InfoServer
    {
        private static InfoServer s_instace = null;

        public static InfoServer Instance
        {
            get
            {
                if (s_instace == null)
                {
                    s_instace = new InfoServer();
                }
                return s_instace;
            }
        }

        public Server server
        {
            get;
            private set;
        }

        public string ip
        {  get;
           set;
        }

        public string port
        {  get;
           set;
        }

        public int time
        {  get;
           set;
        }

        public InfoServer()
        {
            server = new Server();
        }

    }
}