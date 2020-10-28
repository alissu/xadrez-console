using System;
using tabuleiro;

namespace xadrez_console
{
    public class Tela
    {
        public static void ImprimirTabuleiroTab(Tabuleiro tabuleiro)
        {
            for (int i = 0; i < tabuleiro.Linhas; i++)
            {
                for (int j = 0; j < tabuleiro.Colunas; j++)
                {
                    if (tabuleiro.GetPeca(i, j) == null)
                        Console.Write("- ");
                    else
                        Console.Write(tabuleiro.GetPeca(i, j) + " ");
                }

                Console.WriteLine();
            }
        }
    }
}