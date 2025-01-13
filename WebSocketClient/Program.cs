using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebSocketClient.Models;
namespace WebSocketClient;



public class Program
{
    private static readonly ClientWebSocket _webSocket = new();
    private static readonly string _serverUrl = "ws://localhost:5000/ws";

    public static async Task Main(string[] args)
    {
        await ConnectToServer();

        while (true)
        {
            Console.WriteLine("\nChoose an action:");
            Console.WriteLine("1. List Productions");
            Console.WriteLine("2. Add Count");
            Console.WriteLine("3. Deduct Count");
            Console.WriteLine("4. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await SendRequest(new ProductionRequest { RequestType = "List" });
                    break;

                case "2":
                    Console.Write("Enter Production ID: ");
                    var addId = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter count to add: ");
                    var addCount = int.Parse(Console.ReadLine() ?? "0");
                    await SendRequest(new ProductionRequest
                    {
                        RequestType = "Add",
                        ProductionId = addId,
                        Count = addCount
                    });
                    break;

                case "3":
                    Console.Write("Enter Production ID: ");
                    var deductId = int.Parse(Console.ReadLine() ?? "0");
                    Console.Write("Enter count to deduct: ");
                    var deductCount = int.Parse(Console.ReadLine() ?? "0");
                    await SendRequest(new ProductionRequest
                    {
                        RequestType = "Deduct",
                        ProductionId = deductId,
                        Count = deductCount
                    });
                    break;

                case "4":
                    return;

                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }

    private static async Task ConnectToServer()
    {
        try
        {
            await _webSocket.ConnectAsync(new Uri(_serverUrl), CancellationToken.None);
            Console.WriteLine("Connected to server");
            _ = ReceiveMessages();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to server: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static async Task ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var responseJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var response = JsonSerializer.Deserialize<ProductionResponse>(responseJson);

                    Console.WriteLine($"\nResponse Status: {response.Status}");
                    Console.WriteLine($"Message: {response.Message}");
                    Console.WriteLine("\nProductions:");
                    foreach (var production in response.Productions)
                    {
                        Console.WriteLine($"ID: {production.Id}, Title: {production.Title}, Count: {production.Count}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving messages: {ex.Message}");
        }
    }

    private static async Task SendRequest(ProductionRequest request)
    {
        var requestJson = JsonSerializer.Serialize(request);
        var bytes = Encoding.UTF8.GetBytes(requestJson);
        await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}