namespace ToDoListWebApplication.Models;

internal interface IDefaultConstructible<out T>
{
    static abstract T Default { get; }
    bool IsDefault { get; }
}