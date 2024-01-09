namespace Domain.Entities;

public class TodoList
{
    public string? Title { get; set; }

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
