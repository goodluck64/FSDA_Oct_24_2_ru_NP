using ToDoListWebApplication.Models;

namespace ToDoListWebApplication.Services;

internal sealed class ToDoListService : IToDoListService
{
    private List<ToDoItem> _items = new List<ToDoItem>();
    
    public Task<IEnumerable<ToDoItem>> GetAll()
    {
        return Task.FromResult<IEnumerable<ToDoItem>>(_items);
    }

    public Task<ToDoItem?> GetById(int id)
    {
        return Task.FromResult(_items.FirstOrDefault(i => i.Id == id));
    }

    public Task Add(ToDoItem item)
    {
        _items.Add(item);

        return Task.CompletedTask;
    }

    public Task Update(ToDoItem newItem)
    {
        var oldIndex = _items.FindIndex(x => x.Id == newItem.Id);

        if (oldIndex != -1)
        {
            _items[oldIndex] = newItem;
        }


        return Task.CompletedTask;
    }

    public Task Delete(int id)
    {
        var index = _items.FindIndex(x => x.Id == id);

        if (index != -1)
        {
            _items.RemoveAt(index);
        }
        
        return Task.CompletedTask;
    }
}