using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace BikeRide.ViewModels;

public class MainViewModel : BaseViewModel
{
    public const int USER_ID = 1;

    public MainViewModel()
    {
        Task.Run(() => GetUserProfile(USER_ID)).Wait();
        Task.Delay(500).Wait();
        Task.Run(() => GetOverviewItems(USER_ID)).Wait();
        Task.Delay(500).Wait();
        Task.Run(() => GetRidesHistory(USER_ID)).Wait();
    }

    #region Home Tab Code
    public string ProfilePhotoSource { get; set; } //= "dotnet_bot";
    public string ProfileName { get; set; } //= "Leonardo";
    public string ProfileDetails { get; set; } //= "Bike Ride Manager";

    public int LeftTurns { get; set; } //= 89;
    public double AvgSpeed { get; set; } //= 20;
    public int RightTurns { get; set; } //= 71;

    public async Task GetUserProfile(int userId)
    {
        HttpResponseMessage res = await ApiCalls.GETResponse("GetRideActions", $"?UserId={userId}");
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadAsStringAsync();
            UserProfile up = JsonConvert.DeserializeObject<UserProfile>(result);

            ProfilePhotoSource = up.ProfilePhoto;
            ProfileName = up.Username;
            ProfileDetails = up.ProfileDescription;
            LeftTurns = up.LeftTurns;
            RightTurns = up.RightTurns;
            AvgSpeed = up.AvgSpeed;
        }
    }

    public List<OverviewItem> OverviewItems { get; set; }

    public async Task GetOverviewItems(int userId)
    {
        //OverviewItems = new();
        List<OverviewItem> items = new();
        HttpResponseMessage res = await ApiCalls.GETResponse("GetRideActions", $"?UserId={userId}");
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadAsStringAsync();
            List<BikeRideActions> actions = JsonConvert.DeserializeObject<List<BikeRideActions>>(result);
            

            foreach(var a in actions)
            {
                string Road = "Could not find road.";
                res = await ApiCalls.GETRoad(a.Latitude, a.Longitude);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    JObject o = JObject.Parse(json);
                    Road = (string)o.SelectToken("address.road");
                }

                items.Add(new OverviewItem
                {
                    IconSource = a.Actionname == "Turn Left" ? "turn_left" : a.Actionname == "Turn Right" ? "turn_right" : "stop_sign",
                    Title = a.Actionname,
                    Details = $"At {Road} road.",
                    Speed = a.Speed.ToString(".0#")
                });
            }
        }
        OverviewItems = items;
    }
    #endregion

    #region Rides Tab Code
    public List<RidesHistory> Rides { get; set; } 

    public async Task GetRidesHistory(int userId)
    {
        List<RidesHistory> _rides = new();
    
        HttpResponseMessage res = await ApiCalls.GETResponse("GetRideLocPoints", $"?UserId={userId}");
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadAsStringAsync();
            List<RidesPoints> ridesPoints = JsonConvert.DeserializeObject<List<RidesPoints>>(result);

            var groupedRides = ridesPoints.GroupBy(r => r.StartDateTime).Select(g => g.ToList()).ToList();

            foreach (var g in groupedRides)
            {
                ObservableCollection<MapLatLng> Points = new();
                DateTime _startDateTime = DateTime.Now;
                g.ForEach((p) =>
                {
                    _startDateTime = p.StartDateTime;
                    Points.Add(new MapLatLng
                    {
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    });
                });

                _rides.Add(new RidesHistory
                {
                    DateTime = _startDateTime,
                    GpsPoints = Points
                });

                _rides.ForEach((p) => { p.MapCenter = p.GpsPoints[(p.GpsPoints.Count / 2) - 1]; });
            }
        }

        Rides = _rides;
    }
    #endregion

    #region Settings Bluetooh Tab Code
    //Bluetooth setting en BaseViewModel
    #endregion
}