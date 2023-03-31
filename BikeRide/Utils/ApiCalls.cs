namespace BikeRide.Utils;

public class ApiCalls
{
    static string ApiBaseUrl = "https://delcamposupreme.com:144/br/";
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

    public static StreamReader PUTRequest(string Url, object obj)
    {
        var httpRequest = (HttpWebRequest)WebRequest.Create(ApiBaseUrl + Url);
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
}
