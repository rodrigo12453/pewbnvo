using Microsoft.Extensions.Logging;
using MyCOLL.RCL.Services;       // Onde está o CarrinhoService
using MyCOLL.RCL.Auth;           // Onde está o CustomAuthStateProvider
using Microsoft.AspNetCore.Components.Authorization;
using MyCOLL.RCL;                // Onde está o Layout partilhado

namespace MyCOLL.Frontend
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // --- 1. Configurar a API (HttpClient) ---
            // IMPORTANTE: No emulador Android, "localhost" é "10.0.2.2".
            // Se estiveres a testar no Windows, podes usar "https://localhost:7000" (verifica a porta da tua API).
            string apiUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5028"      // Android usa a porta HTTP (5028)
                : "https://localhost:7128";   // Windows usa a porta HTTPS (7128)

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });

            // --- 2. Serviços da Aplicação ---
            builder.Services.AddAuthorizationCore(); // Obrigatório para o Login funcionar
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddScoped<CarrinhoService>();

            return builder.Build();
        }
    }
}