using System.Net.Sockets;
using System.Text;
using WebFront.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<IMongoDBService, MongoDBService>();
builder.Services.AddSingleton<IServerStatusService, ServerStatusService>();
// Start the background service for periodic pinging
builder.Services.AddHostedService<PingBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Background service to ping the server periodically
public class PingBackgroundService : BackgroundService
{
    private readonly IServerStatusService _serverStatusService;

    public PingBackgroundService(IServerStatusService serverStatusService)
    {
        _serverStatusService = serverStatusService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string serverIpAddress = "127.0.0.1";
        const int serverPort = 5258;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient(serverIpAddress, serverPort))
                {
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        string message = "Ping from client!";
                        byte[] buffer = Encoding.ASCII.GetBytes(message);

                        await stream.WriteAsync(buffer, 0, buffer.Length);

                        buffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Server response: {response}");

                        // Set the server status to running
                        _serverStatusService.SetServerStatus(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to the server: {ex.Message}");

                // Set the server status to not running
                _serverStatusService.SetServerStatus(false);

                // Log the error
                _serverStatusService.LogError(ex.Message);
            }

            // Adjust the interval as needed
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}