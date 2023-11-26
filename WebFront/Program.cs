using Common.Library.Interfaces;
using Common.Library.Services;
using Microsoft.AspNetCore.SignalR;
using WebFront.BackgroundServices;
using WebFront.ConfigModels;
using WebFront.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IMongoDBService, MongoDBService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IServerStatusService, ServerStatusService>();

// Registration of configuration sections
builder.Services.Configure<LogonServerSettings>(builder.Configuration.GetSection("LogonServerSettings"));
// Start the background service for periodic pinging
//builder.Services.AddHostedService<PingBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Chess/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GameHub>("/gameHub");
    endpoints.MapFallbackToFile("index.html");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Lobby}/{action=Index}/{id?}");

app.Run();

