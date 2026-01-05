using Microsoft.AspNetCore.Mvc;
using MyCOLL.Data;
using MyCOLL.Data.Models;
using MyCOLL.RCL; // Onde estão os teus DTOs
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MyCOLL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncomendasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EncomendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize] // Obrigatório estar logado
        public async Task<ActionResult> FinalizarCompra(EncomendaDto dto)
        {
            // 1. Descobrir quem é o utilizador através do Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // 2. Criar a Venda (Cabeçalho)
            var venda = new Venda
            {
                ClienteId = userId,
                DataVenda = DateTime.Now,
                Total = dto.Total,
                // Estado = "Pendente" (Se tiveres este campo)
            };

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync(); // Grava para gerar o ID da venda

            // 3. Criar os Detalhes (Linhas da encomenda)
            foreach (var item in dto.Itens)
            {
                var detalhe = new DetalheVenda
                {
                    VendaId = venda.Id,
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                };
                _context.DetalhesVenda.Add(detalhe);

                // Opcional: Abater ao Stock
                var prod = await _context.Produtos.FindAsync(item.ProdutoId);
                if (prod != null) prod.Stock -= item.Quantidade;
            }

            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Encomenda registada com sucesso!", VendaId = venda.Id });
        }
    }
}