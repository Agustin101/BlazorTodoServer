using blazorapp.models;

namespace blazorapp.Services.Account
{
    public interface IAccountService
    {
        Task<LoginResponse> LoginAsync(string username, string password);
        Task<string> GetUserName();
    }
}
