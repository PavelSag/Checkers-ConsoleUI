using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    internal class ConsoleUI
    {
        public bool RunGame(Board i_Board, Player i_Player1, Player i_Player2)
        {
            GameLogic gameLogic = new GameLogic();
            string previousPlayerName = null;
            bool isPieceCaptured = false;
            bool isDraw = false;

            while (gameLogic.IsGameGoing == true)
            {
                Console.Clear();
                this.PrintBoard(i_Board);
                this.PrintPreviousPlayer(previousPlayerName, gameLogic.PlayerMove, GameLogic.s_TurnStatus);
                gameLogic.PlayerMove = this.GetPlayerMove(out previousPlayerName, i_Player1, i_Player2, i_Board, gameLogic);
                gameLogic.IsForfeited = this.IsForfeit(gameLogic.PlayerMove);
                if(gameLogic.IsForfeited == true)
                {
                    goto Winner;
                }

                bool isLegal = gameLogic.CheckIfLegalAndMove(ref isPieceCaptured, i_Board, this);
                if (isLegal != true)
                {
                    gameLogic.PlayerMove = this.ReEnterMove(out previousPlayerName, i_Player1, i_Player2, i_Board, gameLogic);
                    if(gameLogic.PlayerMove == null)
                    {
                        gameLogic.IsForfeited = true;
                        goto Winner;
                    }
                }

                while(isPieceCaptured == true)
                {
                    List<OriginAndDestinationSquare> captureOptionsArray = new List<OriginAndDestinationSquare>();
                    gameLogic.DecrementNumOfPieces(i_Player1, i_Player2);
                    if (gameLogic.FindDoubleCapture(this, i_Board, captureOptionsArray) == true)
                    {
                        Console.Clear();
                        this.PrintBoard(i_Board);
                        gameLogic.PlayerMove = this.GetPlayerMove(out previousPlayerName, i_Player1, i_Player2, i_Board, gameLogic);
                        bool isAnotherMoveValid = gameLogic.ReEnterAnotherMove(this, gameLogic.PlayerMove, captureOptionsArray, i_Board);
                        while(isAnotherMoveValid != true)
                        {
                            Console.WriteLine("Invalid output!");
                            gameLogic.PlayerMove = this.GetPlayerMove(out previousPlayerName, i_Player1, i_Player2, i_Board, gameLogic);
                            isAnotherMoveValid = gameLogic.ReEnterAnotherMove(this, gameLogic.PlayerMove, captureOptionsArray, i_Board);
                            gameLogic.DecrementNumOfPieces(i_Player1, i_Player2);
                        }
                    }
                    else
                    {
                        isPieceCaptured = false;
                    }
                }

            Winner:
                gameLogic.CheckForWinner(out isDraw, i_Player1, i_Player2, i_Board);
                if (gameLogic.Winner != null || isDraw == true)// if winner was declared
                {
                    if(gameLogic.Winner != null)
                    {
                        this.PrintWinnerAndLoser(i_Board, gameLogic);
                    }
                    else
                    {
                        this.PrintDraw(gameLogic);
                    }

                    gameLogic.IsAnotherGame = this.AskAnotherGame();
                    gameLogic.IsGameGoing = false;
                }
                else
                {
                    gameLogic.ChangeTurn();
                }                
            }

            return gameLogic.IsAnotherGame;
        }

        private void PrintDraw(GameLogic i_GameLogic)
        {
            Console.WriteLine("There is a DRAW!");
            Console.WriteLine($"Current Score:" +
                $"{i_GameLogic.Winner.PlayerName} HAS {i_GameLogic.Winner.NumOfPoints} POINTS, {i_GameLogic.Loser.PlayerName} HAS {i_GameLogic.Loser.NumOfPoints} POINTS");
        }

        private void PrintWinnerAndLoser(Board i_Board, GameLogic i_GameLogic)
        {
            Console.WriteLine($"{i_GameLogic.Winner.PlayerName} IS THE WINNER!!");
            i_GameLogic.Winner.NumOfPoints = i_Board.CalculatePoints(i_GameLogic.Winner, i_GameLogic.Loser);
            Console.WriteLine($"WINNER({i_GameLogic.Winner.PlayerName}) HAS {i_GameLogic.Winner.NumOfPoints} POINTS, LOSER({i_GameLogic.Loser.PlayerName}) HAS {i_GameLogic.Loser.NumOfPoints} POINTS");
        }

        private bool AskAnotherGame()
        {
            bool isValidInput = false;
            bool isAnotherGame = false;

            while (isValidInput != true)
            {
                Console.WriteLine("Would you like to play another game? Press YES or NO");
                string userInput = Console.ReadLine();
                userInput = userInput.ToUpper();

                if (userInput.Equals("YES"))
                {
                    GameLogic.s_ComputerIndicator = true;
                    isAnotherGame = true;
                    isValidInput = true;
                }
                else if (userInput.Equals("NO"))
                {
                    GameLogic.s_ComputerIndicator = false;
                    isAnotherGame = false;
                    isValidInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please type YES or NO.");
                }
            }

            return isAnotherGame;
        }

        private bool IsForfeit(string i_PlayerInput)
        {
            bool returnFlag = false;   

            if (i_PlayerInput.ToUpper().Equals("Q"))
            {
                returnFlag = true;
            }

            return returnFlag;
        }

        internal string GetPlayerMove(out string i_PreviousPlayerName, Player i_Player1, Player i_Player2, Board i_Board, GameLogic i_gameLogic)
        {
            string playerMove = null;

            if (GameLogic.s_TurnStatus == eTurnStatus.Player2Turn && GameLogic.s_ComputerIndicator == true)
            {
                Console.WriteLine("Computer's Turn(press 'enter to see it's move)");
                Console.ReadLine();
                playerMove = i_gameLogic.GenerateComputerMove(i_Board);
                Console.WriteLine($"{playerMove}");

                if (playerMove != null)
                {
                    Console.WriteLine($"Computer chose: {playerMove}");
                }
                else
                {
                    Console.WriteLine("Computer has no legal moves. It forfeits.");
                }

                i_PreviousPlayerName = "Computer";
                GameLogic.s_ComputerIndicator = true;
            }
            else if (GameLogic.s_TurnStatus == eTurnStatus.Player1Turn)
            {
                playerMove = GetInputFromPlayer(i_Player1);
                i_PreviousPlayerName = i_Player1.PlayerName;
            }
            else
            {
                playerMove = GetInputFromPlayer(i_Player2);
                i_PreviousPlayerName = i_Player2.PlayerName;
            }

            return playerMove;
        }

        internal static int GetBoardSize()
        {
            int boardSize = 0;
            Console.WriteLine("Please enter the board size. You can choose sizes: 6, 8, 10:");
            boardSize = int.Parse(Console.ReadLine());
            return boardSize;
        }

        internal static int CheckBoardSize(int i_BoardSize)
        {
            while (i_BoardSize != 6 && i_BoardSize != 8 && i_BoardSize != 10)
            {
                Console.WriteLine("Invalid output! Board Size must be either 6, 8 or 10");
                i_BoardSize = int.Parse(Console.ReadLine());
            }

            return i_BoardSize;
        }

        internal static string GetPlayerName()
        {
            string playerName = null;
            Console.WriteLine("Please enter your name:");
            playerName = Console.ReadLine();
            return playerName;
        }

        internal static string CheckPlayerName(string i_PlayerMove)
        {
            while (i_PlayerMove.Length > 20 || i_PlayerMove.Contains(" ") == true)
            {
                Console.WriteLine("Invalid input! Your name must be maximum 20 lettes and without spaces");
                i_PlayerMove = Console.ReadLine();
            }

            return i_PlayerMove;
        }

        internal bool IsVersusComputer()
        {
            bool isValidInput = false;
            bool isVersusComputer = false;

            while (isValidInput != true)
            {
                Console.WriteLine("Would you like to play versus the computer? Press YES or NO");
                string userInput = Console.ReadLine();
                userInput = userInput.ToUpper();

                if (userInput.Equals("YES"))
                {
                    GameLogic.s_ComputerIndicator = true;
                    isVersusComputer = true;
                    isValidInput = true;
                }
                else if (userInput.Equals("NO"))
                {
                    GameLogic.s_ComputerIndicator = false;
                    isVersusComputer = false;
                    isValidInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please type YES or NO.");
                }
            }

            return isVersusComputer;
        }

        internal void PrintBoard(Board i_Board)
        {
            Console.WriteLine("   " + string.Join("   ", GetColumnHeaders(i_Board.BoardSize)));
            for (int row = 0; row < i_Board.BoardSize; row++)
            {
                Console.Write("  ");
                Console.WriteLine(new string('=', i_Board.BoardSize * 4));
                Console.Write($"{(char)('a' + row)}|");
                for (int col = 0; col < i_Board.BoardSize; col++)
                {
                    Console.Write($" {GetSquareSymbol(i_Board.GameBoard[row, col])} |");
                }

                Console.WriteLine();
            }
            Console.Write("  ");
            Console.WriteLine(new string('=', i_Board.BoardSize * 4));
        }

        private string[] GetColumnHeaders(int i_BoardSize)
        {
            string[] headers = new string[i_BoardSize];
            for (int i = 0; i < i_BoardSize; i++)
            {
                headers[i] = ((char)('A' + i)).ToString();
            }

            return headers;
        }

        private char GetSquareSymbol(Square square)
        {
            if (square.SquareOwner == eSquareOwner.Player1)
            {
                if(square.PieceInSquare == ePieceInSquare.RegularPiece)
                {
                    return 'O';
                }
                else
                {
                    return 'U';
                } 
            }
            else if (square.SquareOwner == eSquareOwner.Player2)
            {
                if(square.PieceInSquare == ePieceInSquare.RegularPiece)
                {
                    return 'X';
                }
                else
                {
                    return 'K';
                }
            }
            else
            {
                return ' ';
            }
        }

        internal string GetInputFromPlayer(Player i_Player)
        {
            Console.WriteLine($"{i_Player.PlayerName}'s turn: ");
            string moveStr = Console.ReadLine();
            return moveStr;
        }

        internal void PrintPreviousPlayer(string i_PreviousPlayerName, string i_PreviousMove, eTurnStatus i_s_TurnStatus)
        {
            char symbol = 'X';

            if(i_s_TurnStatus == eTurnStatus.Player1Turn)
            {
                symbol = 'X';
            }
            else
            {
                symbol = 'O';
            }

            if(i_PreviousPlayerName != null)
            {
                Console.WriteLine($"{i_PreviousPlayerName}'s move was ({(symbol)}): {i_PreviousMove}");
            }
        }

        internal string ReEnterMove(out string i_PreviousPlayerName, Player i_Player1, Player i_Player2, Board i_Board, GameLogic i_gameLogic)
        {
            bool isLegal = false;
            string previousPlayerName = null;
            bool isPieceCaptured = false;
            bool isForfeited = false;

            while (isLegal != true)
            {
                Console.WriteLine("Invalid input!");
                i_gameLogic.PlayerMove = GetPlayerMove(out previousPlayerName, i_Player1, i_Player2, i_Board, i_gameLogic);
                isForfeited = IsForfeit(i_gameLogic.PlayerMove);
                if(isForfeited == true)
                {
                    i_gameLogic.PlayerMove = null;
                }

                isLegal = i_gameLogic.CheckIfLegalAndMove(ref isPieceCaptured, i_Board, this);
            }

            i_PreviousPlayerName = previousPlayerName;
            return i_gameLogic.PlayerMove;
        }

        internal bool CheckInputSyntax(string i_PlayerMove)
        {
            bool returnFlag = true;

            if (i_PlayerMove.Length != 5)
            {
                returnFlag = false;
            }
            else
            {
                if ((Char.IsUpper(i_PlayerMove[0]) != true) || (Char.IsLower(i_PlayerMove[1]) != true) || (Char.IsUpper(i_PlayerMove[3]) != true) || (Char.IsLower(i_PlayerMove[4]) != true))
                {
                    returnFlag = false;
                }

                if (i_PlayerMove[2] != '>')
                {
                    returnFlag = false;
                }
            }

            return returnFlag;
        }

        internal OriginAndDestinationSquare ParseMove(string i_PlayerMove, OriginAndDestinationSquare i_FromAndToSquare)
        {
            i_FromAndToSquare.m_ColOrigin = i_PlayerMove[0] - 'A';
            i_FromAndToSquare.m_RowOrigin = i_PlayerMove[1] - 'a';
            i_FromAndToSquare.m_ColDestination = i_PlayerMove[3] - 'A';
            i_FromAndToSquare.m_RowDestination = i_PlayerMove[4] - 'a';
            return i_FromAndToSquare;
        }

        internal void PrintGoodbye()
        {
            Console.WriteLine("GG WP! GOODBYE!");
            Console.WriteLine("Press 'enter' to exit");
            Console.ReadLine();
        }
    }
}
