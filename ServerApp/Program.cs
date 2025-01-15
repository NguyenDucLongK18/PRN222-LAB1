using System.Net;
using System.Net.Sockets;

class Program
{
    static void ProcessMessage(object param)
    {
        string data;
        int count;
        try
        {
            TcpClient client = param as TcpClient;
            IPEndPoint remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Client connected: {remoteEndPoint.Address}:{remoteEndPoint.Port}");
            Byte[] bytes = new Byte[256];

            NetworkStream stream = client.GetStream();

            while ((count = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, count);
                Console.WriteLine($"Received: {data} at {DateTime.Now:t}");
                data = $"{data.ToUpper()}";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                stream.Write(msg, 0, msg.Length);
                Console.WriteLine($"Sent: {data}");
            }

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("{0}", ex.Message);
            Console.WriteLine("Wating message");
        }
    }

    static void ExecuteServer(string host, int port)
    {
        int Count = 0;
        TcpListener server = null;
        try
        {
            Console.Title = "Server Application";
            IPAddress[] localAddrs = Dns.GetHostAddresses(host);
            server = new TcpListener(localAddrs[0], port);

            server.Start();
            //connection details
            Console.WriteLine($"Current IP Address: {host}");
            Console.WriteLine($"Current Port: {port}");
            Console.WriteLine(new string('*', 40));
            Console.WriteLine("Waiting for a connection... ");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine($"Number of client connected: {++Count}");
                Console.WriteLine(new string('*', 40));

                Thread thread = new Thread(new ParameterizedThreadStart(ProcessMessage));
                thread.Start(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: {0}", ex.Message);
            Console.WriteLine("Waiting for message...");
        }
        finally
        {
            server.Stop();
            Console.WriteLine("Server stopped. Press any key to exit...");
        }
        Console.Read();
    }
    static void Main(String[] args)
    {
        string host = "localhost";
        int port = 8080;
        ExecuteServer(host, port);
    }
}