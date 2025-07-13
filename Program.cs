using AppVidaSana.Data;
using AppVidaSana.JsonFormat;
using AppVidaSana.Mappers;
using AppVidaSana.Services;
using AppVidaSana.Services.Habits;
using AppVidaSana.Services.IServices;
using AppVidaSana.Services.IServices.IHabits;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AppVidaSana.Services.Monthly_Follows_Ups;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Azure.Storage.Blobs;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType;
using Newtonsoft.Json;
using AppVidaSana.Services.IServices.IAdminWeb;
using AppVidaSana.Services.AdminWeb;
using System.Security.Claims;
using AppVidaSana.KeyToken;
using System.Threading.RateLimiting;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var adminWeb = Environment.GetEnvironmentVariable("ADMIN_WEB_TEST");

var connectionString = Environment.GetEnvironmentVariable("DB_LOCAL");

var storageAccount = Environment.GetEnvironmentVariable("STORAGE");

var token = Environment.GetEnvironmentVariable("TOKEN") ?? Environment.GetEnvironmentVariable("TOKEN_Replacement");
var keyBytes = Encoding.ASCII.GetBytes(token!);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

builder.Services.AddSingleton(x => new BlobServiceClient(storageAccount));

builder.Services.AddCors(options =>
{
    options.AddPolicy("RulesCORS", policy =>
    {
        policy.WithOrigins(adminWeb)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("Content-Disposition");
    });
});

builder.Logging.AddDebug();

builder.Services.AddRateLimiter(opt =>
{
    opt.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, 
                Window = TimeSpan.FromMinutes(5) 
            }));

    opt.AddPolicy("refresh", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.IsAuthenticated == true
                          ? httpContext.User.Identity.Name
                          : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));

    opt.AddPolicy("forgot-password", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(15) 
            }));

    opt.AddPolicy("reset-password", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(5)
            }));

    opt.AddPolicy("read-only", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.IsAuthenticated == true
                          ? httpContext.User.Identity.Name
                          : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 300,
                Window = TimeSpan.FromMinutes(1)
            }));

    opt.AddPolicy("write", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.IsAuthenticated == true
                          ? httpContext.User.Identity.Name
                          : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1)
            }));

    opt.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        var errorResponse = new RequestTimeoutExceptionMessage
        {
            status = StatusCodes.Status429TooManyRequests,
            error = "Too Many Requests",
            message = "La petici&oacute;n ha tenido muchos intentos, int&eacute;ntelo de nuevo",
            timestamp = DateTime.UtcNow.ToString("o"),
            path = context.HttpContext.Request.Path
        };

        var jsonResponse = JsonConvert.SerializeObject(errorResponse);
        await context.HttpContext.Response.WriteAsync(jsonResponse, token);
    };
});

builder.Services.AddRequestTimeouts(options =>
{
    options.AddPolicy("CustomPolicy",
        new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromSeconds(45),
            TimeoutStatusCode = 503,
            WriteTimeoutResponse = async (HttpContext context) => {
                context.Response.ContentType = "application/json";
                var errorResponse = new RequestTimeoutExceptionMessage
                {
                    status = StatusCodes.Status503ServiceUnavailable,
                    error = "Service Unavailable",
                    message = "La petici&oacute;n ha tardado m&aacute;s de lo esperado, int&eacute;ntelo de nuevo.",
                    timestamp = DateTime.UtcNow.ToString("o"),
                    path = context.Request.Path
                };
                var jsonResponse = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(jsonResponse);
            }
        });
});

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new DateOnlyJsonConverter());
    options.SerializerSettings.Converters.Add(new TimeOnlyJsonConverter());
});

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(typeof(Mapper));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAWAuth, AWAuthService>();
builder.Services.AddScoped<IAWDoctors, AWDoctorService>();
builder.Services.AddScoped<IAWFeeding, AWFeedingService>();
builder.Services.AddScoped<IAWExercise, AWExerciseService>();
builder.Services.AddScoped<IAWHabits, AWHabitService>();
builder.Services.AddScoped<IAWMedication, AWMedicationService>();
builder.Services.AddScoped<IAWPatients, AWPatientsService>();
builder.Services.AddScoped<IExportToZip, AWExportToZipService>();

builder.Services.AddScoped<IUserDaySummary, UserDaySummaryService>();
builder.Services.AddScoped<ICalories, CaloriesService>();
builder.Services.AddScoped<IAccount, AccountService>();
builder.Services.AddScoped<IProfile, ProfileService>();
builder.Services.AddScoped<IDoctor, DoctorService>();
builder.Services.AddScoped<IAuthenticationAuthorization, AuthenticationAuthorizationService>();
builder.Services.AddScoped<IResetPassword, ResetPassswordService>();
builder.Services.AddScoped<IFeeding, FeedingService>();
builder.Services.AddScoped<IMFUsFood, MFUsFoodService>();
builder.Services.AddScoped<IExercise, ExerciseService>();
builder.Services.AddScoped<IMFUsExercise, MFUsExerciseService>();
builder.Services.AddScoped<IHabitsGeneral, HabitGeneralService>();
builder.Services.AddScoped<IDrinkHabit, DrinkHabitService>();
builder.Services.AddScoped<IDrugsHabit, DrugsHabitService>();
builder.Services.AddScoped<ISleepHabit, SleepHabitService>();
builder.Services.AddScoped<IMFUsHabits, MFUsHabitsService>();
builder.Services.AddScoped<IMedication, MedicationService>();
builder.Services.AddScoped<ISideEffects, SideEffectsService>();
builder.Services.AddScoped<IMFUsMedications, MFUsMedicationService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = KeyTokenEnv.GetTokenIssuerEnv(),
        ValidateAudience = true,
        ValidAudience = KeyTokenEnv.GetTokenAudienceEnv(),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException) { throw new TokenExpiredException(); }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("app", new OpenApiInfo
    {
        Title = "Metabolique API",
        Version = "v1",
        Description = "An ASP.NET Core web API to manage medical tracking elements of a user's medical record."
    });

    c.SwaggerDoc("admin", new OpenApiInfo 
    { 
        Title = "Metabolique Admin Web APIs", 
        Version = "v1",
        Description = "An ASP.NET Core web API for Metabolique web administrator."
    });

    c.SwaggerDoc("proxy", new OpenApiInfo 
    { 
        Title = "Metabolique Proxys Admin Web APIs", 
        Version = "v1",
        Description = "Proxies for Metabolique web administrator."
    });

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (string.IsNullOrEmpty(apiDesc.GroupName)) { return false; }

        return string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase);
    });

    c.TagActionsBy(api =>
    {
        try
        {
            var tags = api.ActionDescriptor.EndpointMetadata
                .OfType<TagsAttribute>()
                .SelectMany(t => t.Tags)
                .ToList();

            if (tags.Count > 0) { return tags; }

            return new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] };
        }
        catch
        {
            return new[] { api.ActionDescriptor.RouteValues["controller"] };
        }
    });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    c.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Example = new OpenApiString(DateTime.Today.ToString("yyyy-MM-dd"))
    });
    c.MapType<TimeOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "time",
        Example = new OpenApiString(DateTime.Today.ToString("HH:mm"))
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/app/swagger.json", "Metabolique API");
    c.SwaggerEndpoint("/swagger/admin/swagger.json", "Metabolique Admin Web APIs");
    c.SwaggerEndpoint("/swagger/proxy/swagger.json", "Metabolique Proxys Admin Web APIs");
});

app.Use(async (context, next) =>
{
    try
    {
        await next(); 
    }
    catch (Exception ex) when (ex is TokenExpiredException || ex is ApiKeyException)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var errorResponse = new ExceptionExpiredTokenMessage
        {
            status = StatusCodes.Status401Unauthorized,
            error = "Unauthorized",
            message = ex.Message,
            timestamp = DateTime.UtcNow.ToString("o"),
            path = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
});

app.UseHttpsRedirection();         
app.UseStaticFiles();              
app.UseRouting();
app.UseCors("RulesCORS");
app.UseRequestTimeouts();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
await app.RunAsync();   
