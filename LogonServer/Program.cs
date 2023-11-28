using System.Net.Sockets;
using System.Net;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        // Setup TCP listener on any available IP address and port 5258
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 5258);
        tcpListener.Start();

        Console.WriteLine("Server is listening...");

        // Accept incoming connections and handle clients in an infinite loop
        while (true)
        {
            TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
            HandleClient(tcpClient);
        }
    }

    #region Handle Client

    // Handle communication with a connected client
    static async void HandleClient(TcpClient tcpClient)
    {
        // Get the network stream for reading and writing data
        using (NetworkStream stream = tcpClient.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            // Read data from the client in a loop
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                // Convert the received bytes to a string message
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                // Send a response back to the client
                byte[] responseBuffer = Encoding.ASCII.GetBytes("Message received successfully");
                await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
            }
        }

        // Close the TCP client connection
        tcpClient.Close();
    }

    #endregion
}
