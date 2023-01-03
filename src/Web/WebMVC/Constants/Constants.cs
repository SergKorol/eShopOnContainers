namespace Microsoft.eShopOnContainers.WebMVC.Constants;

public static class Client
{
    public const string MVC = "mvc";
    public const string Secret = "secret";
    public const string CodeIdToken = "code id_token";
}

public static class Patterns
{
    public const string Liveness = "/liveness";
    public const string HC = "/hc";
    public const string CatalogIndex = "{controller=Catalog}/{action=Index}/{id?}";
    public const string ErrorError = "{controller=Error}/{action=Error}";
    public const string Error = "/Error";
}

public static class PropertyKeys
{
    public const string IdentityUrlKey = "IdentityUrl";
    public const string CallBackUrlKey = "CallBackUrl";
    public const string SessionCookieLifetimeMinutesKey = "SessionCookieLifetimeMinutes";
    public const string DPConnectionStringKey = "DPConnectionString";
    public const string DataProtectionKeysKey = "DataProtection-Keys";
    public const string IsClusterEnvKey = "IsClusterEnv";
    public const string IdentityUrlHCKey = "IdentityUrlHC";
    public const string PathBaseKey = "PATH_BASE";
    public const string SubKey = "sub";
}

public static class PropertyNames
{
    public const string ExtendedHandlerLifetime = "extendedhandlerlifetime";
    public const string EshopWebMvc = "eshop.webmvc";
    public const string IdentityApiCheck = "identityapi-check";
    public const string IdentityApi = "identityapi";
    public const string Default = "default";
    public const string DefaultError = "defaultError";
    public const string Self = "self";
    
}

public static class Scopes
{
    public const string OpenId = "openid";
    public const string Profile = "profile";
    public const string Orders = "orders";
    public const string Basket = "basket";
    public const string WebShoppingAgg = "webshoppingagg";
    public const string OrdersSignalrHub = "orders.signalrhub";
}