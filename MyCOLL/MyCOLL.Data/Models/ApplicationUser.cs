using Microsoft.AspNetCore.Identity;
using MyCOLL.Data.Models;
namespace MyCOLL.Data
{
    public class ApplicationUser : IdentityUser
    {
        
        public string NIF { get; set; } = string.Empty;
        public string Morada { get; set; } = string.Empty;

        public bool EstaAtivo { get; set; } = false;

        public virtual ICollection<Produto> MeusProdutos { get; set; } = new List<Produto>();
    }
}