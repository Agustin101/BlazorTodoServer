using blazorapp.models;

namespace blazorapp.Services.Todos
{
    public interface ITodoService
    {
        Task<List<TodoItem>> GetAll();  
    }
}
