using System.Data.SqlClient;

namespace BikeRide_API;

public class SQL
{
    private static SqlConnection Conn = new(@"Data Source=sql5110.site4now.net;Initial Catalog=db_a96f30_bikeride;Persist Security Info=True;User ID=db_a96f30_bikeride_admin;Password=Lucario22;connection timeout=300000");
    public static SqlDataAdapter GetAdapter(string query)
    {
        Conn.Open();
        SqlDataAdapter adapter = new()
        {
            SelectCommand = new SqlCommand(query, Conn)
        };

        return adapter;
    }

    public static object ExecuteScalar(string query)
    {
        Conn.Open();
        var scalar = new SqlCommand(query, Conn).ExecuteScalar();
        Conn.Close();

        return scalar;
    }

    public static (SqlConnection Conn, SqlDataReader Reader) GetReader(string query)
    {
        Conn.Open(); //Needs to close Reader and Connection after use.
        return (Conn, new SqlCommand(query, Conn).ExecuteReader());
    }

    public static bool ReaderReads(string query)
    {
        Conn.Open();
        SqlDataReader rdr = new SqlCommand(query, Conn).ExecuteReader();
        bool isReading = rdr.Read();
        rdr.Close();
        Conn.Close();

        return isReading;
    }

    public static void ExecuteQuery(string query)
    {
        Conn.Open();
        SqlCommand cmd = new(query, Conn);
        cmd.ExecuteNonQuery();
        Conn.Close();
    }
}
