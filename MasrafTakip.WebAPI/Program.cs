using MasrafTakip.Application.Interfaces;
using MasrafTakip.Domain.Interfaces;
using MasrafTakip.Infrastructure.Data;
using MasrafTakip.Infrastructure.Identity;
using MasrafTakip.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Hangfire;
using MasrafTakip.Infrastructure.BackgroundJobs;
using MasrafTakip.Application.Services;
using Microsoft.EntityFrameworkCore;
using Hangfire.MySql;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Logger = new LoggerConfiguration()
    .WriteTo.MySQL(connectionString, "ErrorLogs")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

// Add repositories
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Add controllers
builder.Services.AddControllers();

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();


// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Add Hangfire
var hangfireOptions = new MySqlStorageOptions
{
    TransactionIsolationLevel = (System.Transactions.IsolationLevel?)System.Data.IsolationLevel.ReadCommitted,
    QueuePollInterval = TimeSpan.FromSeconds(15),
    JobExpirationCheckInterval = TimeSpan.FromHours(1),
    CountersAggregateInterval = TimeSpan.FromMinutes(5),
    PrepareSchemaIfNecessary = true,
    DashboardJobListLimit = 50000,
    TransactionTimeout = TimeSpan.FromMinutes(1),
    TablesPrefix = "Hangfire"
};

builder.Services.AddHangfire(config =>
    config.UseStorage(new MySqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"), hangfireOptions)));


builder.Services.AddHangfireServer();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHangfireDashboard();

// Schedule recurring jobs
RecurringJob.AddOrUpdate<TransactionAggregationJob>(
    "daily-transaction-aggregation-job",
    job => job.AggregateDailyTransactions(),
    Cron.Daily);

RecurringJob.AddOrUpdate<TransactionAggregationJob>(
    "weekly-transaction-aggregation-job",
    job => job.AggregateWeeklyTransactions(),
    Cron.Weekly);

RecurringJob.AddOrUpdate<TransactionAggregationJob>(
    "monthly-transaction-aggregation-job",
    job => job.AggregateMonthlyTransactions(),
    Cron.Monthly);


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseMiddleware<MasrafTakip.Infrastructure.Logging.ErrorLoggingMiddleware>();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});

app.Run();
