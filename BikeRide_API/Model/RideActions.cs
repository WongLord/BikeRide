namespace BikeRide_API.Model
{
    public class RideActions
    {
        public int UserId { get; set; }
        public int ActionId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
    }
}
