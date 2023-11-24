using System.Net.Sockets;
using System.Net;
using System.Text;

TcpListener tcpListener = new TcpListener(IPAddress.Any, 5258);
tcpListener.Start();

Console.WriteLine("Server is listening...");

while (true)
{
    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
    HandleClient(tcpClient);
}


static async void HandleClient(TcpClient tcpClient)
{
    using (NetworkStream stream = tcpClient.GetStream())
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received: {message}");

            byte[] responseBuffer = Encoding.ASCII.GetBytes("Message received successfully");
            await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
        }
    }

    tcpClient.Close();
}