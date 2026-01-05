using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCOLL.RCL.Components.Layout;
using MyCOLL.Data;
using MyCOLL.Data.Models;
using MyCOLL.RCL.Services;
using MyCOLL.Gestao.Components;
using MyCOLL.RCL.Auth; // Garante que este using existe

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVIÇOS DO BLAZOR ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// CORREÇÃO CRÍTICA PARA O ERRO DE CAST:
// Registamos a tua classe e depois dizemos ao Blazor para a usar como o provedor oficial
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<CustomAuthStateProvider>(); 
builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
    sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7128/")
});

// --- 2. BASE DE DADOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 3. IDENTIDADE (Login) ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// --- 4. OUTROS SERVIÇOS ---
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddControllers();

var app = builder.Build();

// --- 5. SEED DATA ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await MyCOLL.Data.SeedData.EnsurePopulated(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao criar os dados iniciais.");
    }
}

// --- 6. PIPELINE ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(NavMenu).Assembly);

app.Run();