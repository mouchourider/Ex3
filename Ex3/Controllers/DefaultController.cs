using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Ex3.Models;
using System.IO;
using System.Xml.Linq;

namespace Ex3.Controllers
{
    public class DefaultController : Controller
    {
        private static Random rnd = new Random();

        public static List<string> xmlist;

        public static int counter;

        // GET: First
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult display(string ip, int port, int? time)
        {
            System.Net.IPAddress Ip = null;
            if (System.Net.IPAddress.TryParse(ip, out Ip) == false)
            {
                Session["fileName"] = ip;
                Session["time"] = port;
                string path = Server.MapPath("~/App_Data/" + ip + ".xml");
                string xml = System.IO.File.ReadAllText(path);
                xmlist = xml.Split(new string[] { "</Data>" }, StringSplitOptions.None).ToList();
                xmlist[0] = xmlist[0].Remove(0, 47);
                xmlist.RemoveAt(xmlist.Count - 1);
                for(int i = 0; i < xmlist.Count; i++)
                {
                    xmlist[i] = xmlist[i] + "</Data>";
                }
                counter = 0;
                return View("Load");
            }
            bool containsInt = ip.Any(char.IsDigit);
            //InfoModel.Instance.ip = ip;
            //InfoModel.Instance.port = port.ToString();
            if (time != null)
            {
                //InfoModel.Instance.time = (int)time;
                ClientSide.Instance.Time =(int)time;
                ViewBag.timeLoad = 1;
            }
            else
            {
                InfoModel.Instance.time = 0;
                ViewBag.timeLoad = 0;
            }
            //InfoModel.Instance.server.Connect(ip, port);
            ClientSide.Instance.Ip = ip;
            ClientSide.Instance.Port = port;
            if(!ClientSide.Instance.IsConnectedToSimulator)
            ClientSide.Instance.Connect();
            else
            {
                ClientSide.Instance.Disconnect();
                ClientSide.Instance.Connect();
            }
            SimulatorInfo si = ClientSide.Instance.SampleFlightValues();
            if (containsInt) // Format -> ip/port/time -> Show the pass
            {
                ViewBag.Format = 1;
                ViewBag.lon = InfoModel.Instance.lon;
                ViewBag.lat = InfoModel.Instance.lat;

                Session["time"] = InfoModel.Instance.time;
            }

            return View();
        }

        [HttpGet]
        public ActionResult save(string ip, int port, int time, int sessionT, string file)
        {
            //InfoModel.Instance.ip = ip;
            //InfoModel.Instance.port = port.ToString();
            ///InfoModel.Instance.time = time;
            string path = Server.MapPath("~/App_Data/"+file+".xml");
            ClientSide.Instance.Ip = ip;
            ClientSide.Instance.Port = port;
            ClientSide.Instance.Time = (int)time;
            if (!ClientSide.Instance.IsConnectedToSimulator)
                ClientSide.Instance.Connect();
            else
            {
                ClientSide.Instance.Disconnect();
                ClientSide.Instance.Connect();
            }
            if (!(new FileInfo(path).Length == 0))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                di.Delete(true);
            }
            //InfoModel.Instance.server.Connect(ip, port);

            Session["time"] = time;
            Session["sTime"] = sessionT;

            return View();
        }


        [HttpPost]
        public string GetLocation()
        {
            //InfoModel.Instance.server.ReadFromClient(InfoModel.Instance.server.client);
            SimulatorInfo si = ClientSide.Instance.SampleFlightValues();
            //var emp = InfoModel.Instance;
            //var emp = ClientSide.Instance;
            //System.Diagnostics.Debug.WriteLine("Lon: " + InfoModel.Instance.lon + ", Lat: " + InfoModel.Instance.lat + "\n");
            System.Diagnostics.Debug.WriteLine("Lon: " + si.Lon + ", Lat: " + si.Lat + "\n");
            /*emp.Salary = rnd.Next(1000);*/

            return ToXml(si.Lon, si.Lat, si.Rud, si.Thr, 0);
        }
        [HttpPost]
        public void AddLocation()
        {
            //InfoModel.Instance.server.ReadFromClient(InfoModel.Instance.server.client);
            SimulatorInfo si = ClientSide.Instance.SampleFlightValues();
            var emp = InfoModel.Instance;
            string result = " ";
            string path = Server.MapPath("~/App_Data/flight1.xml");
            if (new FileInfo(path).Length <= 1)
            {
                ToXml(InfoModel.Instance, path);
            }
            else
            {
                XDocument xDocument = XDocument.Load(path);
                XElement root = xDocument.Element("Datas");
                IEnumerable<XElement> rows = root.Descendants("Data");
                XElement firstRow = rows.First();

                firstRow.AddBeforeSelf(
                   new XElement("Data",
                   new XElement("Lon", si.Lon),
                   new XElement("Lat", si.Lat),
                   new XElement("Rud", si.Rud),
                   new XElement("Thr", si.Thr)));
                xDocument.Save(path);
            }
        }

        private string ToXml(InfoModel fd)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Datas");

            fd.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
        private string ToXml(ClientSide fd)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Datas");

            fd.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

        private string ToXml(double lat, double lon, double throttle, double rudder, double isEnd)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Location");
            writer.WriteElementString("Lon", lon.ToString());
            writer.WriteElementString("Lat", lat.ToString());
            writer.WriteElementString("Throttle", throttle.ToString());
            writer.WriteElementString("Rudder", rudder.ToString());
            writer.WriteElementString("IsEnd", isEnd.ToString());
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

        private void ToXml(InfoModel fd, string path)
        {
            //Initiate XML stuff
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlWriter writer = XmlWriter.Create(path, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Datas");

            fd.ToXml(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        [HttpPost]
        public string LoadFlightData()
        {
            string ret = xmlist[counter];
            counter++;
            if(counter >= xmlist.Count)
            {
                return "stop";
            }
            return ret;
        }


        // POST: First/Search
        [HttpPost]
        public string Search(string name)
        {
            InfoModel.Instance.ReadData(name);

            return ToXml(InfoModel.Instance);
        }

    }


}