using LogonServer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.WebSockets;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(120);
    options.ReceiveBufferSize = 4 * 1024;
});

var app = builder.Build();

app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleSocketForMatchMaking(context, webSocket);
    }
    else
    {
        await next();
    }
});

app.Map("/websocket", HandleSocketForMatchMaking);

app.Run();


// websocket we only use for match making
async Task HandleSocketForMatchMaking(HttpContext context, WebSocket webSocket)
{
    var buffer = new byte[4 * 1024];
    WebSocketReceiveResult result;

    do
    {
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Text)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received message: {message}");

            // Your logic to handle the received message

            var responseMessage = Encoding.UTF8.GetBytes("Server received your message");
            await webSocket.SendAsync(new ArraySegment<byte>(responseMessage, 0, responseMessage.Length),
                                      WebSocketMessageType.Text,
                                      true,
                                      CancellationToken.None);
        }

    } while (!result.CloseStatus.HasValue);

    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
}