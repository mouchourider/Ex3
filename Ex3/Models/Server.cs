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
        private float lon=0.0f;
        private float lat=0.0f;
        private bool shouldContinue = true;
        private string[] valuesFromSim = new string[23];
 
        string[] ValuesFromSim
        {
            get {
                return this.valuesFromSim;
            }
            set {
                this.ValuesFromSim = value;
            }
        }
        public float Lon
        {
            get
            {
                return lon;
            }
            set
            {
                lon = value;
            }
        }

        public float Lat
        {
            get
            {
                return lat;
            }
            set
            {
                lat = value;
            }
        }
        public void Connect(string ip, int port)
        {
            shouldContinue = true;
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            TcpListener listener = new TcpListener(ep);
            listener.Start();
            //MessageBox.Show("after start");
            HandleClient(listener);
        }

        public void HandleClient(TcpListener listener)
        {
            TcpClient client = listener.AcceptTcpClient();
            //MessageBox.Show("after connect");
            Thread thread = new Thread(() => ReadFromClient(client, listener));
            thread.Start();
        }

        void ReadFromClient(TcpClient client, TcpListener listener)
        {
            Byte[] bytes;
            while (shouldContinue)
            {
                NetworkStream ns = client.GetStream();
                if (client.ReceiveBufferSize > 0)
                {
                    bytes = new byte[client.ReceiveBufferSize];
                    ns.Read(bytes, 0, client.ReceiveBufferSize);
                    string msg = Encoding.ASCII.GetString(bytes); //the message incoming
                    EditMessage(msg);
                    //MessageBox.Show("the func");
                }
            }
            client.Close();
            listener.Stop();
        }

        void EditMessage(string Message)
        {
            string[] splitwords = Message.Split(',');
            this.valuesFromSim = splitwords;
            try
            {
                Lon = float.Parse(splitwords[0]);
                Lat = float.Parse(splitwords[1]);
            }
            catch (Exception E)
            {
            }
            //MessageBox.Show(valuesFromSim[0]);
        }

        public void Stop()
        {
            this.shouldContinue = false;
        }

    }
}