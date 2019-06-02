using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Ex3.Models;
using System.IO;

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
        public ActionResult display(string ip, int port, int time)
        {
            bool containsInt = ip.Any(char.IsDigit);
            InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            InfoModel.Instance.time = time;
            InfoModel.Instance.server.Connect(ip, port);
            if (containsInt) // Format -> ip/port/time -> Show the pass
            {
                ViewBag.Format = 1;
                ViewBag.lon = InfoModel.Instance.lon;
                ViewBag.lat = InfoModel.Instance.lat;

                Session["time"] = time;
                Session["Lon"] = InfoModel.Instance.lon;
                Session["Lat"] = InfoModel.Instance.lat;
            }
            else // Format -> file/time -> 
            {
                ViewBag.Format = 0;
                //Read datas written in flight1.
            }

            return View();
        }

        [HttpGet]
        public ActionResult save(string ip, int port, int time, string file)
        {
            InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            InfoModel.Instance.time = time;
            InfoModel.Instance.server.Connect(ip, port);

            Session["time"] = time;



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
            string path = "~/App_Data/flight1.txt";
            string result = ToXml(emp);
            System.IO.File.AppendAllText(path, result);
            return result;
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


        // POST: First/Search
        [HttpPost]
        public string Search(string name)
        {
            InfoModel.Instance.ReadData(name);

            return ToXml(InfoModel.Instance);
        }

    }


}