using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TasteBox.Settings;

namespace TasteBox;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRouting(options => { options.LowercaseUrls = true; });
        services.AddControllers();

        services
            .AddInfrastructureServices(configuration)
            .AddAuthConfig(configuration)
            .AddSwaggerServices()
            .AddMapsterConfig()
            .AddFluentValidationConfig();

        services.AddHttpContextAccessor();

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        // Configure cache provider based on settings
        services.AddCacheServices(configuration);

        services.AddScoped<IStockService, StockService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IEmailSender, EmailService>();


        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });


        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience
                };
            });


        return services;
    }

    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            options.SwaggerDoc("dashboard",
                new OpenApiInfo
                {
                    Title = "TasteBox-Dashboard-API", Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "TasteBox-Dashboard-API", Url = new Uri("https://github.com/ziadhanii/TasteBox"),
                        Email = "ziadhani64@gmail.com"
                    }
                });

            options.SwaggerDoc("mobile",
                new OpenApiInfo
                {
                    Title = "TasteBox Mobile API", Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "TasteBox-Mobile-API", Url = new Uri("https://github.com/ziadhanii/TasteBox"),
                        Email = "ziadhani64@gmail.com"
                    }
                });

            options.AddSecurityDefinition("bearer",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                { [new OpenApiSecuritySchemeReference("bearer", document)] = [] });
        });
        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }

    private static IServiceCollection AddCacheServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Using in-memory cache by default
        // To use Redis, install Microsoft.Extensions.Caching.StackExchangeRedis package and configure:
        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = "your-redis-connection-string";
        //     options.InstanceName = "TasteBox:";
        // });
        services.AddDistributedMemoryCache();

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();

        return services;
    }
}