using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSmartHomeMonitoringApp.Models
{
    public class SmartHomeModel
    {
        public string DevId { get; set; }
        public DateTime CurrTime { get; set; }
        public double Temp { get; set; }
        public double Humid { get; set; }

        

    }
}
