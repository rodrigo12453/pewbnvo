using System.ComponentModel.DataAnnotations.Schema;
using MyCOLL.Data.Models;

namespace MyCOLL.Data
{
    public class DetalheVenda
    {
        public int Id { get; set; }

        // A que venda isto pertence?
        public int VendaId { get; set; }
        public virtual Venda Venda { get; set; } = null!;

        // Que produto foi?
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; } = null!;

        public int Quantidade { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; } // O preço no momento da compra
    }
}