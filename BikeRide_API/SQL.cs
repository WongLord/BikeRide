using System.Data.SqlClient;

namespace BikeRide_API;

public class SQL
{
    private static readonly SqlConnection Conn = new(@"Data Source=sql5110.site4now.net;Initial Catalog=db_a96f30_bikeride;Persist Security Info=True;User ID=db_a96f30_bikeride_admin;Password=Lucario22;connection timeout=300000");
    
    public static object ExecuteScalar(string query)
    {
        Conn.Open();
        var scalar = new SqlCommand(query, Conn).ExecuteScalar();
        Conn.Close();

        return scalar;
    }

    public static void ExecuteQuery(string query)
    {
        Conn.Open();
        SqlCommand cmd = new(query, Conn);
        cmd.ExecuteNonQuery();
        Conn.Close();
    }
}
