using System.ComponentModel.DataAnnotations;

namespace MyCOLL.Data.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        // Relação: Uma categoria tem muitos produtos
        public List<Produto> Produtos { get; set; } = new();
    }
}