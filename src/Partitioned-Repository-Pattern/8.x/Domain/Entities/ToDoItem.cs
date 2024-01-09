using Domain.Common;

namespace Domain.Entities;

public class TodoItem : BaseEntity
{
    public required string ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public DateTime? Reminder { get; set; }

    public TodoList List { get; set; } = null!;
}
