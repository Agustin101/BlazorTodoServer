using blazorapp.Authentication;
using blazorapp.models;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace blazorapp.Services.Account
{
    public class AccountService : IAccountService
    {

        private HttpClient _httpClient;
        private ProtectedSessionStorage _protectedSessionStorage;
        private CustomAuthenticationStateProvider _authenticationStateProvider;


        public AccountService(IHttpClientFactory httpClientFactory, ProtectedSessionStorage protectedSessionStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClientFactory.CreateClient("ServerApi");
            _protectedSessionStorage = protectedSessionStorage;
            _authenticationStateProvider = authenticationStateProvider as CustomAuthenticationStateProvider;
        }

        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsync("api/authentication/login",
                                                JsonContent.Create(new LoginModel() { UserName= username, Password= password }));

                if (!response.IsSuccessStatusCode)
                    throw new UnauthorizedAccessException("Login failed.");

                var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (content == null)
                    throw new InvalidDataException();

                await _protectedSessionStorage.SetAsync("JWT_KEY", content.JwtToken);
                await _authenticationStateProvider.UpdateAuthenticationState(new UserSession() { Role = "admin", UserName = GetUsername(content.JwtToken) });


                return content;

            }
            catch (Exception ex)
            {
            }

            //LoginChange?.Invoke(GetUsername(content.JwtToken));

            return null;
        }

        private string GetUsername(string token)
        {
            var jwt = new JwtSecurityToken(token);

            return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        }

        public async Task<string> GetUserName()
        {
            var token = await _protectedSessionStorage.GetAsync<string>("JWT_KEY");
            return GetUsername(token.Value);
        }
    }
}
