namespace FamilySyncApi.Models;

public class TodoItem
{
    public string Task { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
