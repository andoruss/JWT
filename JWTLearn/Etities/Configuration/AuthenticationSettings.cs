namespace Etities.Configuration;

//class qui va récupérer les infos dans le fichier appsettings
public class AuthenticationSettings
{
    public required string Secret { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required int ExpiryMinutes { get; set; }
}
