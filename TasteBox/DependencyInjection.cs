using System.Text.Json;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace TasteBox;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDependencies(IConfiguration configuration)
        {
            services.AddRouting(options => { options.LowercaseUrls = true; });
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters
                        .Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            services
                .AddInfrastructureServices(configuration)
                .AddAuthConfig(configuration)
                .AddSwaggerServices()
                .AddMapsterConfig()
                .AddFluentValidationConfig();

            services.AddHttpContextAccessor();

            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.Configure<GoogleAuthSettings>(configuration.GetSection(GoogleAuthSettings.SectionName));

            services.Configure<OrderSettings>(configuration.GetSection(nameof(OrderSettings)));

            services.AddCacheServices(configuration);

            services.AddScoped<EmailBodyBuilder>();

            services.AddScoped<IStockService, StockService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IUnitConverter, UnitConverter>();
            services.AddScoped<IUserFavoritesService, UserFavoritesService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationSender, NotificationSender>();

            services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }

        private IServiceCollection AddInfrastructureServices(IConfiguration configuration)
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

        private IServiceCollection AddAuthConfig(IConfiguration configuration)
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


                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/hubs/order"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };


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

        private IServiceCollection AddSwaggerServices()
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
                        Title = "TasteBox-Dashboard-API",
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "TasteBox-Dashboard-API",
                            Url = new Uri("https://github.com/ziadhanii/TasteBox"),
                            Email = "ziadhani64@gmail.com"
                        }
                    });

                options.SwaggerDoc("mobile",
                    new OpenApiInfo
                    {
                        Title = "TasteBox Mobile API",
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "TasteBox-Mobile-API",
                            Url = new Uri("https://github.com/ziadhanii/TasteBox"),
                            Email = "ziadhani64@gmail.com"
                        }
                    });

                options.AddSecurityDefinition("bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Authorization header using the Bearer scheme."
                    });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                    { [new OpenApiSecuritySchemeReference("bearer", document)] = [] });
            });
            return services;
        }

        private IServiceCollection AddFluentValidationConfig()
        {
            services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }

        private IServiceCollection AddMapsterConfig()
        {
            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            mappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(mappingConfig));

            return services;
        }

        private IServiceCollection AddCacheServices(IConfiguration configuration)
        {
            services.AddScoped<ICacheService, CacheService>();

            services.AddOptions<CacheSettings>()
                .BindConfiguration(CacheSettings.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var settings = configuration
                               .GetSection(CacheSettings.SectionName)
                               .Get<CacheSettings>()
                           ?? throw new InvalidOperationException("Cache settings not configured");

            var configurationOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false
            };

            configurationOptions.EndPoints.Add(settings.Host, settings.Port);

            if (!string.IsNullOrWhiteSpace(settings.User))
                configurationOptions.User = settings.User;

            if (!string.IsNullOrWhiteSpace(settings.Password))
                configurationOptions.Password = settings.Password;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(configurationOptions));

            services.AddStackExchangeRedisCache(options => { options.ConfigurationOptions = configurationOptions; });

            return services;
        }
    }
}