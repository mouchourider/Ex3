using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ex3.Models
{
    public class Server
    { 
        public TcpListener listener;
        public TcpClient client;
        private string[] valuesFromSim = new string[24];
        public Server(){}

        string[] ValuesFromSim
        {
            get { return this.valuesFromSim; }
            set { this.ValuesFromSim = value; }
        }


        public void Connect(string ip, int port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            listener = new TcpListener(ep);
            listener.Start();
            HandleClient();
        }

        public void HandleClient()
        {
            client = listener.AcceptTcpClient();
        }

        /*public double Get(string requestPath)
        {
            int size = 2048;
            byte[] response = new byte[size];

            if (tcpClient.Connected)
            {
                NetworkStream stream = tcpClient.GetStream();

                byte[] encoded = Encoding.ASCII.GetBytes("get " + requestPath + "\r\n");
                stream.Write(encoded, 0, encoded.Length);
                stream.Flush();
                stream.Read(response, 0, size);
            }

            // getting the value from the response format of the server .
            string num = Encoding.ASCII.GetString(response, 0, size).Split(' ')[2];

            // Get rid of quotation marks and converting the number to Double .
            return Convert.ToDouble(num.Substring(1, num.Length - 2));
        }*/

        public void ReadFromClient(TcpClient client)
        {
            Byte[] bytes;
                NetworkStream ns = client.GetStream();
                if (client.ReceiveBufferSize > 0)
                {
                    bytes = new byte[client.ReceiveBufferSize];
                    ns.Read(bytes, 0, client.ReceiveBufferSize);
                    string msg = Encoding.ASCII.GetString(bytes);
                    EditMessage(msg);
                }
        }

        void EditMessage(string Message)
        {
            string[] splitStr = Message.Split(',');
            this.valuesFromSim = splitStr;
            try
            {
                InfoModel.Instance.lon = Convert.ToDouble(splitStr[0]);
                InfoModel.Instance.lat = Convert.ToDouble(splitStr[1]);
                InfoModel.Instance.alt = Convert.ToDouble(splitStr[9]);
                InfoModel.Instance.rud = Convert.ToDouble(splitStr[21]);
                InfoModel.Instance.thr = Convert.ToDouble(splitStr[23]);
            }
            catch (Exception exception) { }
            
        }

        public void Stop()
        {
            listener.Stop();
        }

    }
}