namespace Infrastructure.Authentication;

public class KeycloakSettings
{
    public const string SectionName = "keycloak";


    public string Authority { get; init; } = string.Empty;
    public string Realm { get; init; } = string.Empty;
    public string AdminApiUrl { get; init; } = string.Empty;
    public string ClientId { get;  init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;


    public string AdminUserName { get;  init; } = string.Empty;
    public string AdminPassword { get; init; } = string.Empty;



    public string AppName { get;  init; } = string.Empty;

}
