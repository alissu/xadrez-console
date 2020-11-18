using tabuleiro;

namespace xadrez
{
    public class Torre : Peca
    {
        public Torre(Tabuleiro tabuleiro, Cor cor) : base(tabuleiro, cor)
        {
        }

        public override string ToString()
        {
            return "T";
        }

        private bool PodeMover(Posicao posicao)
        {
            Peca peca = Tabuleiro.Peca(posicao);
            return peca == null || peca.Cor != Cor;
        }

        public override bool[,] MovimentosPossiveis()
        {
            bool[,] mat = new bool[Tabuleiro.Linhas, Tabuleiro.Colunas];

            Posicao newPosicao = new Posicao(0, 0); //TODO: validar nome da 

            // acima
            newPosicao.DefinirValores(Posicao.Linha - 1, Posicao.Coluna);

            while (Tabuleiro.PosicaoValida(newPosicao) && PodeMover(newPosicao))
            {
                mat[newPosicao.Linha, newPosicao.Coluna] = true;
                if (Tabuleiro.Peca(newPosicao) != null && Tabuleiro.Peca(newPosicao).Cor != Cor)
                    break;

                newPosicao.Linha -= 1;
            }

            // abaixo
            newPosicao.DefinirValores(Posicao.Linha + 1, Posicao.Coluna);

            while (Tabuleiro.PosicaoValida(newPosicao) && PodeMover(newPosicao))
            {
                mat[newPosicao.Linha, newPosicao.Coluna] = true;
                if (Tabuleiro.Peca(newPosicao) != null && Tabuleiro.Peca(newPosicao).Cor != Cor)
                    break;

                newPosicao.Linha += 1;
            }

            // direita
            newPosicao.DefinirValores(Posicao.Linha, Posicao.Coluna + 1);
            while (Tabuleiro.PosicaoValida(newPosicao) && PodeMover(newPosicao))
            {
                mat[newPosicao.Linha, newPosicao.Coluna] = true;
                if (Tabuleiro.Peca(newPosicao) != null && Tabuleiro.Peca(newPosicao).Cor != Cor)
                    break;

                newPosicao.Coluna += 1;
            }

            // esquerda
            newPosicao.DefinirValores(Posicao.Linha, Posicao.Coluna - 1);

            while (Tabuleiro.PosicaoValida(newPosicao) && PodeMover(newPosicao))
            {
                mat[newPosicao.Linha, newPosicao.Coluna] = true;
                if (Tabuleiro.Peca(newPosicao) != null && Tabuleiro.Peca(newPosicao).Cor != Cor)
                    break;

                newPosicao.Coluna -= 1;
            }

            return mat;
        }
    }
}