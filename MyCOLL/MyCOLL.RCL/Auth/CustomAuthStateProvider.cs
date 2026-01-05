using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using Microsoft.JSInterop; // <--- OBRIGATÓRIO

namespace MyCOLL.RCL.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _jsRuntime; // <--- OBRIGATÓRIO
        private string? _jwtToken;

        // Atualiza o construtor para receber o JS
        public CustomAuthStateProvider(HttpClient http, IJSRuntime jsRuntime)
        {
            _http = http;
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            try
            {
                // O Blazor Server pode falhar aqui se a ligação SignalR não estiver pronta
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _jwtToken = token;
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch
            {
                // Impede que a aplicação crasha se o browser ainda não responder
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void MarkUserAsAuthenticated(string token)
        {
            _jwtToken = token;
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"))));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            _jwtToken = null;
            // O await aqui é crítico para o erro não acontecer
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");

            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}