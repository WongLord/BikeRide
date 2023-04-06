using Syncfusion.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRide.Models
{
    public class RidesHistory
    {
        public int RideId { get; set; }
        public DateTime DateTime { get; set; }
        public ObservableCollection<MapLatLng> GpsPoints { get; set; }
        public MapLatLng MapCenter { get; set; }
    }

    public class RidesPoints
    {
        public int RideId { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Username { get; set; }
        public string Time { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

}
