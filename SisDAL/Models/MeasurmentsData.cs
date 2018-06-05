using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisDAL.Models
{
    public class MeasurmentsData
    {
        public DateTime Time { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }


        public double HudimityVal { get; set; }
        public double HudimityPrecent { get; set; }
        public double IdrVal { get; set; }
        public double WaterVal { get; set; }
        public double BuzzerVal { get; set; }
    }
}
