using System.Collections.Generic;
using tabuleiro;

namespace xadrez
{
    public class PartidaDeXadrez
    {
        public Tabuleiro Tabuleiro { get; private set; }
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> Pecas;
        private HashSet<Peca> Captudaras;
        public bool Xeque { get; private set; }

        public PartidaDeXadrez()
        {
            Tabuleiro = new Tabuleiro(8, 8);
            Turno = 1;
            JogadorAtual = Cor.Branca;
            Terminada = false;
            Xeque = false;
            Pecas = new HashSet<Peca>();
            Captudaras = new HashSet<Peca>();
            ColocarPecas();
        }

        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca pecaRetirada = Tabuleiro.RetirarPeca(origem);
            pecaRetirada.AlteraQuantidadeDeMovimento(eIncremento: true);
            Peca pecaCapturada = Tabuleiro.RetirarPeca(destino);
            Tabuleiro.ColocarPeca(pecaRetirada, destino);
            if (pecaCapturada != null)
                Captudaras.Add(pecaCapturada);

            return pecaCapturada;
        }

        public void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca pecaRetirada = Tabuleiro.RetirarPeca(destino);
            pecaRetirada.AlteraQuantidadeDeMovimento(eIncremento: false);

            if (pecaCapturada != null)
            {
                Tabuleiro.ColocarPeca(pecaCapturada, destino);
                Captudaras.Remove(pecaCapturada);
            }

            Tabuleiro.ColocarPeca(pecaRetirada, origem);
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);

            if (EstaEmXeque(JogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque.");
            }

            Xeque = EstaEmXeque(PecaAdversaria(JogadorAtual));

            if (TesteXequemate(PecaAdversaria(JogadorAtual)))
            {
                Terminada = true;
            }
            else
            {
                Turno++;
                JogadorAtual = JogadorAtual == Cor.Branca ? Cor.Preta : Cor.Branca;
            }
        }

        public void ValidarPosicaoOrigem(Posicao origem)
        {
            if (Tabuleiro.Peca(origem) == null)
                throw new TabuleiroException("Não existe peça na posição de origem escolhida.");

            if (JogadorAtual != Tabuleiro.Peca(origem).Cor)
                throw new TabuleiroException("A peça de origem escolhida não é sua!");

            if (!Tabuleiro.Peca(origem).ExisteMovimentosPossiveis())
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida!");
        }

        public void ValidarPosicaoDestino(Posicao origem, Posicao destino)
        {
            if (!Tabuleiro.Peca(origem).PodeMoverPara(destino))
                throw new TabuleiroException("Posição de destino inválida!");
        }

        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca peca in Captudaras)
            {
                if (peca.Cor == cor)
                    aux.Add(peca);
            }
            return aux;
        }

        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach (Peca peca in Pecas)
            {
                if (peca.Cor == cor)
                    aux.Add(peca);
            }
            aux.ExceptWith(PecasCapturadas(cor));
            return aux;
        }

        private Cor PecaAdversaria(Cor cor) => cor == Cor.Branca ? Cor.Preta : Cor.Branca;

        private Peca Rei(Cor cor)
        {
            foreach (Peca x in PecasEmJogo(cor))
            {
                if (x is Rei)
                    return x;
            }
            return null;
        }

        public bool EstaEmXeque(Cor cor)
        {
            Peca rei = Rei(cor);
            if (rei == null)
                throw new TabuleiroException("Não existe um rei da cor " + cor + " no tabuleiro!");

            foreach (Peca x in PecasEmJogo(PecaAdversaria(cor)))
            {
                bool[,] mat = x.MovimentosPossiveis();
                if (mat[rei.Posicao.Linha, rei.Posicao.Coluna])
                    return true;
            }

            return false;
        }

        public bool TesteXequemate(Cor cor)
        {
            if (!EstaEmXeque(cor))
                return false;

            System.Console.WriteLine(!EstaEmXeque(cor));

            foreach (Peca x in PecasEmJogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis(); //ajuda a validar onde tem pessas companheiras
                for (int i = 0; i < Tabuleiro.Linhas; i++)
                {
                    for (int j = 0; j < Tabuleiro.Colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem, destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(origem, destino, pecaCapturada);

                            if (!testeXeque)
                                return false;
                        }
                    }
                }
            }
            return true;
        }

        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tabuleiro.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ConvertePosicao());
            Pecas.Add(peca);
        }

        private void ColocarPecas()
        {
            // ColocarNovaPeca('c', 1, new Torre(Tabuleiro, Cor.Branca));
            // ColocarNovaPeca('c', 2, new Torre(Tabuleiro, Cor.Branca));
            // ColocarNovaPeca('d', 2, new Torre(Tabuleiro, Cor.Branca));
            // ColocarNovaPeca('e', 2, new Torre(Tabuleiro, Cor.Branca));
            // ColocarNovaPeca('e', 1, new Torre(Tabuleiro, Cor.Branca));
            // ColocarNovaPeca('d', 1, new Rei(Tabuleiro, Cor.Branca));

            // ColocarNovaPeca('c', 7, new Torre(Tabuleiro, Cor.Preta));
            // ColocarNovaPeca('c', 8, new Torre(Tabuleiro, Cor.Preta));
            // ColocarNovaPeca('d', 7, new Torre(Tabuleiro, Cor.Preta));
            // ColocarNovaPeca('e', 7, new Torre(Tabuleiro, Cor.Preta));
            // ColocarNovaPeca('e', 8, new Torre(Tabuleiro, Cor.Preta));
            // ColocarNovaPeca('d', 8, new Rei(Tabuleiro, Cor.Preta));
            ColocarNovaPeca('c', 1, new Torre(Tabuleiro, Cor.Branca));
            ColocarNovaPeca('d', 1, new Rei(Tabuleiro, Cor.Branca));
            ColocarNovaPeca('h', 7, new Torre(Tabuleiro, Cor.Branca));

            ColocarNovaPeca('a', 8, new Rei(Tabuleiro, Cor.Preta));
            ColocarNovaPeca('b', 8, new Torre(Tabuleiro, Cor.Preta));
        }
    }
}