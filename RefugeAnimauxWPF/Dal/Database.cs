using Npgsql;

namespace RefugeAnimaux.Dal;

public static class Database
{
   
    public static string ConnectionString =
        "Host=localhost;Port=5432;Database=refuge_animaux;Username=postgres;Password=Samy2001";

    public static NpgsqlConnection GetConnection()
    {
        var cnx = new NpgsqlConnection(ConnectionString);
        cnx.Open();
        return cnx;
    }
}
