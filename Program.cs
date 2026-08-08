using AppVidaSana.Data;
using AppVidaSana.Exceptions;
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
using AppVidaSana.RateLimitHelpers;
using AppVidaSana.TESTS;
using AppVidaSana.TESTS.ServicesTests;
using AppVidaSana.ProducesResponseType.ResponseOperationsFilters;
using AppVidaSana.Api;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var adminWeb = Environment.GetEnvironmentVariable("ADMIN_WEB");

var connectionString = Environment.GetEnvironmentVariable("DB_REMOTE");

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
        policy.WithOrigins(adminWeb!)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("Content-Disposition");
    });
});

builder.Logging.AddDebug();

builder.Services.AddRateLimiter(opt =>
{
    opt.AddPolicy("login", httpContext =>
        RateLimitHelpers.CreateFixedWindowLimiter(RateLimitHelpers.GetIpPartitionKey(httpContext), 10, TimeSpan.FromMinutes(5)));

    opt.AddPolicy("refresh", httpContext =>
        RateLimitHelpers.CreateFixedWindowLimiter(RateLimitHelpers.GetUserOrIpPartitionKey(httpContext), 5, TimeSpan.FromMinutes(1)));

    opt.AddPolicy("forgot-password", httpContext =>
        RateLimitHelpers.CreateFixedWindowLimiter(RateLimitHelpers.GetIpPartitionKey(httpContext), 3, TimeSpan.FromMinutes(15)));

    opt.AddPolicy("reset-password", httpContext =>
        RateLimitHelpers.CreateFixedWindowLimiter(RateLimitHelpers.GetIpPartitionKey(httpContext), 5, TimeSpan.FromMinutes(5)));

    opt.AddPolicy("read-only", httpContext =>
        RateLimitHelpers.CreateFixedWindowLimiter(RateLimitHelpers.GetUserOrIpPartitionKey(httpContext), 300, TimeSpan.FromMinutes(1)));

    opt.AddPolicy("write", httpContext =>
        RateLimitHelpers.CreateFixedWindowLimiter(RateLimitHelpers.GetUserOrIpPartitionKey(httpContext), 50, TimeSpan.FromMinutes(1)));

    opt.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        var errorResponse = new RequestGeneralExceptionMessage
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
                var errorResponse = new RequestGeneralExceptionMessage
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

builder.Services.AddAutoMapper(cfg => { }, typeof(Mapper).Assembly);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IServicesTests, ServicesTests>();

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
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("accessToken"))
            {
                context.Token = context.Request.Cookies["accessToken"];
            }
            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException) { throw new TokenExpiredException(); }

            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            var typ = context.Principal?.FindFirst("typ")?.Value;

            if (typ != "access")
                context.Fail("Invalid token type");

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

SwaggerConfiguration.AddSwagger(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/app/swagger.json", "Metabolique API");
    c.SwaggerEndpoint("/swagger/admin/swagger.json", "Metabolique Admin Web APIs");
    c.SwaggerEndpoint("/swagger/proxy/swagger.json", "Metabolique Proxys Admin Web APIs");
    c.SwaggerEndpoint("/swagger/app_test/swagger.json", "App TESTS");
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
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestTimeouts();
app.UseCors("RulesCORS");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
await app.RunAsync();
