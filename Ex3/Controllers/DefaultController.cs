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

        // GET: First
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult display(string ip, int port, int? time)
        {

            bool containsInt = ip.Any(char.IsDigit);
            InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            if (time != null)
            {
                InfoModel.Instance.time = (int)time;
                ViewBag.timeLoad = 1;
            }
            else
            {
                InfoModel.Instance.time = 0;
                ViewBag.timeLoad = 0;
            }
            InfoModel.Instance.server.Connect(ip, port);
            if (containsInt) // Format -> ip/port/time -> Show the pass
            {
                ViewBag.Format = 1;
                ViewBag.lon = InfoModel.Instance.lon;
                ViewBag.lat = InfoModel.Instance.lat;

                Session["time"] = InfoModel.Instance.time;
            }
            else // Format -> file/time -> 
            {
                ViewBag.Format = 0;
                //Read datas written in flight1.
            }

            return View();
        }

        [HttpGet]
        public ActionResult save(string ip, int port, int time, int sessionT, string file)
        {
            InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            InfoModel.Instance.time = time;
            InfoModel.Instance.server.Connect(ip, port);

            Session["time"] = time;
            Session["sTime"] = sessionT;



            return View();
        }


        [HttpPost]
        public string GetLocation()
        {
            InfoModel.Instance.server.ReadFromClient(InfoModel.Instance.server.client);
            var emp = InfoModel.Instance;
            System.Diagnostics.Debug.WriteLine("Lon: " + InfoModel.Instance.lon + ", Lat: " + InfoModel.Instance.lat + "\n");
            /*emp.Salary = rnd.Next(1000);*/

            return ToXml(emp);
        }
        [HttpPost]
        public string AddLocation()
        {
            InfoModel.Instance.server.ReadFromClient(InfoModel.Instance.server.client);
            var emp = InfoModel.Instance;
            string result = " ";
            string path = "C:/Users/Francki/Desktop/Ex3/Ex3/App_Data/flight1.xml";
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
                   new XElement("Lon", InfoModel.Instance.lon),
                   new XElement("Lat", InfoModel.Instance.lat),
                   new XElement("Rud", InfoModel.Instance.rud),
                   new XElement("Thr", InfoModel.Instance.thr)));
                xDocument.Save(path);
            }
            System.Diagnostics.Debug.WriteLine("result: " + result);
            return result;
        }

        private string ToXml(InfoModel fd)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteProcessingInstruction("xml", "version='1.0'");

            writer.WriteStartDocument();
            writer.WriteStartElement("Datas");

            fd.ToXml(writer);

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


        // POST: First/Search
        [HttpPost]
        public string Search(string name)
        {
            InfoModel.Instance.ReadData(name);

            return ToXml(InfoModel.Instance);
        }

    }


}