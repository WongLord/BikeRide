﻿using System.Collections.ObjectModel;

namespace BikeRide_API.Model;

public class Rides
{
    public int UserId { get; set; }
    public int ActionId { get; set; }
    public int RideId { get; set; }
    public DateTime EndDateTime { get; set; }
}
