using MySqlConnector;

namespace PuntoVentaPOS.Data;

public static class Db
{
    public static MySqlConnection CreateConnection()
    {
        return new MySqlConnection(AppConfig.ConnectionString);
    }
}
