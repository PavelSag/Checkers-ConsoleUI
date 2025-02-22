using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    internal class Board
    {
        private int m_BoardSize = 0;
        private Square[,] m_GameBoard = null;

        public Board()
        {
            BoardSize = ConsoleUI.GetBoardSize();
            m_GameBoard = new Square[BoardSize, BoardSize];
        }

        public Square[,] GameBoard
        {
            get { return m_GameBoard; }
            set
            {
                m_GameBoard = value;
            }
        }

        public int BoardSize
        {
            get { return m_BoardSize; }
            set
            {
                value = ConsoleUI.CheckBoardSize(value);
                m_BoardSize = value;
            }
        }

        internal void SetBoard()
        {
            CreateEmptyBoard();
            PlacePiecesOnBoard();
        }

        private void CreateEmptyBoard()
        {
            for(int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    GameBoard[i, j] = new Square();
                }
            }
        }

        private void PlacePiecesOnBoard()
        {
            for (int row = 0; row < m_BoardSize; row++)
            {
                for (int col = 0; col < m_BoardSize; col++)
                {
                    if ((row + col) % 2 == 1)
                    {
                        if (row < (m_BoardSize / 2) - 1)
                        {
                            GameBoard[row, col].SquareOwner = eSquareOwner.Player2;
                            GameBoard[row, col].PieceInSquare = ePieceInSquare.RegularPiece;
                        }
                        else if (row > (m_BoardSize / 2))
                        {
                            GameBoard[row, col].SquareOwner = eSquareOwner.Player1;
                            GameBoard[row, col].PieceInSquare = ePieceInSquare.RegularPiece;
                        }
                    }
                }
            }
        }

        internal int CalculatePoints(Player i_Player1, Player i_Player2)
        {
            for (int row = 0; row < m_BoardSize; row++)
            {
                for (int col = 0; col < m_BoardSize; col++)
                {
                    if(this.GameBoard[row, col].SquareOwner == eSquareOwner.Player1)
                    {
                        this.AddPoints(i_Player1, row, col);
                    }
                    else if(this.GameBoard[row, col].SquareOwner == eSquareOwner.Player2)
                    {
                        this.AddPoints(i_Player2, row, col);
                    }
                }
            }

            return Math.Abs(i_Player1.NumOfPoints - i_Player2.NumOfPoints);
        }

        private void AddPoints(Player i_Player, int i_Row, int i_Col)
        {
            if (this.GameBoard[i_Row, i_Col].PieceInSquare == ePieceInSquare.RegularPiece)
            {
                i_Player.NumOfPoints++;
            }
            else if (this.GameBoard[i_Row, i_Col].PieceInSquare == ePieceInSquare.KingPiece)
            { 
                i_Player.NumOfPoints += 4;
            }
        }

        internal void ResetBoard()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    GameBoard[i, j].PieceInSquare = ePieceInSquare.Empty;
                    GameBoard[i, j].SquareOwner = eSquareOwner.None;
                }
            }

            this.PlacePiecesOnBoard();
        }
    } 
}
