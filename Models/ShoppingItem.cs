namespace FamilySyncApi.Models;

public class ShoppingItem
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsCompleted { get; set; }
}