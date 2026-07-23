using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ToolCallingDemo.Tools;

public class OrderTools(IMongoDatabase database)
{
    private readonly IMongoCollection<Order> _orders = database.GetCollection<Order>("orders");

    [Description("Gets the status, total and delivery date of an order given its code.")]
    public async Task<OrderStatus> GetOrderStatusAsync(
        [Description("The order code, e.g. ORD-1024")] string orderCode,
        CancellationToken ct = default)
    {
        var order = await _orders.Find(o => o.Code == orderCode).FirstOrDefaultAsync(ct);
        if (order is null)
            return new OrderStatus(orderCode, "not_found", 0m, null);

        return new OrderStatus(order.Code, order.Status, order.Total, order.EstimatedDelivery);
    }
}

public record OrderStatus(string OrderCode, string Status, decimal Total, DateOnly? EstimatedDelivery);

[BsonIgnoreExtraElements]
public class Order
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string Code { get; set; } = "";
    public string Status { get; set; } = "";

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Total { get; set; }

    public DateOnly? EstimatedDelivery { get; set; }
}
