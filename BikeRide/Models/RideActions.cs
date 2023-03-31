using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRide.Models
{
    public class RideActions
    {
        public DateTime ActionDate { get; set; }
        public string Username { get; set; }
        public string Actionname { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Speed { get; set; }
    }

}
