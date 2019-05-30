using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Ex3.Models
{
    public class FlightData
    {
        public float Lon { get; set; }
        public float Lat { get; set; }
        public int Altitude { get; set; }
        public int Direction { get; set; }
        public int Speed { get; set; }

        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("Data");
            writer.WriteElementString("Location", this.Lon.ToString());
            writer.WriteElementString("Location", this.Lat.ToString());
            writer.WriteElementString("Altitude", this.Altitude.ToString());
            writer.WriteElementString("Direction", this.Direction.ToString());
            writer.WriteElementString("Speed", this.Speed.ToString());
            writer.WriteEndElement();
        }
    }
}