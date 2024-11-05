using AppVidaSana.Api;
using AppVidaSana.Api.Key;
using AppVidaSana.Data;
using AppVidaSana.JsonFormat;
using AppVidaSana.Mappers;
using AppVidaSana.Services;
using AppVidaSana.Services.Habits;
using AppVidaSana.Services.IServices;
using AppVidaSana.Services.IServices.IHabits;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using AppVidaSana.Services.Monthly_Follows_Ups;
using AppVidaSana.Services.Seguimientos_Mensuales;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DB_REMOTE");

var token = Environment.GetEnvironmentVariable("TOKEN") ?? Environment.GetEnvironmentVariable("TOKEN_Replacement");
var key = Encoding.ASCII.GetBytes(token);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

var myrulesCORS = "RulesCORS";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: myrulesCORS, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

        /*
         builder.WithOrigins("https://dominio1.com", "https://dominio2.com")
               .AllowAnyMethod()
               .AllowAnyHeader();*/
    });
});

builder.Services.AddRequestTimeouts(options =>
{
    options.DefaultPolicy =
        new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromMilliseconds(1000),
            TimeoutStatusCode = StatusCodes.Status503ServiceUnavailable
        };
    options.AddPolicy("CustomPolicy",
        new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromMilliseconds(30000),
            TimeoutStatusCode = StatusCodes.Status503ServiceUnavailable
        });
});


builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new TimeOnlyJsonConverter());
});


builder.Services.AddScoped<IAccount, AccountService>();
builder.Services.AddScoped<IProfile, ProfileService>();
builder.Services.AddScoped<IAuthentication_Authorization, Authentication_AuthorizationService>();
builder.Services.AddScoped<IResetPassword, ResetPassswordService>();
builder.Services.AddScoped<IMFUsFood, MFUsFoodService>();
builder.Services.AddScoped<IExercise, ExerciseService>();
builder.Services.AddScoped<IMFUsExercise, MFUsExerciseService>();
builder.Services.AddScoped<IHabitsGeneral, HabitGeneralService>();
builder.Services.AddScoped<IDrinkHabit, DrinkHabitService>();
builder.Services.AddScoped<IDrugsHabit, DrugsHabitService>();
builder.Services.AddScoped<ISleepHabit, SleepHabitService>();
builder.Services.AddScoped<IMFUsHabits, MFUsHabitsService>();
builder.Services.AddScoped<IMedication, MedicationService>();
builder.Services.AddScoped<ISideEffects, MedicationService>();
builder.Services.AddScoped<IMFUsMedications, MFUsMedicationService>();

builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(Mapper));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseInMemoryDatabase(nameof(ApiDbContext)));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "metaboliqueapi",
        ValidAudience = "metabolique.com",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthentication(ApiKeySchemeOptions.Scheme)
    .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(
        ApiKeySchemeOptions.Scheme, options =>
        {
            options.HeaderName = "Metabolique_API_KEY";
        });


builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Metabolique_API",
        Version = "v1",
        Description = "An ASP.NET Core web API to manage medical tracking elements of a user's medical record."
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

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeveloperTest V1");
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(myrulesCORS);
app.UseRequestTimeouts();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

await Seed();

await app.RunAsync();

async Task Seed()
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetService<ApiDbContext>();

    var apiKeyEnv = Environment.GetEnvironmentVariable("API_KEY");

    if (string.IsNullOrEmpty(apiKeyEnv))
    {
        throw new InvalidOperationException("La variable de entorno 'API_KEY' no está configurada.");
    }

    if (!await context.ApiKeys.AnyAsync())
    {
        context.ApiKeys.Add(new ApiKey
        {
            Key = Guid.Parse(apiKeyEnv),
            Name = "Metabolique"
        });

        await context.SaveChangesAsync();
    }
}

