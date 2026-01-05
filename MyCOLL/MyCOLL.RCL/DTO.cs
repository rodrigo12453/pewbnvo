namespace MyCOLL.RCL
{
    // --- O QUE JÁ TINHAS (ENCOMENDAS) ---
    public class EncomendaDto
    {
        public string? ClienteId { get; set; }
        public decimal Total { get; set; }
        public List<ItemEncomendaDto> Itens { get; set; } = new();
    }

    public class ItemEncomendaDto
    {
        public int ProdutoId { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
    }

    // --- O QUE FALTAVA (AUTENTICAÇÃO) ---
    // Sem isto, o Login.razor e o Register.razor dão erro!

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}