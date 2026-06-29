using AI_genda_API.HealthChecks;
using AI_genda_API.Services.FocusSessionService;
using AI_genda_API.Services.ProfileService;
using AI_genda_API.Services.SubTaskService;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Threading.RateLimiting;


namespace AI_genda_API;

public static class Dependenciynjections
{

    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {

        // direct adding 
        services.AddOpenApi().AddControllers();
        services.AddScoped<IAuthService, AuthServic>();
        services.AddScoped<IWorkSpaceService, WorkSpaceService>();
        services.AddScoped<ISpaceService, SpaceService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ISubTaskService, SubTaskService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IFocusSessionService, FocusSessionService>();
        services.AddScoped<ITaskServiceNotification, TaskServiceNotification>();
        services.AddOptions<MailSettings>().BindConfiguration(nameof(MailSettings)).ValidateDataAnnotations().ValidateOnStart();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();


        // functions 
        services.AddCorsMethod(configuration);
        services.AddMapsterGlobalConfiguration(configuration);
        services.RegisterAppContext(configuration);
        services.RegisterIdentityServices();
        services.AddValidatorsFromAssemblyContaining<Program>().AddFluentValidationAutoValidation();
        services.AddAuthenticationServices(configuration);
        services.AddOptionclassBinding(configuration);
        services.AddJobs(configuration);
        services.AddRateLimiter(configuration);
        services.CheckTheHealth(configuration);

        return services;
    }

   

    private static void AddCorsMethod(this IServiceCollection services, IConfiguration config)
    {
        var AllowedOrigins = config.GetSection("AllowedOrigins").Get<string>();

        services.AddCors(
            opts =>
            {
                opts.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(AllowedOrigins!)
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            }

            );
    }

    private static void AddOptionclassBinding(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JWTOptions>()
            .BindConfiguration(JWTOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();
    }

    private static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJWTProvider, JWTProvider>();

        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        services.AddScoped<IAuthorizationHandler, WorkspacePermissionAuthorizationHandler>();

        var jwtSettings = configuration.GetSection(JWTOptions.SectionName).Get<JWTOptions>();

        services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }

        ).AddJwtBearer(options =>
        {
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,

                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SymmetricKey)),

                ValidIssuer = jwtSettings.Issuer,

                ValidAudience = jwtSettings.Audience,
            };
        });


        services.Configure<IdentityOptions>(Option =>
        {

            Option.SignIn.RequireConfirmedEmail = true;

            Option.User.RequireUniqueEmail = true;

            Option.Password.RequiredLength = 8;

        });
    }

    private static void AddMapsterGlobalConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(Assembly.GetAssembly(typeof(Program))!);

        services.AddSingleton<IMapper>(new Mapper(config));

    }

    private static void RegisterIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ExtendedUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppContext>().AddDefaultTokenProviders();
    }

    private static void RegisterAppContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connstring = configuration["ConnectionStrings:connectionstring"];
        services.AddDbContext<AppContext>(
            e => e.UseSqlServer(connstring).ConfigureWarnings(w =>
             w.Ignore(RelationalEventId.PendingModelChangesWarning)));
    }

    private static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {


        // Add Hangfire services.
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();

        // Add framework services.
        services.AddMvc();

        return services;
    }

    private static void AddRateLimiter(this IServiceCollection services, IConfiguration config)
    {
        services.AddRateLimiter(RateLimiterOptions =>
        {
            RateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            RateLimiterOptions.AddPolicy(
                config.GetValue<string>("RateLimitingTokens:PolicyName")!, context =>

                RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString(),
                partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = config.GetValue<int>("RateLimitingTokens:PerLimit"),
                    Window = TimeSpan.FromHours(config.GetValue<int>("RateLimitingTokens:PerLimitPeriod")),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst

                }));
        });

    }

    private static void CheckTheHealth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
        .AddSqlServer(connectionString: configuration.GetConnectionString("connectionstring")!, name: "MSQL-Check")
        .AddHangfire(opts => opts.MinimumAvailableServers = 1, name: "HangFire-Check")
        .AddCheck<EmailServiceHealthCheck>("EmailService-Check");
    }




}
