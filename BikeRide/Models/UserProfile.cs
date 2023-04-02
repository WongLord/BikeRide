using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRide.Models
{
    public class UserProfile
    {
        public string Username { get; set; }
        public string ProfileDescription { get; set; }
        public string ProfilePhoto { get; set; }
        public int LeftTurns { get; set; }
        public int RightTurns { get; set; }
        public float AvgSpeed { get; set; }
    }

}
