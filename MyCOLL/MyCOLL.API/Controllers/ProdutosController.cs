using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCOLL.Data.Models;
using MyCOLL.Data;
using Microsoft.AspNetCore.Authorization;

namespace MyCOLL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env; // <--- NOVO: Para aceder às pastas

        // Injetamos o Ambiente (_env) no construtor
        public ProdutosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _context.Produtos
                                 .Include(p => p.Categoria)
                                 .ToListAsync();
        }

        // GET: api/produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            return produto;
        }

        // POST: api/produtos
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            produto.Id = 0;
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
        }

        // PUT: api/produtos/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id) return BadRequest("O ID do produto não corresponde.");

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/produtos/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }

        // --- NOVO MÉTODO PARA UPLOAD DE IMAGEM ---
        [HttpPost("upload")]
        public async Task<ActionResult<string>> UploadImagem(IFormFile ficheiro)
        {
            if (ficheiro == null || ficheiro.Length == 0)
                return BadRequest("Nenhum ficheiro enviado.");

            // 1. Cria um nome único (para não haver conflitos)
            var nomeFicheiro = Guid.NewGuid().ToString() + Path.GetExtension(ficheiro.FileName);

            // 2. Define a pasta de destino: wwwroot/imagens
            var pastaDestino = Path.Combine(_env.WebRootPath, "imagens");

            // Cria a pasta se não existir
            if (!Directory.Exists(pastaDestino)) Directory.CreateDirectory(pastaDestino);

            var caminhoCompleto = Path.Combine(pastaDestino, nomeFicheiro);

            // 3. Grava o ficheiro no disco do servidor
            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await ficheiro.CopyToAsync(stream);
            }

            // 4. Cria o URL público para devolver ao Frontend
            // Ex: https://localhost:7000/imagens/foto123.jpg
            var urlBase = $"{Request.Scheme}://{Request.Host}";
            var urlImagem = $"{urlBase}/imagens/{nomeFicheiro}";

            return Ok(new { Url = urlImagem });
        }
    }
}