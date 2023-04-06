namespace BikeRide.Models;

public class BikeRideActions
{
    public int RideActionId { get; set; }
    public DateTime ActionDate { get; set; }
    public string Username { get; set; }
    public string Actionname { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public float Speed { get; set; }
}

public class RideActions //Class para la API
{
    public int UserId { get; set; }
    public int ActionId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
}