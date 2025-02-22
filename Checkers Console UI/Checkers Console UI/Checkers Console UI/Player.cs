using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    internal class Player
    {
        private string m_PlayerName = null;
        private int m_Score = 0;
        private int m_NumOfPiecesOnBoard = 0;

        public Player(bool i_IsVersusComputer)
        {
            if(i_IsVersusComputer != true)
            {
                PlayerName = ConsoleUI.GetPlayerName();
            }
            else
            {
                PlayerName = "Computer";
            }

            NumOfPoints = 0;
        }

        public string PlayerName
        {
            get { return m_PlayerName; }
            set
            {
                value = ConsoleUI.CheckPlayerName(value);
                m_PlayerName = value;
            }
        }

        internal int NumOfPoints
        {
            get { return m_Score; }
            set
            {
                m_Score = value;
            }
        }

        internal int NumOfPiecesOnBoard
        {
            get { return m_NumOfPiecesOnBoard; }
            set
            {
                m_NumOfPiecesOnBoard = value;
            }
        }

        internal void SetNumOfPieces(int i_BoardSize)
        {
            NumOfPiecesOnBoard = (i_BoardSize / 2 - 1) * (i_BoardSize/2);
        }

        internal void ResetNumOfPiecesOnBoard(int i_BoardSize)
        {
            SetNumOfPieces(i_BoardSize);
        } 
    }
}
