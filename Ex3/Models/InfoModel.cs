﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


namespace Ex3.Models
{
        public class InfoModel
        {
            private static InfoModel s_instace = null;

            public static InfoModel Instance
            {
                get
                {
                    if (s_instace == null)
                    {
                        s_instace = new InfoModel();
                    }
                    return s_instace;
                }
            }

            public FlightData flightdata { get; private set; }
            public string ip { get; set; }
            public string port { get; set; }
            public int time { get; set; }

            public InfoModel()
            {
                flightdata = new FlightData();
            }

            public const string SCENARIO_FILE = "~/App_Data/{0}.txt";           // The Path of the Secnario

            public void ReadData(string name)
            {
                string path = HttpContext.Current.Server.MapPath(String.Format(SCENARIO_FILE, name));
                if (!File.Exists(path))
                {
                    /*Employee.FirstName = name;
                    Employee.LastName = name;
                    Employee.Salary = 500;*/

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
                    {
                        /*file.WriteLine(Employee.FirstName);
                        file.WriteLine(Employee.LastName);
                        file.WriteLine(Employee.Salary);*/
                    }
                }
                else
                {
                    string[] lines = System.IO.File.ReadAllLines(path);        // reading all the lines of the file
                    flightdata.Location = lines[0];
                    flightdata.Altitude = int.Parse(lines[1]);
                    flightdata.Direction = int.Parse(lines[2]);
                    flightdata.Speed = int.Parse(lines[3]);
            }
            }

        }
 }