using System.Security.Claims;
using System.Text.Json;
using ApiContracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorApp.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private const string StorageKey = "currentUser";

    private readonly HttpClient httpClient;
    private readonly ProtectedSessionStorage sessionStorage;
    private ClaimsPrincipal currentClaimsPrincipal = new(new ClaimsIdentity());
    private bool restored;

    public SimpleAuthProvider(HttpClient httpClient, ProtectedSessionStorage sessionStorage)
    {
        this.httpClient = httpClient;
        this.sessionStorage = sessionStorage;
    }

    public async Task Login(string username, string password)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "auth/login",
            new LoginRequest
            {
                Username = username,
                Password = password
            });

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        currentClaimsPrincipal = BuildPrincipal(userDto);
        restored = true;

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentClaimsPrincipal))
        );

        try
        {
            await sessionStorage.SetAsync(StorageKey, userDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not save login to session storage: {e.Message}");
        }
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!restored)
        {
            try
            {
                var stored = await sessionStorage.GetAsync<UserDto>(StorageKey);
                if (stored.Success && stored.Value != null)
                {
                    currentClaimsPrincipal = BuildPrincipal(stored.Value);
                }

                restored = true;
            }
            catch (InvalidOperationException)
            {
                
            }
            catch (Exception)
            {
                restored = true;
            }
        }

        return new AuthenticationState(currentClaimsPrincipal);
    }

    public async Task Logout()
    {
        currentClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        restored = true;

        try
        {
            await sessionStorage.DeleteAsync(StorageKey);
        }
        catch (InvalidOperationException)
        {
            
        }

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(currentClaimsPrincipal)));
    }

    private static ClaimsPrincipal BuildPrincipal(UserDto userDto)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, userDto.UserName),
            new Claim("Id", userDto.Id.ToString()),
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "apiauth"));
    }
}