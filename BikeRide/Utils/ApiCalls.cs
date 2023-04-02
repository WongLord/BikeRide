namespace BikeRide.Utils;

public class ApiCalls
{
    static string ApiBaseUrl = "https://delcamposupreme.com/br/";
    public static async Task<HttpResponseMessage> GETResponse(string Url, string Parameters)
    {
        return await new HttpClient
        {
            BaseAddress = new Uri(ApiBaseUrl+Url),
            DefaultRequestHeaders =
            {
                { "accept", "*/*" },
            }
        }.GetAsync(Parameters);
    }

    public static StreamReader POSTRequest(string method, object obj)
    {
        var httpRequest = (HttpWebRequest)WebRequest.Create(ApiBaseUrl + method);
        httpRequest.Method = "POST";

        httpRequest.Headers["accept"] = "text/plain";
        httpRequest.ContentType = "application/json-patch+json";

        var data = JsonConvert.SerializeObject(obj);

        using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
        {
            streamWriter.Write(data);
        }


        var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //{
        //    var result = streamReader.ReadToEnd();

        //    return Convert.ToInt32(result == "" ? "0" : result.Replace("\"", ""));
        //}
        return new StreamReader(httpResponse.GetResponseStream());
    }

    public static async Task<HttpResponseMessage> GETRoad(float Lat, float Lon)
    {
        return await new HttpClient
        {
            BaseAddress = new Uri($"https://nominatim.openstreetmap.org/reverse?format=json&lat={Lat}&lon={Lon}"),
            DefaultRequestHeaders =
            {
                { "accept", "*/*" },
            }
        }.GetAsync("");
    }
}
