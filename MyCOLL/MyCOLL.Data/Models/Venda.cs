using System.ComponentModel.DataAnnotations.Schema;

namespace MyCOLL.Data
{
    public class Venda
    {
        public int Id { get; set; }
        public string ClienteId { get; set; } = string.Empty;
        public ApplicationUser? Cliente { get; set; }
        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string Estado { get; set; } = "Pendente";

        // Relação com os itens vendidos 
        public List<DetalheVenda> Detalhes { get; set; } = new();
    }
}