using Microsoft.eShopOnContainers.WebMVC.Constants;

namespace Microsoft.eShopOnContainers.WebMVC;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the IoC container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews()
            .Services
            .AddAppInsight(Configuration)
            .AddHealthChecks(Configuration)
            .AddCustomMvc(Configuration)
            .AddDevspaces()
            .AddHttpClientServices(Configuration);

        IdentityModelEventSource.ShowPII = true;       // Caution! Do NOT use in production: https://aka.ms/IdentityModel/PII

        services.AddCustomAuthentication(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove(PropertyKeys.SubKey);
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(Patterns.Error);
        }

        var pathBase = Configuration[PropertyKeys.PathBaseKey];

        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        app.UseStaticFiles();
        app.UseSession();

        WebContextSeed.Seed(app, env);

        // Fix samesite issue when running eShop from docker-compose locally as by default http protocol is being used
        // Refer to https://github.com/dotnet-architecture/eShopOnContainers/issues/1391
        //SameSiteMode.Lax => SameSiteMode.None for launch locally
        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.None });

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(PropertyNames.Default, Patterns.CatalogIndex);
            endpoints.MapControllerRoute(PropertyNames.DefaultError, Patterns.ErrorError);
            endpoints.MapControllers();
            endpoints.MapHealthChecks(Patterns.Liveness, new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains(PropertyNames.Self)
            });
            endpoints.MapHealthChecks(Patterns.HC, new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }
}

static class ServiceCollectionExtensions
{

    public static IServiceCollection AddAppInsight(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetry(configuration);
        services.AddApplicationInsightsKubernetesEnricher();

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddUrlGroup(new Uri(configuration[PropertyKeys.IdentityUrlHCKey]), name: PropertyNames.IdentityApiCheck, tags: new string[] { PropertyNames.IdentityApi });

        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<AppSettings>(configuration);
        services.AddSession();
        services.AddDistributedMemoryCache();

        if (configuration.GetValue<string>(PropertyKeys.IsClusterEnvKey) == bool.TrueString)
        {
            services.AddDataProtection(opts =>
            {
                opts.ApplicationDiscriminator = PropertyNames.EshopWebMvc;
            })
            .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(configuration[PropertyKeys.DPConnectionStringKey]), PropertyKeys.DataProtectionKeysKey);
        }

        return services;
    }

    // Adds all Http client services
    public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        //register delegating handlers
        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
        services.AddTransient<HttpClientRequestIdDelegatingHandler>();

        //set 5 min as the lifetime for each HttpMessageHandler int the pool
        services.AddHttpClient(PropertyNames.ExtendedHandlerLifetime).SetHandlerLifetime(TimeSpan.FromMinutes(5)).AddDevspacesSupport();

        //add http client services
        services.AddHttpClient<IBasketService, BasketService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddDevspacesSupport();

        services.AddHttpClient<ICatalogService, CatalogService>()
                .AddDevspacesSupport();

        services.AddHttpClient<IOrderingService, OrderingService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddHttpMessageHandler<HttpClientRequestIdDelegatingHandler>()
                .AddDevspacesSupport();


        //add custom application services
        services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

        return services;
    }


    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var identityUrl = configuration.GetValue<string>(PropertyKeys.IdentityUrlKey);
        var callBackUrl = configuration.GetValue<string>(PropertyKeys.CallBackUrlKey);
        var sessionCookieLifetime = configuration.GetValue(PropertyKeys.SessionCookieLifetimeMinutesKey, 60);

        // Add Authentication services          

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
        .AddOpenIdConnect(options =>
        {
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Authority = identityUrl.ToString();
            options.SignedOutRedirectUri = callBackUrl.ToString();
            options.ClientId = Client.MVC;
            options.ClientSecret = Client.Secret;
            options.ResponseType = Client.CodeIdToken;
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.RequireHttpsMetadata = false;
            options.Scope.Add(Scopes.OpenId);
            options.Scope.Add(Scopes.Profile);
            options.Scope.Add(Scopes.Orders);
            options.Scope.Add(Scopes.Basket);
            options.Scope.Add(Scopes.WebShoppingAgg);
            options.Scope.Add(Scopes.OrdersSignalrHub);
        });

        return services;
    }
}
