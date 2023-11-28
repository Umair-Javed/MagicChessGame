using Common.Library.ConfigModels;
using Common.Library.Interfaces;
using Common.Library.Services;
using WebFront.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Singleton services (One instance for the entire application)
builder.Services.AddSingleton<IServerStatusService, ServerStatusService>();

// Adding a hosted service for the PingBackgroundService to periodically ping the Logon Server and check its availability
builder.Services.AddHostedService<PingBackgroundService>();

// Scoped services (One instance per scope, e.g., per request)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IMongoDBService, MongoDBService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IMatchMakingServices, MatchMakingServices>();

// SignalR (Hub instances managed by SignalR)
builder.Services.AddSignalR();

// Transient services (A new instance every time it's requested)
// AddControllersWithViews internally registers controllers as transient
builder.Services.AddControllersWithViews();

// Registration of configuration sections
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<LogonServerSettings>(builder.Configuration.GetSection("LogonServerSettings"));
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Chess/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Map SignalR hub
    endpoints.MapHub<GameHub>("/gameHub");
    endpoints.MapFallbackToFile("index.html");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Lobby}/{action=Index}/{id?}");

app.Run();
