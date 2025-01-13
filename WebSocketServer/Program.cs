
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebSocketServer.Models;
namespace WebSocketServer;



public class Program
{
    private static List<Production> _productions = new()
    {
        new Production { Id = 1, Count = 10, Title = "Product A", ProductionTypeId = 1, CreateDate = DateTimeOffset.Now, Comment = "Initial stock" },
        new Production { Id = 2, Count = 20, Title = "Product B", ProductionTypeId = 2, CreateDate = DateTimeOffset.Now, Comment = "Initial stock" }
    };

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.UseWebSockets();

        app.Map("/ws", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await HandleConnection(webSocket);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        });

        await app.RunAsync("http://localhost:5000");
    }

    private static async Task HandleConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var requestJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var request = JsonSerializer.Deserialize<ProductionRequest>(requestJson);

                ProductionResponse response = request.RequestType switch
                {
                    "List" => HandleListRequest(),
                    "Add" => HandleAddRequest(request),
                    "Deduct" => HandleDeductRequest(request),
                    _ => new ProductionResponse { Status = false, Message = "Invalid request type" }
                };

                var responseJson = JsonSerializer.Serialize(response);
                var responseBytes = Encoding.UTF8.GetBytes(responseJson);
                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    private static ProductionResponse HandleListRequest()
    {
        return new ProductionResponse
        {
            Productions = _productions,
            Message = "Production list retrieved successfully",
            Status = true
        };
    }

    private static ProductionResponse HandleAddRequest(ProductionRequest request)
    {
        var production = _productions.FirstOrDefault(p => p.Id == request.ProductionId);

        if (production == null)
        {
            return new ProductionResponse
            {
                Productions = _productions,
                Message = "Production not found",
                Status = false
            };
        }

        production.Count += request.Count;
        return new ProductionResponse
        {
            Productions = _productions,
            Message = $"Added {request.Count} to production {production.Title}",
            Status = true
        };
    }

    private static ProductionResponse HandleDeductRequest(ProductionRequest request)
    {
        var production = _productions.FirstOrDefault(p => p.Id == request.ProductionId);

        if (production == null)
        {
            return new ProductionResponse
            {
                Productions = _productions,
                Message = "Production not found",
                Status = false
            };
        }

        if (production.Count < request.Count)
        {
            return new ProductionResponse
            {
                Productions = _productions,
                Message = "Insufficient quantity",
                Status = false
            };
        }

        production.Count -= request.Count;
        return new ProductionResponse
        {
            Productions = _productions,
            Message = $"Deducted {request.Count} from production {production.Title}",
            Status = true
        };
    }
}
