using ToDoListWebApplication.Models;
using ToDoListWebApplication.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IToDoListService, ToDoListService>();

var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("api/v1/todolist", async (IToDoListService toDoListService) => await toDoListService.GetAll());

app.MapGet("api/v1/todolist/{id:int}",
    async (int id, IToDoListService toDoListService) => await toDoListService.GetById(id));

app.MapPost("api/v1/todolist", async (ToDoItem toDoItem, IToDoListService toDoListService) =>
{
    await toDoListService.Add(toDoItem);

    return Results.Created();
});

app.MapDelete("api/v1/todolist/{id:int}", async (int id, IToDoListService toDoListService) =>
{
    await toDoListService.Delete(id);

    return Results.Ok();
});

app.MapPut("api/v1/todolist", async (ToDoItem toDoItem, IToDoListService toDoListService) =>
{
    await toDoListService.Update(toDoItem);

    return Results.Ok();
});

app.Run();