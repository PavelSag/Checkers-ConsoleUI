using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    internal class GameLogic
    {
        private bool m_IsGameGoing = true;
        private string m_PlayerMove = null;
        private bool m_IsForfeited = false;
        private bool m_IsAnotherGame = false;
        private bool m_IsAnotherCapture = true;
        private Player m_Winner = null;
        private Player m_Loser = null;
        internal static eTurnStatus s_TurnStatus = eTurnStatus.Player1Turn;
        internal static bool s_ComputerIndicator = false;

        public GameLogic()
        {

        }

        public Player Winner
        {
            get { return m_Winner; }
            set
            {
                m_Winner = value;
            }

        }

        public Player Loser
        {
            get { return m_Loser; }
            set
            {
                m_Loser = value;
            }

        }

        public bool IsAnotherCapture
        {
            get { return m_IsAnotherCapture; }
            set
            {
                m_IsAnotherCapture = value;
            }
        }

        public bool IsGameGoing
        {
            get { return m_IsGameGoing; }
            set
            {
                m_IsGameGoing = value;
            }
        }

        public string PlayerMove
        {
            get { return m_PlayerMove; }
            set
            {
                m_PlayerMove = value;
            }
        }

        public bool IsForfeited
        {
            get { return m_IsForfeited; }
            set
            {
                m_IsForfeited = value;
            }
        }

        public bool IsAnotherGame
        {
            get { return m_IsAnotherGame; }
            set
            {
                m_IsAnotherGame = value;
            }
        }

        internal bool CheckIfLegalAndMove(ref bool isPieceCaptured, Board i_Board, ConsoleUI i_UI)
        {
            OriginAndDestinationSquare fromAndToSquare = new OriginAndDestinationSquare();
            bool isLegalMove = true;
            bool isCaptureChosen = false;

            isLegalMove = i_UI.CheckInputSyntax(this.PlayerMove);
            if (isLegalMove == true)
            {
                fromAndToSquare = i_UI.ParseMove(this.PlayerMove, fromAndToSquare);
            }

            if (isLegalMove == true)
            {
                isLegalMove = CheckIfLegal(fromAndToSquare, i_Board);
            }

            List<OriginAndDestinationSquare> captureOptionsArray = new List<OriginAndDestinationSquare>();
            bool isThereACapture = GiveArrayOfCaptureOptionsFromWholeBoard(captureOptionsArray, i_Board);

            if(isThereACapture == true && isCaptureChosen != true)
            {
                for(int i = 0; i< captureOptionsArray.Count; i++)
                {
                    if (captureOptionsArray[i].Equals(fromAndToSquare))
                    {
                        isCaptureChosen = true;
                    }
                }

                if (isCaptureChosen == false)
                {
                    isLegalMove = false;
                }
            }

            if (isLegalMove == true)
            {
                isPieceCaptured = MovePiece(fromAndToSquare, i_Board);
            }

            return isLegalMove;
        }

        private bool GiveArrayOfCaptureOptionsFromWholeBoard(List<OriginAndDestinationSquare> i_CaptureOptionsArray, Board i_Board)
        {
            bool returnFlag = false;
            OriginAndDestinationSquare fromAndToSquare = new OriginAndDestinationSquare();

            for (int i = 0; i < i_Board.BoardSize; i++) 
            {
                for (int j = 0; j < i_Board.BoardSize; j++) 
                {
                    fromAndToSquare.m_RowOrigin = i;
                    fromAndToSquare.m_ColOrigin = j;
                    GiveArrayOfCaptureOptionsFromOneSquare(i_CaptureOptionsArray, fromAndToSquare, i_Board);
                }
            }

            if(i_CaptureOptionsArray.Count == 0)
            {
                returnFlag = false;
            }
            else
            {
                returnFlag = true;
            }

            return returnFlag;
        }

        private bool CheckIfLegal(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool returnFlag = true;

            returnFlag = CheckBoundaries(i_FromAndToSquare, i_Board.BoardSize);

            if (returnFlag == true)
            {
                returnFlag = CheckIfDiagonalIsValid(i_FromAndToSquare, i_Board);
            }

            if (returnFlag == true)
            {
                returnFlag = CheckIfPlayerMovesOthersPiece(i_FromAndToSquare, i_Board);
            }

            return returnFlag;
        }

        internal bool CheckBoundaries(OriginAndDestinationSquare i_FromAndToSquare, int i_BoardSize)
        {
            bool returnFlag = false;

            if (i_FromAndToSquare.m_RowOrigin >= 0 && i_FromAndToSquare.m_RowOrigin < i_BoardSize &&
                i_FromAndToSquare.m_ColOrigin >= 0 && i_FromAndToSquare.m_ColOrigin < i_BoardSize &&
                i_FromAndToSquare.m_RowDestination >= 0 && i_FromAndToSquare.m_RowDestination < i_BoardSize &&
                i_FromAndToSquare.m_ColDestination >= 0 && i_FromAndToSquare.m_ColDestination < i_BoardSize)
            {
                returnFlag = true;
            }

            return returnFlag;
        }

        internal bool CheckIfDiagonalIsValid(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool returnFlag = true;
            Square originSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin];
            Square destinationSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowDestination, i_FromAndToSquare.m_ColDestination];

            if (destinationSquare.PieceInSquare != ePieceInSquare.Empty || destinationSquare.SquareOwner == originSquare.SquareOwner)
            {
                returnFlag = false;
            }

            if (returnFlag == true)
            {
                returnFlag = CheckCapture(i_FromAndToSquare, i_Board);
            }

            if (returnFlag == true)
            {
                returnFlag = CheckKingMove(i_FromAndToSquare, i_Board);
            }

            return returnFlag;
        }

        private bool CheckCapture(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool returnFlag = true;

            Square originSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin];
            Square destinationSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowDestination, i_FromAndToSquare.m_ColDestination];
            int rowDiff = Math.Abs(i_FromAndToSquare.m_RowDestination - i_FromAndToSquare.m_RowOrigin);
            int colDiff = Math.Abs(i_FromAndToSquare.m_ColDestination - i_FromAndToSquare.m_ColOrigin);

            if (rowDiff == 2 && colDiff == 2)
            {
                int midRow = (i_FromAndToSquare.m_RowOrigin + i_FromAndToSquare.m_RowDestination) / 2;
                int midCol = (i_FromAndToSquare.m_ColOrigin + i_FromAndToSquare.m_ColDestination) / 2;
                Square midSquare = i_Board.GameBoard[midRow, midCol];

                if (midSquare.SquareOwner != originSquare.SquareOwner && midSquare.SquareOwner != eSquareOwner.None)
                {
                    returnFlag = true; ;
                }
                else
                {
                    returnFlag = false;
                }
            }
            else
            {
                if (originSquare.SquareOwner == destinationSquare.SquareOwner)
                {
                    returnFlag = false;
                }
            }

            return returnFlag;
        }

        private bool CheckKingMove(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool returnFlag = true;
            Square originSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin];
            int rowDiff = Math.Abs(i_FromAndToSquare.m_RowDestination - i_FromAndToSquare.m_RowOrigin);
            int colDiff = Math.Abs(i_FromAndToSquare.m_ColDestination - i_FromAndToSquare.m_ColOrigin);

            if (originSquare.PieceInSquare == ePieceInSquare.KingPiece)
            {
                if ((rowDiff == 1 && colDiff == 1) || (rowDiff == 2 && colDiff == 2))
                {
                    returnFlag = true;
                }
                else
                {
                    returnFlag = false;
                }
            }
            else
            {
                bool isMovingForward = (originSquare.SquareOwner == eSquareOwner.Player1 && i_FromAndToSquare.m_RowDestination < i_FromAndToSquare.m_RowOrigin) ||
                                       (originSquare.SquareOwner == eSquareOwner.Player2 && i_FromAndToSquare.m_RowDestination > i_FromAndToSquare.m_RowOrigin);

                if (isMovingForward == true && rowDiff <= 2 && colDiff <= 2)
                {
                    returnFlag = true;
                }
                else
                {
                    returnFlag = false;
                }
            }

            return returnFlag;
        }

        private bool CheckIfPlayerMovesOthersPiece(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool returnFlag = true;
            Square originSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin];

            if (originSquare.SquareOwner == eSquareOwner.Player1 && s_TurnStatus != eTurnStatus.Player1Turn)
            {
                returnFlag = false;
            }
            else if (originSquare.SquareOwner == eSquareOwner.Player2 && s_TurnStatus != eTurnStatus.Player2Turn)
            {
                returnFlag = false;
            }
            else if (originSquare.SquareOwner == eSquareOwner.None)
            {
                returnFlag = false;
            }

            return returnFlag;
        }

        private bool MovePiece(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool isPieceCaptured = false;
            UpdateNewSquareAndEmptyPrevious(i_FromAndToSquare, i_Board);
            isPieceCaptured = UpdateCapture(i_FromAndToSquare, i_Board);
            UpdateKing(i_FromAndToSquare, i_Board);
            return isPieceCaptured;
        }

        private void UpdateNewSquareAndEmptyPrevious(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            i_Board.GameBoard[i_FromAndToSquare.m_RowDestination, i_FromAndToSquare.m_ColDestination].PieceInSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin].PieceInSquare;
            i_Board.GameBoard[i_FromAndToSquare.m_RowDestination, i_FromAndToSquare.m_ColDestination].SquareOwner = i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin].SquareOwner;
            i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin].PieceInSquare = ePieceInSquare.Empty;
            i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin].SquareOwner = eSquareOwner.None;
        }

        private bool UpdateCapture(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool isPieceCaptured = false;

            if (Math.Abs(i_FromAndToSquare.m_RowDestination - i_FromAndToSquare.m_RowOrigin) == 2 && Math.Abs(i_FromAndToSquare.m_ColDestination - i_FromAndToSquare.m_ColOrigin) == 2)
            {
                int midRow = (i_FromAndToSquare.m_RowOrigin + i_FromAndToSquare.m_RowDestination) / 2;
                int midCol = (i_FromAndToSquare.m_ColOrigin + i_FromAndToSquare.m_ColDestination) / 2;
                Square midSquare = i_Board.GameBoard[midRow, midCol];
                midSquare.PieceInSquare = ePieceInSquare.Empty;
                midSquare.SquareOwner = eSquareOwner.None;
                isPieceCaptured = true;
            }

            return isPieceCaptured;
        }

        private void UpdateKing(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            Square destinationSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowDestination, i_FromAndToSquare.m_ColDestination];

            if ((destinationSquare.SquareOwner == eSquareOwner.Player1 && i_FromAndToSquare.m_RowDestination == 0) ||
                    (destinationSquare.SquareOwner == eSquareOwner.Player2 && i_FromAndToSquare.m_RowDestination == i_Board.BoardSize - 1))
            {
                destinationSquare.PieceInSquare = ePieceInSquare.KingPiece;
                i_Board.GameBoard[i_FromAndToSquare.m_RowDestination, i_FromAndToSquare.m_ColDestination].PieceInSquare = ePieceInSquare.KingPiece;
            }
        }

        internal void DecrementNumOfPieces(Player i_Player1, Player i_Player2)
        {
            if (s_TurnStatus == eTurnStatus.Player2Turn || s_TurnStatus == eTurnStatus.ComputerTurn)
            {
                i_Player1.NumOfPiecesOnBoard--;
            }
            else
            {
                i_Player2.NumOfPiecesOnBoard--;
            }
        }

        internal void CheckForWinner(out bool o_IsDraw, Player i_Player1, Player i_Player2, Board i_Board)
        {
            string winnerIndicator = null;
            o_IsDraw = false;

            if (IsForfeited == true)
            {
                if (s_TurnStatus.Equals(eTurnStatus.Player1Turn))
                {
                    this.Winner = i_Player2;
                    this.Loser = i_Player1;
                }
                else
                {
                    this.Winner = i_Player1;
                    this.Loser = i_Player2;
                }
            }
            else
            {
                if (i_Player2.NumOfPiecesOnBoard == 0)
                {
                    this.Winner = i_Player1;
                    this.Loser = i_Player2;
                }
                else if (i_Player1.NumOfPiecesOnBoard == 0)
                {
                    this.Winner = i_Player2;
                    this.Loser = i_Player1;
                }
                else
                {
                    winnerIndicator = CheckIfLegalMovesAreLeftForPlayer(i_Board);
                    if (winnerIndicator != null)
                    {
                        if (winnerIndicator == ("Player1"))
                        {
                            this.Winner = i_Player1;
                            this.Loser = i_Player2;
                        }
                        else
                        {
                            this.Winner = i_Player2;
                            this.Loser = i_Player2;
                        }
                    }
                    else
                    {
                        o_IsDraw = false;
                    }
                }
            }
        }
        private string CheckIfLegalMovesAreLeftForPlayer(Board i_Board)
        {
            string winner = null;
            bool isLegalMoveFoundForPlayer1 = false;
            bool isLegalMoveFoundForPlayer2 = false;
            int[,] directions = { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 }, { -2, -2 }, { -2, 2 }, { 2, -2 }, { 2, 2 } };

            for (int row = 0; row < i_Board.BoardSize; row++)
            {
                for (int col = 0; col < i_Board.BoardSize; col++)
                {
                    for (int i = 0; i < directions.GetLength(0); i++)
                    {
                        int newRow = row + directions[i, 0];
                        int newCol = col + directions[i, 1];
                        OriginAndDestinationSquare fromAndToSquare = new OriginAndDestinationSquare();
                        fromAndToSquare.m_RowOrigin = row;
                        fromAndToSquare.m_ColOrigin = col;
                        fromAndToSquare.m_RowDestination = newRow;
                        fromAndToSquare.m_ColDestination = newCol;

                        if ((isLegalMoveFoundForPlayer1 == true && i_Board.GameBoard[row, col].SquareOwner == eSquareOwner.Player1)
                            || (isLegalMoveFoundForPlayer2 == true && i_Board.GameBoard[row, col].SquareOwner == eSquareOwner.Player2))
                        {
                            continue;
                        }

                        if (CheckBoundaries(fromAndToSquare, i_Board.BoardSize) == true)
                        {
                            if ((CheckIfDiagonalIsValid(fromAndToSquare, i_Board) == true
                                || IsCaptureMoveForComputer(fromAndToSquare, i_Board) == true))
                            {
                                if (i_Board.GameBoard[row, col].SquareOwner == eSquareOwner.Player1)
                                {
                                    isLegalMoveFoundForPlayer1 = true;
                                }
                                else if (i_Board.GameBoard[row, col].SquareOwner == eSquareOwner.Player2)
                                {
                                    isLegalMoveFoundForPlayer2 = true;
                                }
                            }
                        }
                    }
                }
            }

            if (isLegalMoveFoundForPlayer1 == true && isLegalMoveFoundForPlayer2 == true)
            {
                winner = null;
            }

            if (isLegalMoveFoundForPlayer2 == true && isLegalMoveFoundForPlayer1 != true && s_TurnStatus == eTurnStatus.Player2Turn)
            {
                winner = "Player2";
            }

            if (isLegalMoveFoundForPlayer1 == true && isLegalMoveFoundForPlayer2 != true && s_TurnStatus == eTurnStatus.Player1Turn)
            {
                winner = "Player1";
            }

            return winner;
        }

        private bool IsCaptureMoveForComputer(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool returnFlag = false;
            int rowDiff = Math.Abs(i_FromAndToSquare.m_RowDestination - i_FromAndToSquare.m_RowOrigin);
            int colDiff = Math.Abs(i_FromAndToSquare.m_ColDestination - i_FromAndToSquare.m_ColOrigin);

            if (rowDiff == 2 && colDiff == 2)
            {
                int midRow = (i_FromAndToSquare.m_RowOrigin + i_FromAndToSquare.m_RowDestination) / 2;
                int midCol = (i_FromAndToSquare.m_ColOrigin + i_FromAndToSquare.m_ColDestination) / 2;
                Square originSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin];
                Square midSquare = i_Board.GameBoard[midRow, midCol];
                Square destinationSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowDestination, i_FromAndToSquare.m_ColDestination];

                if (midSquare.SquareOwner != originSquare.SquareOwner && midSquare.SquareOwner != eSquareOwner.None && destinationSquare.SquareOwner == eSquareOwner.None)
                {
                    if (originSquare.PieceInSquare == ePieceInSquare.RegularPiece)
                    {
                        int direction = i_FromAndToSquare.m_RowDestination - i_FromAndToSquare.m_RowOrigin;
                        if (originSquare.SquareOwner == eSquareOwner.Player2 && direction > 0)
                        {
                            returnFlag = true;
                        }
                    }
                    else if (originSquare.PieceInSquare == ePieceInSquare.KingPiece)
                    {
                        returnFlag = true;
                    }
                }
            }

            return returnFlag;
        }

        internal void ChangeTurn()
        {
            if (s_TurnStatus == eTurnStatus.Player1Turn)
            {
                s_TurnStatus = eTurnStatus.Player2Turn;
            }
            else
            {
                s_TurnStatus = eTurnStatus.Player1Turn;
            }
        }

        internal string GenerateComputerMove(Board i_Board)
        {
            List<OriginAndDestinationSquare> optionsArray = GetOptionsArray(i_Board);
            string computerMove = ChooseRandomFromOptionsArray(optionsArray, i_Board);
            return computerMove;
        }

        private List<OriginAndDestinationSquare> GetOptionsArray(Board i_Board)
        {
            List<OriginAndDestinationSquare> optionsArray = new List<OriginAndDestinationSquare>();
            int[,] directions = { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 }, { -2, -2 }, { -2, 2 }, { 2, -2 }, { 2, 2 } };

            for (int row = 0; row < i_Board.BoardSize; row++)
            {
                for (int col = 0; col < i_Board.BoardSize; col++)
                {
                    if (i_Board.GameBoard[row, col].SquareOwner == eSquareOwner.Player2)
                    {
                        for (int i = 0; i < directions.GetLength(0); i++)
                        {
                            int newRow = row + directions[i, 0];
                            int newCol = col + directions[i, 1];
                            OriginAndDestinationSquare fromAndToSquare = new OriginAndDestinationSquare();
                            fromAndToSquare.m_RowOrigin = row;
                            fromAndToSquare.m_ColOrigin = col;
                            fromAndToSquare.m_RowDestination = newRow;
                            fromAndToSquare.m_ColDestination = newCol;

                            if (CheckBoundaries(fromAndToSquare, i_Board.BoardSize) == true)
                            {
                                if (CheckIfDiagonalIsValid(fromAndToSquare, i_Board) == true || IsCaptureMoveForComputer(fromAndToSquare, i_Board) == true)
                                {
                                    optionsArray.Add(fromAndToSquare);
                                }
                            }
                        }
                    }
                }
            }

            return optionsArray;
        }

        private string ChooseRandomFromOptionsArray(List<OriginAndDestinationSquare> i_OptionsArray, Board i_Board)
        {
            List<OriginAndDestinationSquare> capturesArray = new List<OriginAndDestinationSquare>();
            List<OriginAndDestinationSquare> makeKingArray = new List<OriginAndDestinationSquare>();
            OriginAndDestinationSquare chosenMove;
            Random random = new Random();
            int randomIndex = 0;

            if (i_OptionsArray.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < i_OptionsArray.Count; i++)
            {
                if (IsCaptureMoveForComputer(i_OptionsArray[i], i_Board) == true)
                {
                    capturesArray.Add(i_OptionsArray[i]);
                }
            }

            for (int i = 0; i < i_OptionsArray.Count; i++)
            {
                if (CheckIfComputerCanMakeKing(i_OptionsArray[i], i_Board) == true)
                {
                    makeKingArray.Add(i_OptionsArray[i]);
                }
            }

            if (capturesArray.Count != 0)
            {
                randomIndex = random.Next(capturesArray.Count);
                chosenMove = capturesArray[randomIndex];
            }
            else if (makeKingArray.Count != 0)
            {
                randomIndex = random.Next(makeKingArray.Count);
                chosenMove = makeKingArray[randomIndex];
            }
            else
            {
                randomIndex = random.Next(i_OptionsArray.Count);
                chosenMove = i_OptionsArray[randomIndex];
            }

            StringBuilder generatedMove = new StringBuilder(".....");
            generatedMove[0] = (char)(chosenMove.m_ColOrigin + 'A');
            generatedMove[1] = (char)(chosenMove.m_RowOrigin + 'a');
            generatedMove[2] = '>';
            generatedMove[3] = (char)(chosenMove.m_ColDestination + 'A');
            generatedMove[4] = (char)(chosenMove.m_RowDestination + 'a');

            return generatedMove.ToString();
        }

        private bool CheckIfComputerCanMakeKing(OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool returnFlag = false;
            Square originSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowOrigin, i_FromAndToSquare.m_ColOrigin];
            Square destinationSquare = i_Board.GameBoard[i_FromAndToSquare.m_RowDestination, i_FromAndToSquare.m_ColDestination];

            if (originSquare.SquareOwner == eSquareOwner.Player2 && i_FromAndToSquare.m_RowDestination == i_Board.BoardSize - 1)
            {
                returnFlag = true;
            }
            else if (originSquare.SquareOwner == eSquareOwner.Player1 && i_FromAndToSquare.m_RowDestination == 0)
            {
                returnFlag = true;
            }

            return returnFlag;
        }

        internal bool FindDoubleCapture(ConsoleUI i_UI, Board i_Board, List<OriginAndDestinationSquare> i_CaptureOptionsArray)
        {
            bool returnFlag = false;
            OriginAndDestinationSquare oldFromAndToSquare = new OriginAndDestinationSquare();
            oldFromAndToSquare = i_UI.ParseMove(this.PlayerMove, oldFromAndToSquare);
            OriginAndDestinationSquare newFromAndToSquare = new OriginAndDestinationSquare();
            newFromAndToSquare.m_RowOrigin = oldFromAndToSquare.m_RowDestination;
            newFromAndToSquare.m_ColOrigin = oldFromAndToSquare.m_ColDestination;
            returnFlag = GiveArrayOfCaptureOptionsFromOneSquare(i_CaptureOptionsArray, newFromAndToSquare, i_Board);
            return returnFlag;
        }

        private bool GiveArrayOfCaptureOptionsFromOneSquare(List<OriginAndDestinationSquare> i_CaptureOptionsArray, OriginAndDestinationSquare i_FromAndToSquare, Board i_Board)
        {
            bool returnFlag = false;
            int[,] captureDirections = { { -2, -2 }, { -2, 2 }, { 2, -2 }, { 2, 2 } };

            for (int i = 0; i < captureDirections.GetLength(0); i++)
            {
                int potentialRow = i_FromAndToSquare.m_RowOrigin + captureDirections[i, 0];
                int potentialCol = i_FromAndToSquare.m_ColOrigin + captureDirections[i, 1];
                i_FromAndToSquare.m_RowDestination = potentialRow;
                i_FromAndToSquare.m_ColDestination = potentialCol;

                if (this.CheckIfLegal(i_FromAndToSquare, i_Board) == true)
                {
                    i_CaptureOptionsArray.Add(i_FromAndToSquare);
                    returnFlag = true;
                }
            }

            return returnFlag;
        }

        internal bool ReEnterAnotherMove(ConsoleUI i_UI, string i_PlayerMove, List<OriginAndDestinationSquare> i_CaptureOptionsArray, Board i_Board)
        {
            bool isValidMove = false;
            OriginAndDestinationSquare chosenMove = new OriginAndDestinationSquare();
            chosenMove = i_UI.ParseMove(i_PlayerMove, chosenMove);

            for (int i = 0; i < i_CaptureOptionsArray.Count; i++) 
            {
                if(i_CaptureOptionsArray[i].Equals(chosenMove))
                {
                    MovePiece(chosenMove, i_Board);
                    isValidMove = true;
                }
            }

            return isValidMove;
        }
    }
}