namespace WebSocketServer.Models;

public class Production
{
    public int Id { get; set; }
    public int Count { get; set; }
    public string Title { get; set; }
    public int ProductionTypeId { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public string Comment { get; set; }
}

public class ProductionRequest
{
    public string RequestType { get; set; } // "List", "Add", "Deduct"
    public int ProductionId { get; set; }
    public int Count { get; set; }
}

public class ProductionResponse
{
    public List<Production> Productions { get; set; }
    public string Message { get; set; }
    public bool Status { get; set; }
}
