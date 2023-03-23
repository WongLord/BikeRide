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
        public DateTime DateTime { get; set; } = DateTime.Now;
        public ObservableCollection<MapLatLng> GpsPoints { get; set; }
        public MapLatLng MapCenter { get; set; }
    }

}
