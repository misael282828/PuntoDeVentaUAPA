namespace PuntoVentaPOS.Models;

public static class UserSession
{
    public static Usuario? CurrentUser { get; private set; }

    public static void Start(Usuario user)
    {
        CurrentUser = user;
    }

    public static void End()
    {
        CurrentUser = null;
    }
}
