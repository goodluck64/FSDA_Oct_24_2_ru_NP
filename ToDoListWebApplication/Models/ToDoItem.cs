using System.Text.Json.Serialization;
using ToDoListWebApplication.Services;

namespace ToDoListWebApplication.Models;

internal class ToDoItem : IDefaultConstructible<ToDoItem>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<string> Tags { get; set; } = [];

    public static ToDoItem Default { get; } = new()
    {
        Id = 0,
    };

    [JsonIgnore]
    public bool IsDefault => Id == Default.Id;
}