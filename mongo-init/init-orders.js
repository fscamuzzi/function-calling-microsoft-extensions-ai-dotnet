// Runs once on the first `docker compose up` (empty volume): seeds the demo
// order used throughout the tutorial. Field names match the C# Order model.
db = db.getSiblingDB("shopdemo");

db.orders.insertOne({
  Code: "ORD-1024",
  Status: "shipped",
  Total: NumberDecimal("149.90"),
  EstimatedDelivery: ISODate("2026-07-30T00:00:00Z"),
});
