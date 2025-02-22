using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ex02.ConsoleUtils;

namespace Checkers
{
    public class Program
    {
        public static void Main()
        {
            BeginGame();
        }

        private static void BeginGame()
        {
            Player player1 = new Player(false);
            Board board = new Board();
            Player player2 = null;
            bool isAnotherGame = true;
            ConsoleUI ui = new ConsoleUI();

            board.SetBoard();
            if (ui.IsVersusComputer() == true)
            {
                player2 = new Player(true);
            }
            else
            {
                player2 = new Player(false);
            }

            player1.SetNumOfPieces(board.BoardSize);
            player2.SetNumOfPieces(board.BoardSize);

            while(isAnotherGame == true)
            {
                isAnotherGame = ui.RunGame(board, player1, player2);
                board.ResetBoard();
                player1.ResetNumOfPiecesOnBoard(board.BoardSize);
                player2.ResetNumOfPiecesOnBoard(board.BoardSize);
            }

            ui.PrintGoodbye();
        }
    }
}
