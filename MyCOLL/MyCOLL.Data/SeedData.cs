using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyCOLL.Data.Models; 


namespace MyCOLL.Data
{
    public static class SeedData
    {
        public static async Task EnsurePopulated(IServiceProvider serviceProvider)
        {
            // Serviços necessários
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            // IMPORTANTE: Precisamos do Contexto para adicionar Produtos e Categorias
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // =========================================================================
            // 1. ROLES E UTILIZADORES
            // =========================================================================
            string[] roles = { "Administrador", "Funcionario", "Cliente", "Fornecedor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // --- Criar Administrador ---
            var adminEmail = "admin@loja.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    
                };
                await userManager.CreateAsync(adminUser, "PWeb@2025");
                await userManager.AddToRoleAsync(adminUser, "Administrador");
            }

            // --- Criar Fornecedor de Teste (Essencial para demonstração) ---
            var fornEmail = "fornecedor@loja.com";
            var fornUser = await userManager.FindByEmailAsync(fornEmail);
            if (fornUser == null)
            {
                fornUser = new ApplicationUser
                {
                    UserName = fornEmail,
                    Email = fornEmail,
                    EmailConfirmed = true,
                    
                };
                await userManager.CreateAsync(fornUser, "PWeb@2025");
                await userManager.AddToRoleAsync(fornUser, "Fornecedor");
            }

            // =========================================================================
            // 2. DADOS DE DEMONSTRAÇÃO (CATEGORIAS E PRODUTOS)
            // =========================================================================

            // Verifica se já existem categorias, se não, cria
            if (!context.Categorias.Any())
            {
                var catMoedas = new Categoria { Nome = "Moedas", Descricao = "Numismática" };
                var catSelos = new Categoria { Nome = "Selos", Descricao = "Filatelia" };

                context.Categorias.AddRange(catMoedas, catSelos);
                await context.SaveChangesAsync(); // Guarda para gerar os IDs

                // Verifica se já existem produtos, se não, cria
                if (!context.Produtos.Any())
                {
                    context.Produtos.AddRange(
                        new Produto
                        {
                            Nome = "Moeda 2 Euros Comemorativa",
                            Descricao = "Moeda rara de 2020",
                            PrecoBase = 10.0m,
                            PrecoVenda = 12.5m, // Preço com margem
                            Estado = "Ativo",   // Já visível na loja
                            CategoriaId = catMoedas.Id,
                            FornecedorId = fornUser.Id, // Associa ao fornecedor criado acima
                            Stock = 5,
                            ImagemUrl = "moeda1.jpg" // Podes por URLs falsos por enquanto
                        },
                        new Produto
                        {
                            Nome = "Selo D. Afonso Henriques",
                            Descricao = "Selo antigo de 1950",
                            PrecoBase = 50.0m,
                            PrecoVenda = 60.0m,
                            Estado = "Ativo",
                            CategoriaId = catSelos.Id,
                            FornecedorId = fornUser.Id,
                            Stock = 1,
                            ImagemUrl = "selo1.jpg"
                        },
                        // Produto Pendente (para testares a aprovação no Backoffice)
                        new Produto
                        {
                            Nome = "Carteira de Fósforos Rara",
                            Descricao = "A aguardar aprovação",
                            PrecoBase = 5.0m,
                            PrecoVenda = 0m, // Ainda não definido
                            Estado = "Pendente",
                            CategoriaId = catMoedas.Id, // (Ou outra categoria)
                            FornecedorId = fornUser.Id,
                            Stock = 10
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}