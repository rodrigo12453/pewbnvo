using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCOLL.Data.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        // Regra de Negócio: Preço definido pelo Fornecedor
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoBase { get; set; }

        // Regra de Negócio: Preço Final (Base + %) definido pela Loja
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoVenda { get; set; }

        // Importante para a gestão de stock
        public int Stock { get; set; }

        // Estados possíveis: "Pendente", "Ativo", "Vendido", "Inativo"
        public string Estado { get; set; } = "Pendente";

        public string? ImagemUrl { get; set; } // Pode ser null se não tiver foto

        public DateTime DataInsercao { get; set; } = DateTime.Now;

        // --- RELAÇÕES (Foreign Keys) ---

        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        // Quem é o dono do produto? (Fornecedor)
        public string FornecedorId { get; set; } = string.Empty;

        [ForeignKey("FornecedorId")]
        public ApplicationUser? Fornecedor { get; set; }
    }
}