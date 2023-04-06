using BikeRide_API.Model;
using Microsoft.AspNetCore.Mvc;

namespace BikeRide_API.Controllers;

public class BikeRideController : ControllerBase
{
    [HttpGet("~/GetProfileInfo")]
    public IActionResult GetProfileInfo(int UserId)
    {
        return Ok(SQL.ExecuteScalar($"SP_GetUserInfo {UserId}").ToString()); //JsonConvert.DeserializeObject<List<RideActions>>
    }

    [HttpGet("~/GetRideActions")]
    public IActionResult GetRideActions(int UserId)
    {
        return Ok(GetJsonList($"SP_GetRideActions {UserId}")); //JsonConvert.DeserializeObject<List<RideActions>>
    }

    [HttpGet("~/GetRideLocPoints")]
    public IActionResult GetRideLocPoints(int UserId)
    {

        return Ok(GetJsonList($"SP_GetRidesLocationPoints {UserId}")); //JsonConvert.DeserializeObject<List<RideLocationPoints>>
    }

    private static string GetJsonList(string query)
    {
        var (conn, rdr) = SQL.GetReader(query);

        string result = "[";
        while (rdr.Read())
        {
            result += $"{rdr[0]},";
        }
        rdr.Close();
        result = result[..^1];
        result += "]";

        conn.Close();

        return result;
    }

    [HttpPost("~/PostNewRide")]
    public IActionResult PostNewRide([FromBody] Rides ride)
    {
        try
        {
            return Ok(SQL.ExecuteScalar($"SP_InNewRide {ride.UserId}, {ride.ActionId}"));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("~/EndRide")]
    public IActionResult EndRide([FromBody] Rides ride)
    {
        try
        {
            SQL.ExecuteQuery($"SP_UpStopRide {ride.RideId}, '{ride.EndDateTime}'");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("~/PostNewRideAction")]
    public IActionResult PostNewRideAction([FromBody] RideActions action)
    {
        try
        {
            SQL.ExecuteQuery($"SP_InRideAction {action.UserId}, {action.ActionId}, {action.Latitude}, {action.Longitude}, {action.Speed}");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("~/PostLocPoint")]
    public IActionResult PostLocPoint([FromBody] RideLocationPoints Rlp)
    {
        try
        {
            foreach (var point in Rlp.RideCoordinates)
                SQL.ExecuteQuery($"SP_InLocationPoint {Rlp.RideId}, {point.Latitude}, {point.Longitude}");

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
