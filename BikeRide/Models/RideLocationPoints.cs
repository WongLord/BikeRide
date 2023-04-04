namespace BikeRide.Models;

public class RideLocationPoints
{
    public int RideId { get; set; }
    public List<RideCoordinates> RideCoordinates { get; set; }
}

public class RideCoordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}