namespace BikeRide.ViewModels;

public class MainViewModel : BaseViewModel
{
    private const int USER_ID = 1;
    public MainViewModel()
    {
        //Rides.ForEach((p) => { p.MapCenter = p.GpsPoints[(p.GpsPoints.Count / 2) - 1]; });

        Task.Run(()=>GetOverviewItems(USER_ID)).Wait();
        //Task.Delay(1000).Wait();
        Task.Run(() => GetRidesHistory(USER_ID)).Wait();
    }

    #region Home Tab Code
    public string ProfilePhotoSource { get; set; } = "dotnet_bot";
    public string ProfileName { get; set; } = "Leonardo";
    public string ProfileDetails { get; set; } = "Bike Ride Manager";

    public int LeftTurns { get; set; } = 89;
    public int AvgSpeed { get; set; } = 20;
    public int RightTurns { get; set; } = 71;

    public List<OverviewItem> OverviewItems { get; set; }

    public async Task GetOverviewItems(int userId)
    {
        //OverviewItems = new();
        List<OverviewItem> items = new();
        HttpResponseMessage res = await ApiCalls.GETResponse("GetRideActions", $"?UserId={userId}");
        if (res.IsSuccessStatusCode)
        {
            var result = await res.Content.ReadAsStringAsync();
            List<RideActions> actions = JsonConvert.DeserializeObject<List<RideActions>>(result);
            

            actions.ForEach((a) => {
                items.Add(new OverviewItem
                {
                    IconSource = a.Actionname == "Turn Left" ? "turn_left": a.Actionname == "Turn Right" ? "turn_right":"stop_sign",
                    Title = a.Actionname,
                    Details = $"You {a.Actionname} at {a.Latitude}, {a.Longitude}",
                    Speed = a.Speed.ToString(".0#")
                });
            });
        }
        OverviewItems = items;
    }
    #endregion

    #region Rides Tab Code
    public List<RidesHistory> Rides { get; set; } = new List<RidesHistory>
    {
        new RidesHistory
        {
            DateTime = DateTime.Now,
            GpsPoints = new ObservableCollection<MapLatLng>
            {
                new MapLatLng { Latitude = 31.292736570443644, Longitude=-110.94944681504553 },
                new MapLatLng { Latitude = 31.29214433096709, Longitude=  -110.94933113906677 },
                new MapLatLng { Latitude = 31.292733306421663, Longitude=-110.94768725929922 },
                new MapLatLng{ Latitude=31.292360966046672, Longitude=-110.94639988349347},
                new MapLatLng{ Latitude=31.291761833444056, Longitude=-110.94554823493918},
                new MapLatLng{ Latitude=31.291460573238986, Longitude=-110.94563141921658}
            }
        },
        new RidesHistory
        {
            DateTime = DateTime.Now,
            GpsPoints = new ObservableCollection<MapLatLng>
            {
                new MapLatLng { Latitude = 31.292736570443644, Longitude=-110.94944681504553 },
                new MapLatLng { Latitude = 31.29214433096709, Longitude=  -110.94933113906677 },
                new MapLatLng { Latitude = 31.292733306421663, Longitude=-110.94768725929922 },
                new MapLatLng{ Latitude=31.292360966046672, Longitude=-110.94639988349347},
                new MapLatLng{ Latitude=31.291761833444056, Longitude=-110.94554823493918},
                new MapLatLng{ Latitude=31.291460573238986, Longitude=-110.94563141921658}
            }
        },
        new RidesHistory
        {
            DateTime = DateTime.Now,
            GpsPoints = new ObservableCollection<MapLatLng>
            {
                new MapLatLng { Latitude = 31.292736570443644, Longitude=-110.94944681504553 },
                new MapLatLng { Latitude = 31.29214433096709, Longitude=  -110.94933113906677 },
                new MapLatLng { Latitude = 31.292733306421663, Longitude=-110.94768725929922 },
                new MapLatLng{ Latitude=31.292360966046672, Longitude=-110.94639988349347},
                new MapLatLng{ Latitude=31.291761833444056, Longitude=-110.94554823493918},
                new MapLatLng{ Latitude=31.291460573238986, Longitude=-110.94563141921658}
            }
        }
    };

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

    #endregion
}