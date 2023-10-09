using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using blazorapp.models;
using Blazored.SessionStorage;

namespace blazorapp.Services;

public class AuthenticationService : IAuthenticationService
{
  private const string JWT_KEY = nameof(JWT_KEY);
  private const string REFRESH_KEY = nameof(REFRESH_KEY);
  private readonly HttpClient _httpClient;
  private readonly IHttpClientFactory _factory;
  private ISessionStorageService _sessionStorageService;


  public AuthenticationService(IHttpClientFactory httpClientFactory, IHttpClientFactory factory, ISessionStorageService sessionStorageService)
  {
    _httpClient = httpClientFactory.CreateClient("ServerApi");
    _factory = factory;
    _sessionStorageService = sessionStorageService;
  }

  public event Action<string?>? LoginChange;

  public ValueTask<string> GetJwtAsync()
  {
    throw new NotImplementedException();
  }

  public async Task<DateTime> LoginAsync(LoginModel loginModel)
  {
    var response = await _factory.CreateClient("ServerApi").PostAsync("api/authentication/login",
                                                            JsonContent.Create(loginModel));

    if (!response.IsSuccessStatusCode)
      throw new UnauthorizedAccessException("Login failed.");

    var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

    if (content == null)
      throw new InvalidDataException();

    await _sessionStorageService.SetItemAsync(JWT_KEY, content.JwtToken);
    await _sessionStorageService.SetItemAsync(REFRESH_KEY, content.RefreshToken);

    LoginChange?.Invoke(GetUsername(content.JwtToken));

    return content.Expiration;
  }

  private static string GetUsername(string token)
  {
    var jwt = new JwtSecurityToken(token);

    return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
  }
  public Task LogoutAsync()
  {
    throw new NotImplementedException();
  }

  public Task<bool> RefreshAsync()
  {
    throw new NotImplementedException();
  }
}