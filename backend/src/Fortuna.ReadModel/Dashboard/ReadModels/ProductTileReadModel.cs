namespace Fortuna.ReadModel.Dashboard.ReadModels;

public sealed class ProductTileReadModel
{
    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }
    public string ProductCategory { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
}
