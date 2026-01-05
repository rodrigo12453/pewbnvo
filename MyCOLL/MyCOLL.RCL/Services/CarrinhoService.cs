using MyCOLL.Data.Models;

namespace MyCOLL.RCL.Services
{
    // 1. Classe auxiliar para gerir Quantidades (podes colocar noutro ficheiro se preferires)
    public class ItemCarrinho
    {
        public Produto Produto { get; set; } = new();
        public int Quantidade { get; set; }
    }

    public class CarrinhoService
    {
        // 2. Agora a lista é de 'ItemCarrinho' e não apenas 'Produto'
        private List<ItemCarrinho> _carrinho = new List<ItemCarrinho>();

        public event Action? OnChange;

        // Adicionar produto (agora verifica se já existe para somar quantidade)
        public void Adicionar(Produto produto)
        {
            var itemExistente = _carrinho.FirstOrDefault(x => x.Produto.Id == produto.Id);

            if (itemExistente != null)
            {
                // Se já existe, só aumenta a quantidade
                itemExistente.Quantidade++;
            }
            else
            {
                // Se não existe, cria novo item com quantidade 1
                _carrinho.Add(new ItemCarrinho { Produto = produto, Quantidade = 1 });
            }

            NotifyStateChanged();
        }

        // --- NOVOS MÉTODOS PARA OS BOTÕES ---

        public void AumentarQuantidade(Produto produto)
        {
            var item = _carrinho.FirstOrDefault(x => x.Produto.Id == produto.Id);
            if (item != null)
            {
                item.Quantidade++;
                NotifyStateChanged();
            }
        }

        public void DiminuirQuantidade(Produto produto)
        {
            var item = _carrinho.FirstOrDefault(x => x.Produto.Id == produto.Id);
            if (item != null)
            {
                if (item.Quantidade > 1)
                {
                    item.Quantidade--;
                }
                else
                {
                    // Se chegar a 0, removemos o item? 
                    // Geralmente sim, ou deixamos o botão de remover tratar disso.
                    // Para o 20, vamos assumir que o botão "-" não remove, 
                    // para evitar cliques acidentais. O user deve clicar em "Remover".
                }
                NotifyStateChanged();
            }
        }

        public void RemoverItem(Produto produto)
        {
            var item = _carrinho.FirstOrDefault(x => x.Produto.Id == produto.Id);
            if (item != null)
            {
                _carrinho.Remove(item);
                NotifyStateChanged();
            }
        }

        // --- MÉTODOS DE LEITURA ---

        // Nota: Agora retorna List<ItemCarrinho> para teres acesso à .Quantidade no Razor
        public List<ItemCarrinho> ObterItens()
        {
            return _carrinho;
        }

        public decimal ObterTotal()
        {
            // O cálculo agora multiplica pela quantidade
            return _carrinho.Sum(item => item.Produto.PrecoVenda * item.Quantidade);
        }

        public int ObterNumeroItensTotal()
        {
            // Útil para mostrar no ícone do carrinho (ex: tens 5 itens no total, mesmo que sejam só 2 produtos diferentes)
            return _carrinho.Sum(item => item.Quantidade);
        }

        public void LimparCarrinho()
        {
            _carrinho.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}